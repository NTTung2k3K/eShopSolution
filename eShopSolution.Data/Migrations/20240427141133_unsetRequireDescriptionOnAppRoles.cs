using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShopSolution.Data.Migrations
{
    public partial class unsetRequireDescriptionOnAppRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "3fd1c623-0847-4296-a8cd-8be643ecb2af");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "59929e76-a5c3-4e7c-a490-b9a4b67aa861", "AQAAAAEAACcQAAAAEKziHISkYBDsOUKQ7ZmNPDxAG1og7PEMbWZGmUyaZITpH/uLqahSvde4muUNjK3vCQ==" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2024, 4, 27, 21, 11, 32, 346, DateTimeKind.Local).AddTicks(6352));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "8f231966-0e9f-4f8e-8b9c-73b9567026a4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "fcd7df34-2d31-4902-aa0e-5b10cc927e10", "AQAAAAEAACcQAAAAEJZBRCgERojBYorf0QhhItuNcdGeJVlIeU5MR8k/Yyw1W2SJJwUceJ6sei0b70e0NA==" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2024, 4, 26, 20, 19, 44, 276, DateTimeKind.Local).AddTicks(1925));
        }
    }
}
