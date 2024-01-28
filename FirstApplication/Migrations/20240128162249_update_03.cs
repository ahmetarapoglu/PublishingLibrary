using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShop.Migrations
{
    /// <inheritdoc />
    public partial class update_03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LibraryRatio",
                table: "VersionBooks");

            migrationBuilder.AddColumn<int>(
                name: "LibraryRatio",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LibraryRatio",
                table: "Books");

            migrationBuilder.AddColumn<int>(
                name: "LibraryRatio",
                table: "VersionBooks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
