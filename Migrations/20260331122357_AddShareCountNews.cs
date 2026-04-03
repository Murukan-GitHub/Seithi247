using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seithi247.Migrations
{
    /// <inheritdoc />
    public partial class AddShareCountNews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShareCount",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShareCount",
                table: "News");
        }
    }
}
