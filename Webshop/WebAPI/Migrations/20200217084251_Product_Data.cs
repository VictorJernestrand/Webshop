using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Migrations
{
    public partial class Product_Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "CategoryId", "Description", "Name", "Price", "Quantity" },
                values: new object[,]
                {
                    { 1, 2, 1, "Black and white", "Stratocaster", 4000m, 4 },
                    { 2, 2, 3, "Smooth", "Precision", 3000m, 5 },
                    { 3, 2, 3, "Blue bas", "Vintera", 4000m, 2 },
                    { 4, 1, 3, "Advanced", "Epiphone", 4000m, 2 },
                    { 5, 5, 2, "For kids", "Youngster", 1100m, 8 },
                    { 6, 5, 2, "For good players", "MPS-150X", 3200m, 4 },
                    { 7, 3, 2, "Nice set of drums", "DTX­432K", 5600m, 2 },
                    { 8, 3, 4, "Black and black", "P116M", 8000m, 1 },
                    { 9, 3, 4, "Old model", "Calvinova", 8900m, 1 },
                    { 10, 4, 4, "Digitalpiano", "B2SP", 2300m, 6 },
                    { 11, 4, 3, "Traveling model", "SP-280", 5300m, 3 },
                    { 12, 3, 4, "Our best keyboard", "P-45", 4900m, 3 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);
        }
    }
}
