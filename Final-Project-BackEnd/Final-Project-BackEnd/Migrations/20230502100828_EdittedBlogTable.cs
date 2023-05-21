using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_Project_BackEnd.Migrations
{
    public partial class EdittedBlogTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Blogs_BlogId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_BlogId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "BlogId",
                table: "ProductImages");

            migrationBuilder.AddColumn<string>(
                name: "CreatorImage",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorImage",
                table: "Blogs");

            migrationBuilder.AddColumn<int>(
                name: "BlogId",
                table: "ProductImages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_BlogId",
                table: "ProductImages",
                column: "BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Blogs_BlogId",
                table: "ProductImages",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "Id");
        }
    }
}
