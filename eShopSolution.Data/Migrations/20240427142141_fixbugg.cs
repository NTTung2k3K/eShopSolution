using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShopSolution.Data.Migrations
{
    public partial class fixbugg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "84b8f88b-3199-408d-aa22-0eb4779f320d");
            migrationBuilder.AlterColumn<string>(
        name: "Description",
        table: "AspNetRoles",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "nvarchar(200)");
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "2b6cea21-188f-457b-b1e8-3e3547370d2e", "AQAAAAEAACcQAAAAEIzo68COKcw58UB4zDzPwD+uwteaKZNrCm7xs46s9v0Q2iqWXdchzJ3YTFp3V44q3g==" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2024, 4, 27, 21, 21, 40, 884, DateTimeKind.Local).AddTicks(4775));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
