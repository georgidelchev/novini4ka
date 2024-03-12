using Microsoft.EntityFrameworkCore.Migrations;

namespace Novinichka.Data.Migrations
{
    public partial class AddOriginalSourceIdToNewsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalSourceId",
                table: "News",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: string.Empty);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalSourceId",
                table: "News");
        }
    }
}
