using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeIt.Migrations
{
    public partial class SeedDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Timers",
                columns: new[] { "TimerID", "Name", "Repetitions" },
                values: new object[] { 1, "Default", 2 });

            migrationBuilder.InsertData(
                table: "Intervals",
                columns: new[] { "IntervalID", "Color", "Duration", "Name", "Position", "TimerID" },
                values: new object[] { 1, "#FFFFFF00", 180, "Workout", 1, 1 });

            migrationBuilder.InsertData(
                table: "Intervals",
                columns: new[] { "IntervalID", "Color", "Duration", "Name", "Position", "TimerID" },
                values: new object[] { 2, "#FF0000FF", 40, "Rest", 2, 1 });

            migrationBuilder.InsertData(
                table: "Intervals",
                columns: new[] { "IntervalID", "Color", "Duration", "Name", "Position", "TimerID" },
                values: new object[] { 3, "#FFFF0000", 90, "Run", 3, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Intervals",
                keyColumn: "IntervalID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Intervals",
                keyColumn: "IntervalID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Intervals",
                keyColumn: "IntervalID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Timers",
                keyColumn: "TimerID",
                keyValue: 1);
        }
    }
}
