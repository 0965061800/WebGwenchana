using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGwenchana.Migrations
{
    /// <inheritdoc />
    public partial class updatePropertiesInOrderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "District",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Ward",
                table: "Orders",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "District",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Ward",
                table: "Orders");
        }
    }
}
