using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeIt.Migrations
{
    public partial class IntervalNavigationToTimer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Timers",
                columns: table => new
                {
                    TimerID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Repetitions = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timers", x => x.TimerID);
                });

            migrationBuilder.CreateTable(
                name: "Intervals",
                columns: table => new
                {
                    IntervalID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Duration = table.Column<int>(nullable: false),
                    Color = table.Column<string>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    TimerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intervals", x => x.IntervalID);
                    table.ForeignKey(
                        name: "FK_Intervals_Timers_TimerID",
                        column: x => x.TimerID,
                        principalTable: "Timers",
                        principalColumn: "TimerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Intervals_TimerID",
                table: "Intervals",
                column: "TimerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Intervals");

            migrationBuilder.DropTable(
                name: "Timers");
        }
    }
}
