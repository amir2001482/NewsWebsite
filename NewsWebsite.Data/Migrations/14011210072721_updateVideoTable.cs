using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsWebsite.Data.Migrations
{
    public partial class updateVideoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishDateTime",
                table: "Videos",
                nullable: true,
                defaultValueSql: "CONVERT(datetime,GetDate())",
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishDateTime",
                table: "Videos",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true,
                oldDefaultValueSql: "CONVERT(datetime,GetDate())");
        }
    }
}
