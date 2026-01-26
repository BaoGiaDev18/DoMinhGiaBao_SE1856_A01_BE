using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoMinhGiaBao_SE1856_A01_Repository.Migrations
{
    /// <inheritdoc />
    public partial class FixNewsTagCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsTag_NewsArticle",
                table: "NewsTag");

            migrationBuilder.DropForeignKey(
                name: "FK_NewsTag_Tag",
                table: "NewsTag");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsTag_NewsArticle",
                table: "NewsTag",
                column: "NewsArticleID",
                principalTable: "NewsArticle",
                principalColumn: "NewsArticleID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NewsTag_Tag",
                table: "NewsTag",
                column: "TagID",
                principalTable: "Tag",
                principalColumn: "TagID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsTag_NewsArticle",
                table: "NewsTag");

            migrationBuilder.DropForeignKey(
                name: "FK_NewsTag_Tag",
                table: "NewsTag");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsTag_NewsArticle",
                table: "NewsTag",
                column: "NewsArticleID",
                principalTable: "NewsArticle",
                principalColumn: "NewsArticleID");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsTag_Tag",
                table: "NewsTag",
                column: "TagID",
                principalTable: "Tag",
                principalColumn: "TagID");
        }
    }
}
