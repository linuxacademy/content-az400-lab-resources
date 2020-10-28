using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MercuryHealthServices.Models
{
    public class FoodModel
    {
        public string food_name { get; set; }
        public int serving_qty { get; set; }
        public string serving_unit { get; set; }
        public decimal serving_weight_grams { get; set; }
        public decimal nf_calories { get; set; }
        public decimal nf_total_fat { get; set; }
        public decimal nf_protein { get; set; }
        public decimal nf_total_carbohydrate { get; set; }
        public decimal nf_sodium { get; set; }
    }
}
