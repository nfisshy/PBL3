using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL_3.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNew3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalNetProfit",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TotalNetProfit",
                table: "OrderDetails");
        }
    }
}
