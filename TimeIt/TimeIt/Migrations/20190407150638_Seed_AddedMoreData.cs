using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeIt.Migrations
{
    public partial class Seed_AddedMoreData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Timers",
                columns: new[] { "TimerID", "Name", "Repetitions" },
                values: new object[] { 2, "Estados sin luz", 2 });

            migrationBuilder.InsertData(
                table: "Intervals",
                columns: new[] { "IntervalID", "Color", "Duration", "Name", "Position", "TimerID" },
                values: new object[] { 4, "#FFFF0000", 180, "Distrito Capital", 1, 2 });

            migrationBuilder.InsertData(
                table: "Intervals",
                columns: new[] { "IntervalID", "Color", "Duration", "Name", "Position", "TimerID" },
                values: new object[] { 5, "#FFFFFFFF", 340, "Zulia", 2, 2 });

            migrationBuilder.InsertData(
                table: "Intervals",
                columns: new[] { "IntervalID", "Color", "Duration", "Name", "Position", "TimerID" },
                values: new object[] { 6, "#FFFFFF00", 300, "Anzoategui", 3, 2 });

            migrationBuilder.InsertData(
                table: "Intervals",
                columns: new[] { "IntervalID", "Color", "Duration", "Name", "Position", "TimerID" },
                values: new object[] { 7, "#FFEE82EE", 240, "Nueva Esparta", 4, 2 });

            migrationBuilder.InsertData(
                table: "Intervals",
                columns: new[] { "IntervalID", "Color", "Duration", "Name", "Position", "TimerID" },
                values: new object[] { 8, "#FFF5FFFA", 290, "Guarico", 5, 2 });

            migrationBuilder.InsertData(
                table: "Intervals",
                columns: new[] { "IntervalID", "Color", "Duration", "Name", "Position", "TimerID" },
                values: new object[] { 9, "#FFF0F8FF", 280, "Trujillo", 6, 2 });

            migrationBuilder.InsertData(
                table: "Intervals",
                columns: new[] { "IntervalID", "Color", "Duration", "Name", "Position", "TimerID" },
                values: new object[] { 10, "#FFCD5C5C", 220, "Tachira", 7, 2 });

            migrationBuilder.InsertData(
                table: "Intervals",
                columns: new[] { "IntervalID", "Color", "Duration", "Name", "Position", "TimerID" },
                values: new object[] { 11, "#FF008000", 180, "Aragua", 8, 2 });

            migrationBuilder.InsertData(
                table: "Intervals",
                columns: new[] { "IntervalID", "Color", "Duration", "Name", "Position", "TimerID" },
                values: new object[] { 12, "#FFADFF2F", 150, "Bolivar", 9, 2 });

            migrationBuilder.InsertData(
                table: "Intervals",
                columns: new[] { "IntervalID", "Color", "Duration", "Name", "Position", "TimerID" },
                values: new object[] { 13, "#FF7CFC00", 345, "Aragua", 10, 2 });

            migrationBuilder.InsertData(
                table: "Intervals",
                columns: new[] { "IntervalID", "Color", "Duration", "Name", "Position", "TimerID" },
                values: new object[] { 14, "#FFF0E68C", 235, "Delta Amacuro", 11, 2 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Intervals",
                keyColumn: "IntervalID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Intervals",
                keyColumn: "IntervalID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Intervals",
                keyColumn: "IntervalID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Intervals",
                keyColumn: "IntervalID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Intervals",
                keyColumn: "IntervalID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Intervals",
                keyColumn: "IntervalID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Intervals",
                keyColumn: "IntervalID",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Intervals",
                keyColumn: "IntervalID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Intervals",
                keyColumn: "IntervalID",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Intervals",
                keyColumn: "IntervalID",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Intervals",
                keyColumn: "IntervalID",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Timers",
                keyColumn: "TimerID",
                keyValue: 2);
        }
    }
}
