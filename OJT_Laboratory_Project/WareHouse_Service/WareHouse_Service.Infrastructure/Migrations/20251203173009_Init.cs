using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WareHouse_Service.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "warehouse_service");

            migrationBuilder.CreateTable(
                name: "Instruments",
                schema: "warehouse_service",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InstrumentName = table.Column<string>(type: "text", nullable: false),
                    SupportTestType = table.Column<string[]>(type: "text[]", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Load = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruments", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Instruments",
                schema: "warehouse_service");
        }
    }
}
