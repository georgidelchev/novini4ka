using Microsoft.EntityFrameworkCore.Migrations;

namespace Novinichka.Data.Migrations
{
    public partial class AddBigBannerUrlToSourcesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DefaultImageUrl",
                table: "Sources",
                newName: "SmallBannerUrl");

            migrationBuilder.AddColumn<string>(
                name: "BigBannerUrl",
                table: "Sources",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BigBannerUrl",
                table: "Sources");

            migrationBuilder.RenameColumn(
                name: "SmallBannerUrl",
                table: "Sources",
                newName: "DefaultImageUrl");
        }
    }
}
