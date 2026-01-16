using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FreshNFluffy.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorrectedSeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "Description", "ImageUrl", "Name", "NutritionTypes", "Price" },
                values: new object[,]
                {
                    { 1, 1, "Rich chocolate sponge with fudge frosting.", "https://example.com/img/chocolate-fudge-cake.jpg", "Chocolate Fudge Cake", 0, 29.90m },
                    { 2, 1, "Moist carrot cake with walnuts and cinnamon.", "https://example.com/img/carrot-walnut-cake.jpg", "Carrot Walnut Cake", 0, 27.50m },
                    { 3, 1, "Plant-based chocolate cake, dairy-free.", "https://example.com/img/vegan-chocolate-cake.jpg", "Vegan Chocolate Cake", 12, 30.90m },
                    { 4, 2, "Classic French croissant, flaky and buttery.", "https://example.com/img/butter-croissant.jpg", "Butter Croissant", 0, 3.20m },
                    { 5, 2, "Croissant pastry filled with dark chocolate.", "https://example.com/img/pain-au-chocolat.jpg", "Pain au Chocolat", 0, 3.80m },
                    { 6, 2, "Crispy puff pastry with spiced apples, vegan.", "https://example.com/img/vegan-apple-turnover.jpg", "Vegan Apple Turnover", 12, 4.10m },
                    { 7, 3, "Naturally leavened sourdough bread.", "https://example.com/img/sourdough-loaf.jpg", "Sourdough Loaf", 0, 6.50m },
                    { 8, 3, "Hearty wholegrain loaf, great for sandwiches.", "https://example.com/img/wholegrain-bread.jpg", "Wholegrain Bread", 0, 5.90m },
                    { 9, 3, "Gluten-free loaf with mixed seeds.", "https://example.com/img/gluten-free-seed-bread.jpg", "Gluten-Free Seed Bread", 13, 7.40m },
                    { 10, 4, "Soft cookies with dark chocolate chips.", "https://example.com/img/choc-chip-cookies.jpg", "Chocolate Chip Cookies", 0, 6.90m },
                    { 11, 4, "Oat cookies with raisins and a hint of cinnamon.", "https://example.com/img/oat-raisin-cookies.jpg", "Oat & Raisin Cookies", 0, 6.20m },
                    { 12, 4, "Crunchy almond cookies without added sugar.", "https://example.com/img/sugar-free-almond-cookies.jpg", "Sugar-Free Almond Cookies", 19, 7.10m },
                    { 13, 5, "Creamy cheesecake with vanilla and biscuit base.", "https://example.com/img/classic-cheesecake.jpg", "Classic Cheesecake", 0, 22.90m },
                    { 14, 5, "Low-carb cheesecake dessert cup.", "https://example.com/img/keto-cheesecake-cup.jpg", "Keto Cheesecake Cup", 19, 8.90m },
                    { 15, 5, "Silky vegan mousse, dairy-free.", "https://example.com/img/vegan-chocolate-mousse.jpg", "Vegan Chocolate Mousse", 12, 9.20m },
                    { 16, 6, "Soft vanilla scone, perfect with tea.", "https://example.com/img/vanilla-scone.jpg", "Vanilla Sweet Scone", 0, 2.80m },
                    { 17, 6, "Buttery scone with blueberries.", "https://example.com/img/blueberry-scone.jpg", "Blueberry Sweet Scone", 0, 3.10m },
                    { 18, 7, "Savory scone with cheddar cheese.", "https://example.com/img/cheddar-scone.jpg", "Cheddar Salty Scone", 0, 3.40m },
                    { 19, 7, "Savory scone with olives and herbs.", "https://example.com/img/olive-herb-scone.jpg", "Olive & Herb Salty Scone", 0, 3.60m },
                    { 20, 7, "Low-carb savory scone, keto friendly.", "https://example.com/img/keto-salty-scone.jpg", "Keto Salty Scone", 17, 4.20m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 20);
        }
    }
}
