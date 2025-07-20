using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataSeek.Web.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UploadLines_UploadFileId",
                table: "UploadLines",
                column: "UploadFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadLines_UploadFiles_UploadFileId",
                table: "UploadLines",
                column: "UploadFileId",
                principalTable: "UploadFiles",
                principalColumn: "UploadFileId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadLines_UploadFiles_UploadFileId",
                table: "UploadLines");

            migrationBuilder.DropIndex(
                name: "IX_UploadLines_UploadFileId",
                table: "UploadLines");
        }
    }
}
