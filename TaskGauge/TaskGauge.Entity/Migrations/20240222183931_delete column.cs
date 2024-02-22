using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskGauge.Entity.Migrations
{
    /// <inheritdoc />
    public partial class deletecolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_User_ModifiedBy",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_ModifiedBy",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Task");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "Task",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Task_ModifiedBy",
                table: "Task",
                column: "ModifiedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_User_ModifiedBy",
                table: "Task",
                column: "ModifiedBy",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
