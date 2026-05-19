using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestoraApp.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_sleep_StudentID",
                table: "sleep",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_mood_StudentID",
                table: "mood",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_diary_StudentID",
                table: "diary",
                column: "StudentID");

            migrationBuilder.AddForeignKey(
                name: "FK_diary_Students_StudentID",
                table: "diary",
                column: "StudentID",
                principalTable: "Students",
                principalColumn: "StudentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mood_Students_StudentID",
                table: "mood",
                column: "StudentID",
                principalTable: "Students",
                principalColumn: "StudentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sleep_Students_StudentID",
                table: "sleep",
                column: "StudentID",
                principalTable: "Students",
                principalColumn: "StudentID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_diary_Students_StudentID",
                table: "diary");

            migrationBuilder.DropForeignKey(
                name: "FK_mood_Students_StudentID",
                table: "mood");

            migrationBuilder.DropForeignKey(
                name: "FK_sleep_Students_StudentID",
                table: "sleep");

            migrationBuilder.DropIndex(
                name: "IX_sleep_StudentID",
                table: "sleep");

            migrationBuilder.DropIndex(
                name: "IX_mood_StudentID",
                table: "mood");

            migrationBuilder.DropIndex(
                name: "IX_diary_StudentID",
                table: "diary");
        }
    }
}
