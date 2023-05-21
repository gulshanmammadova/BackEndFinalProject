using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_Project_BackEnd.Migrations
{
    public partial class AddedClases : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlogId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BlogId",
                table: "Reviews",
                column: "BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Blogs_BlogId",
                table: "Reviews",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Blogs_BlogId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_BlogId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "BlogId",
                table: "Reviews");
        }
    }
}
