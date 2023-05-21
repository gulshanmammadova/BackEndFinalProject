using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_Project_BackEnd.Migrations
{
    public partial class EdittedClases2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Comments",
                newName: "Commenttext");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Commenttext",
                table: "Comments",
                newName: "Description");
        }
    }
}
