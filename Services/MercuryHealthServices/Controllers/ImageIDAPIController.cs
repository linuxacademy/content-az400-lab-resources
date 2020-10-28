using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MercuryHealthServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace MercuryHealthServices.Controllers
{
    [Produces("application/json")]
    //[Route("api/ImageID")]
    [Route("ImageIDAPI")]
    public class ImageIDAPIController : Controller
    {
        // cognative services
        private string _SubscriptionKey;
        private string _URIBase;

        // nutrionix
        private string _NutritionixId;
        private string _NutritionixKey;
        private string _NutritionixURIBase;

        // mercury health api
        private string _MercuryHealthAPIUrl; // = "http://localhost:3972";

        private IConfiguration _config;

        public ImageIDAPIController(IConfiguration config)
        {
            _config = config;

            _SubscriptionKey = _config.GetValue<string>("VisionSubscriptionKey");
            _URIBase = _config.GetValue<string>("CognativeServiceURIBase");
            _NutritionixId = _config.GetValue<string>("NutritionixId");
            _NutritionixKey = _config.GetValue<string>("NutritionixKey");
            _NutritionixURIBase = _config.GetValue<string>("NutritionixURIBase");
            _MercuryHealthAPIUrl = _config.GetValue<string>("MercuryHealthAPIUrl");
        }

        [Route("UploadFoodImage")]
        public async Task<NutritionModel> UploadFoodImage(IList<IFormFile> files)
        {
            // get image from list of files uploaded
            IFormFile uploadedImage = files.FirstOrDefault();
            if (uploadedImage != null && uploadedImage.ContentType.ToLower().StartsWith("image/"))
            {
                // read the image into a memory stream
                MemoryStream ms = new MemoryStream();
                uploadedImage.OpenReadStream().CopyTo(ms);
                byte[] pictureByteArray = ms.ToArray();

                return await IDImageAndGetNutrition(pictureByteArray);
            }

            return null;
        }

        /// <summary>
        /// Upload image, try to read it from HttpContent as a string
        /// </summary>
        /// <returns></returns>
        [Route("UploadFoodImage1")]
        [HttpPost]
        public async Task<NutritionModel> UploadFoodImage1()
        {
            using (var streamReader = new MemoryStream())
            {
                // read picture from body of post as a serialized string
                this.Request.Body.CopyTo(streamReader);
                var pictureSerialized = System.Text.Encoding.UTF8.GetString(streamReader.ToArray());

                // deserialize it as a byte[]
                var pictureByteArray = JsonConvert.DeserializeObject<byte[]>(pictureSerialized);

                // ID the picture and then get nutrition info
                return await IDImageAndGetNutrition(pictureByteArray);
            }
        }

        /// <summary>
        /// Upload image, byte array automatically mapped and deserialized
        /// </summary>
        /// <returns></returns>
        [Route("UploadFoodImage2")]
        [HttpPost]
        public async Task<NutritionModel> UploadFoodImage2(byte[] pictureByteArray)
        {
            // ID the picture and then get nutrition info
            return await IDImageAndGetNutrition(pictureByteArray);
        }
        

        private async Task<NutritionModel> IDImageAndGetNutrition(byte[] pictureByteArray)
        {
            // identify food using cognative services, create cognative service vision
            // api client and look for tags and descriptions
            var visionClient = new VisionServiceClient(_SubscriptionKey);
            var features = new VisualFeature[] { VisualFeature.Tags, VisualFeature.Description };

            using (var ms = new MemoryStream(pictureByteArray))
            {
                // analyze image
                var analysisResult = await visionClient.AnalyzeImageAsync(ms, features);
                var description = string.Empty;
                if (analysisResult.Description.Captions.Length > 0)
                {
                    description = analysisResult.Description.Captions[0].Text;
                }

                // figure out what the food item is from the result
                var specificItem = string.Empty;
                var identified = false;
                foreach (var tag in analysisResult.Tags)
                {
                    if ((tag.Hint != null) && (tag.Hint.ToLower().Equals("food") && tag.Confidence > .6))
                    {
                        specificItem = tag.Name;
                        identified = true;
                        break;
                    }
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("x-app-id", _NutritionixId);
                    client.DefaultRequestHeaders.Add("x-app-key", _NutritionixKey);

                    var jsonPayload = "{\"query\": \"i ate " + description + "\"" + "}";
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(_NutritionixURIBase, content).Result;
                    var contentString = response.Content.ReadAsStringAsync().Result;

                    var nutritionInfo = JsonConvert.DeserializeObject<NutritionModel>(contentString);

                    await this.PostNewFoodItem(nutritionInfo);
                    return nutritionInfo;
                }
            }
        }

        private async Task PostNewFoodItem(NutritionModel nutritionModel)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_MercuryHealthAPIUrl);

                foreach (var food in nutritionModel.foods)
                {
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("Description", food.food_name),
                        new KeyValuePair<string, string>("Quantity", "1"),
                        new KeyValuePair<string, string>("Calories", ((int)food.nf_calories).ToString()),
                        new KeyValuePair<string, string>("ProteinInGrams", food.nf_protein.ToString()),
                        new KeyValuePair<string, string>("FatInGrams", food.nf_total_fat.ToString()),
                        new KeyValuePair<string, string>("CarbohydratesInGrams", food.nf_total_carbohydrate.ToString()),
                        new KeyValuePair<string, string>("SodiumInGrams", food.nf_sodium.ToString())
                    });
                    await client.PostAsync("FoodApi/LogFood", content);
                }


            }

        }

    }
}