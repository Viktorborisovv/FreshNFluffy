namespace FreshNFluffy.Data.Models.Enum
{
    [Flags]
    public enum NutritionTypes
    {
        None = 0,
        GlutenFree = 1,
        SugarFree = 2,
        LactoseFree = 3,
        Vegan = 4,
        Keto = 5
    }
}
