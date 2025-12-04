using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Monitoring_Service.Infastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "monitoring_service");

            migrationBuilder.CreateTable(
                name: "RawResults",
                schema: "monitoring_service",
                columns: table => new
                {
                    TestResultId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    TestCode = table.Column<string>(type: "text", nullable: false),
                    Parameter = table.Column<string>(type: "text", nullable: false),
                    ValueNumeric = table.Column<double>(type: "double precision", nullable: true),
                    ValueText = table.Column<string>(type: "text", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    ReferenceRange = table.Column<string>(type: "text", nullable: false),
                    Instrument = table.Column<string>(type: "text", nullable: false),
                    PerformedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResultStatus = table.Column<string>(type: "text", nullable: false),
                    PerformedByUserId = table.Column<int>(type: "integer", nullable: true),
                    ReviewedByUserId = table.Column<int>(type: "integer", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawResults", x => x.TestResultId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RawResults",
                schema: "monitoring_service");
        }
    }
}
