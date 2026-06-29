using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seithi247.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCommentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add new columns to Comments table
            migrationBuilder.AddColumn<int>(
                name: "ParentCommentId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedDate",
                table: "Comments",
                type: "datetime2",
                nullable: true);

            // Add foreign key for parent-child relationship
            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Ensure CommentReactions table has proper indexes
            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_CommentId_UserIdentifier_Emoji",
                table: "CommentReactions",
                columns: new[] { "CommentId", "UserIdentifier", "Emoji" },
                unique: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_CommentReactions_CommentId_UserIdentifier_Emoji",
                table: "CommentReactions");

            migrationBuilder.DropColumn(
                name: "ParentCommentId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "EditedDate",
                table: "Comments");
        }
    }
}
