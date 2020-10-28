namespace HealthClinic.Shared
{
    public static class PageTitleConstants
    {
        public const string AddFoodPage = "Add Food";
        public const string FoodListPage = "Food Consumed";

        public static string GetFoodListPage
        {
            get
            {
                return FoodListPage + " " + APIConstants.GetServiceIcon;
            }
        }

    }
}
