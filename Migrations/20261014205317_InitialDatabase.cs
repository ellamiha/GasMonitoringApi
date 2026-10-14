using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GasMonitoringApi.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Login = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    NormalizedLogin = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.CheckConstraint("CK_Users_Role", "Role IN ('User', 'Admin')");
                });

            migrationBuilder.CreateTable(
                name: "Wells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MaxPressure = table.Column<double>(type: "REAL", nullable: false),
                    PlannedFlow = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wells", x => x.Id);
                    table.CheckConstraint("CK_Wells_MaxPressure", "MaxPressure BETWEEN 0.01 AND 1000");
                    table.CheckConstraint("CK_Wells_Name", "length(trim(Name)) BETWEEN 1 AND 100");
                    table.CheckConstraint("CK_Wells_PlannedFlow", "PlannedFlow BETWEEN 0.01 AND 1000000");
                });

            migrationBuilder.CreateTable(
                name: "Measurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WellId = table.Column<int>(type: "INTEGER", nullable: false),
                    RecordedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    Pressure = table.Column<double>(type: "REAL", nullable: false),
                    Flow = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measurements", x => x.Id);
                    table.CheckConstraint("CK_Measurements_Flow", "Flow BETWEEN 0 AND 1000000");
                    table.CheckConstraint("CK_Measurements_Pressure", "Pressure BETWEEN 0 AND 1000");
                    table.ForeignKey(
                        name: "FK_Measurements_Wells_WellId",
                        column: x => x.WellId,
                        principalTable: "Wells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_WellId_RecordedAt",
                table: "Measurements",
                columns: new[] { "WellId", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedLogin",
                table: "Users",
                column: "NormalizedLogin",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Measurements");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Wells");
        }
    }
}
