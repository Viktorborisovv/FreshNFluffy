namespace FreshNFluffy.Common
{
    public class EntityValidation
    {
        public static class Product
        {
            public const int ProductNameMinLength = 3;
            public const int ProductNameMaxLength = 35;
            public const int DescriptionMinLength = 20;
            public const int DescriptionMaxLength = 100;
            public const string PriceSqlType = "DECIMAL(9,2)";
        }

        public static class Category
        {
            public const int CategoryNameMinLength = 3;
            public const int CategoryNameMaxLength = 100;
        }

        public static class OrderRequest
        {
            public const int CustomerNameMinLength = 2;
            public const int CustomerNameMaxLength = 25;
            public const int PhoneNumberLength = 13;
            public const int NotesMinLength = 6;
            public const int NotesMaxLength = 450;
        }
    }
}
