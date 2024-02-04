using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskGauge.Entity.Migrations
{
    /// <inheritdoc />
    public partial class SecurityQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityQuestion",
                table: "User");

            migrationBuilder.AddColumn<int>(
                name: "SecurityQuestionId",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SecurityQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityQuestion", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_SecurityQuestionId",
                table: "User",
                column: "SecurityQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_SecurityQuestion_SecurityQuestionId",
                table: "User",
                column: "SecurityQuestionId",
                principalTable: "SecurityQuestion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_SecurityQuestion_SecurityQuestionId",
                table: "User");

            migrationBuilder.DropTable(
                name: "SecurityQuestion");

            migrationBuilder.DropIndex(
                name: "IX_User_SecurityQuestionId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "SecurityQuestionId",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "SecurityQuestion",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
