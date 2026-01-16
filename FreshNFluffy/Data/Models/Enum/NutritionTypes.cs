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

    public enum OrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        Ready = 2,
        Completed = 3,
        Cancelled = 4
    }
}
