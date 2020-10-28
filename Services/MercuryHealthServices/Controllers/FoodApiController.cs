using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MercuryHealthServices.Models;

namespace MercuryHealthServices.Controllers
{
    [Produces("application/json")]
    [Route("FoodApi")]
    public class FoodApiController : Controller
    {
        // mercury health api
        private string _MercuryHealthAPIUrl; // = "http://localhost:3972";

        private IConfiguration _config;

        public FoodApiController(IConfiguration config)
        {
            _config = config;
            _MercuryHealthAPIUrl = _config.GetValue<string>("MercuryHealthAPIUrl");
        }

        [Route("DeleteFoodItem")]
        public async Task DeleteFoodItem(string id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_MercuryHealthAPIUrl);
                await client.DeleteAsync("api/FoodLogApi/" + id);
            }
        }
        
    }
}