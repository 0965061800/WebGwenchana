using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGwenchana.Migrations
{
    /// <inheritdoc />
    public partial class addMorePropertyInLocationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Levels",
                table: "Locations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameWithType",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Parent",
                table: "Locations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChangePasswordViewModel",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PasswordNow = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfirmPassword = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangePasswordViewModel", x => x.CustomerId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChangePasswordViewModel");

            migrationBuilder.DropColumn(
                name: "Levels",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "NameWithType",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Parent",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Locations");
        }
    }
}
