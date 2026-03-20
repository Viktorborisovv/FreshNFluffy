namespace FreshNFluffy.Common
{
    public class EntityValidation
    {
        public static class Product
        {
            public const int ProductNameMinLength = 3;
            public const int ProductNameMaxLength = 35;
            public const int DescriptionMinLength = 15;
            public const int DescriptionMaxLength = 1000;
            public const int ProductImageUrlMaxLength = 2048;
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

        public static class OrderItem
        {
            public const string UnitPriceSqlType = "DECIMAL(9,2)";
            public const int QuantityMin = 1;
            public const int QuantityMax = 100;
        }

        public static class Review
        {
            public const int CommentMinLength = 10;
            public const int CommentMaxLength = 1000;
            public const int RatingMin = 1;
            public const int RatingMax = 5;
        }
    }
}
