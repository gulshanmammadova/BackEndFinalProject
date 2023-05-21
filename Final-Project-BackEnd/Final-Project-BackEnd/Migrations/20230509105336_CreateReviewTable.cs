using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_Project_BackEnd.Migrations
{
    public partial class CreateReviewTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stat",
                table: "Reviews",
                newName: "Star");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Star",
                table: "Reviews",
                newName: "Stat");
        }
    }
}
