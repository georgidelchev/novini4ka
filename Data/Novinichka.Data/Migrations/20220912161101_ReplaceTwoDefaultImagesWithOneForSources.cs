using Microsoft.EntityFrameworkCore.Migrations;

namespace Novinichka.Data.Migrations
{
    public partial class ReplaceTwoDefaultImagesWithOneForSources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BigBannerUrl",
                table: "Sources");

            migrationBuilder.RenameColumn(
                name: "SmallBannerUrl",
                table: "Sources",
                newName: "DefaultImagePath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DefaultImagePath",
                table: "Sources",
                newName: "SmallBannerUrl");

            migrationBuilder.AddColumn<string>(
                name: "BigBannerUrl",
                table: "Sources",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
