namespace FreshNFluffy.Data.Models.Enum
{
    [Flags]
    public enum NutritionTypes
    {
        None = 0,
        GlutenFree = 1,
        SugarFree = 2,
        LactoseFree = 4,
        Vegan = 8,
        Keto = 16
    }
}
