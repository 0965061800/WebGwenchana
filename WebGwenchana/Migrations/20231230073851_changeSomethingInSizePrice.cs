using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGwenchana.Migrations
{
    /// <inheritdoc />
    public partial class changeSomethingInSizePrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ProductPrice",
                table: "SizesPrice",
                type: "decimal(20,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ProductPrice",
                table: "SizesPrice",
                type: "decimal(8,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,2)",
                oldNullable: true);
        }
    }
}
