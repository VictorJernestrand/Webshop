using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Migrations
{
    public partial class AddNews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    Text = table.Column<string>(nullable: false),
                    NewsDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "News",
                columns: new[] { "Id", "NewsDate", "Text", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2020, 4, 8, 3, 52, 3, 240, DateTimeKind.Local).AddTicks(4938), "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam mollis eu arcu at rhoncus. Cras ut felis dui. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Aenean vitae aliquet dui. Suspendisse fermentum risus ut arcu condimentum, nec fringilla turpis mattis.", "Butiken växer" },
                    { 2, new DateTime(2020, 4, 10, 3, 52, 3, 243, DateTimeKind.Local).AddTicks(1751), "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam mollis eu arcu at rhoncus. Cras ut felis dui. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Aenean vitae aliquet dui. Suspendisse fermentum risus ut arcu condimentum, nec fringilla turpis mattis.", "Nya Produkter" },
                    { 3, new DateTime(2020, 4, 11, 3, 52, 3, 243, DateTimeKind.Local).AddTicks(1873), "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam mollis eu arcu at rhoncus. Cras ut felis dui. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Aenean vitae aliquet dui. Suspendisse fermentum risus ut arcu condimentum, nec fringilla turpis mattis.", "Pressade Priser" },
                    { 4, new DateTime(2020, 4, 13, 3, 52, 3, 243, DateTimeKind.Local).AddTicks(1899), "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam mollis eu arcu at rhoncus. Cras ut felis dui. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Aenean vitae aliquet dui. Suspendisse fermentum risus ut arcu condimentum, nec fringilla turpis mattis.", "Som en käftsmäll" },
                    { 5, new DateTime(2020, 4, 15, 3, 52, 3, 243, DateTimeKind.Local).AddTicks(1916), "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam mollis eu arcu at rhoncus. Cras ut felis dui. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Aenean vitae aliquet dui. Suspendisse fermentum risus ut arcu condimentum, nec fringilla turpis mattis.", "Vi provar RG2750" }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "Photo",
                value: "Guitar/guitar1_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "Photo",
                value: "Piano/piano1_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "Photo",
                value: "Piano/piano2_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "Photo",
                value: "Piano/piano3_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "Photo",
                value: "Bas/bas1_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "Photo",
                value: "Bas/bas2_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                column: "Photo",
                value: "Drum set/drum1_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                column: "Photo",
                value: "Keyboard/keyboard1_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                column: "Photo",
                value: "Keyboard/keyboard2_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                column: "Photo",
                value: "Piano/piano4_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11,
                column: "Photo",
                value: "Guitar/guitar2_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12,
                column: "Photo",
                value: "Keyboard/keyboard3_original.jpg");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "Photo",
                value: "Guitar\\guitar1_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "Photo",
                value: "Piano\\piano1_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "Photo",
                value: "Piano\\piano2_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "Photo",
                value: "Piano\\piano3_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "Photo",
                value: "Bas\\bas1_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "Photo",
                value: "Bas\\bas2_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                column: "Photo",
                value: "Drum set\\drum1_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                column: "Photo",
                value: "Keyboard\\keyboard1_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                column: "Photo",
                value: "Keyboard\\keyboard2_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                column: "Photo",
                value: "Piano\\piano4_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11,
                column: "Photo",
                value: "Guitar\\guitar2_original.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12,
                column: "Photo",
                value: "Keyboard\\keyboard3_original.jpg");
        }
    }
}
