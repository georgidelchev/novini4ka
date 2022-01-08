using Microsoft.EntityFrameworkCore.Migrations;

namespace Novinichka.Data.Migrations
{
    public partial class RemoveSmallImageAndLeftOnlyImageUrlForNewsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BigImageUrl",
                table: "News");

            migrationBuilder.RenameColumn(
                name: "SmallImageUrl",
                table: "News",
                newName: "ImageUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "News",
                newName: "SmallImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "BigImageUrl",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
