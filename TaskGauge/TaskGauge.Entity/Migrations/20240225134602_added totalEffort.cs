using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskGauge.Entity.Migrations
{
    /// <inheritdoc />
    public partial class addedtotalEffort : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TotalEffort",
                table: "RoomTaskInformation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalEffort",
                table: "RoomTaskInformation");
        }
    }
}
