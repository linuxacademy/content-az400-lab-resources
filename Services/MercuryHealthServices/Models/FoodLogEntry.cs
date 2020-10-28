using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MercuryHealthServices.Models
{
    public class FoodLogEntry
    {

        public int Id { get; set; }

        
        public string Description { get; set; }

        public float Quantity { get; set; }

        public DateTime MealTime { get; set; }

        public string Tags { get; set; }

        public int Calories { get; set; }

        public decimal ProteinInGrams { get; set; }

        public decimal FatInGrams { get; set; }

        public decimal CarbohydratesInGrams { get; set; }

        public decimal SodiumInGrams { get; set; }

        //[DisplayName("Color")]
        //public string Color { get; set; }

        // added some more stuf

    }
}
