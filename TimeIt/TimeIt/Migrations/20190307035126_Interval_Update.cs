using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeIt.Migrations
{
    public partial class Interval_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Intervals_TimerID_Position",
                table: "Intervals",
                columns: new[] { "TimerID", "Position" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Intervals_TimerID_Position",
                table: "Intervals");
        }
    }
}
