using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_Project_BackEnd.Migrations
{
    public partial class Teamtableedit_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "Teams",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instagram",
                table: "Teams",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Twitter",
                table: "Teams",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Instagram",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Twitter",
                table: "Teams");
        }
    }
}
