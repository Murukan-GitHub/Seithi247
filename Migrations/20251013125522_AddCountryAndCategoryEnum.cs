using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seithi247.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryAndCategoryEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Country",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NewsCategory",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "News");

            migrationBuilder.DropColumn(
                name: "NewsCategory",
                table: "News");
        }
    }
}
