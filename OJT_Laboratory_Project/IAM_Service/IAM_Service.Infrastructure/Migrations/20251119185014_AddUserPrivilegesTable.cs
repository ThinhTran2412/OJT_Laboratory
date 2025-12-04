using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAM_Service.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPrivilegesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPrivileges",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PrivilegeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPrivileges", x => new { x.UserId, x.PrivilegeId });
                    table.ForeignKey(
                        name: "FK_UserPrivileges_Privileges_PrivilegeId",
                        column: x => x.PrivilegeId,
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPrivileges_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPrivileges_PrivilegeId",
                table: "UserPrivileges",
                column: "PrivilegeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPrivileges");
        }
    }
}
