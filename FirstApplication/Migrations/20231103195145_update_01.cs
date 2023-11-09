using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShop.Migrations
{
    /// <inheritdoc />
    public partial class update_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInvoiced",
                table: "Invoices");

            migrationBuilder.AddColumn<bool>(
                name: "IsInvoiced",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInvoiced",
                table: "Orders");

            migrationBuilder.AddColumn<bool>(
                name: "IsInvoiced",
                table: "Invoices",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }
    }
}
