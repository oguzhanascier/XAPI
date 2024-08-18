using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTweetUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tweets_AspNetUsers_UserId1",
                table: "Tweets");

            migrationBuilder.DropIndex(
                name: "IX_Tweets_UserId1",
                table: "Tweets");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Tweets");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Tweets",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Tweets_UserId",
                table: "Tweets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tweets_AspNetUsers_UserId",
                table: "Tweets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tweets_AspNetUsers_UserId",
                table: "Tweets");

            migrationBuilder.DropIndex(
                name: "IX_Tweets_UserId",
                table: "Tweets");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Tweets",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Tweets",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tweets_UserId1",
                table: "Tweets",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Tweets_AspNetUsers_UserId1",
                table: "Tweets",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
