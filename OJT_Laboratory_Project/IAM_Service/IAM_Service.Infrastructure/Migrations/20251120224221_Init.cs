using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IAM_Service.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "iam_service");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                schema: "iam_service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserEmail = table.Column<string>(type: "text", nullable: false),
                    EntityName = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Changes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Privileges",
                schema: "iam_service",
                columns: table => new
                {
                    PrivilegeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Privileges", x => x.PrivilegeId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "iam_service",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "RolePrivileges",
                schema: "iam_service",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    PrivilegeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePrivileges", x => new { x.RoleId, x.PrivilegeId });
                    table.ForeignKey(
                        name: "FK_RolePrivileges_Privileges_PrivilegeId",
                        column: x => x.PrivilegeId,
                        principalSchema: "iam_service",
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePrivileges_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "iam_service",
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "iam_service",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    IdentifyNumber = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                    LockoutEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastFailedLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "iam_service",
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResets",
                schema: "iam_service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResets_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "iam_service",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "iam_service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "iam_service",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPrivileges",
                schema: "iam_service",
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
                        principalSchema: "iam_service",
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPrivileges_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "iam_service",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "iam_service",
                table: "Privileges",
                columns: new[] { "PrivilegeId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Only have right to view patient test orders and patient test order results.", "READ_ONLY" },
                    { 2, "Have right to create a new patient test order", "CREATE_TEST_ORDER" },
                    { 3, "Have right to modify information a patient test order.", "MODIFY_TEST_ORDER" },
                    { 4, "Have right to delete an exist test order.", "DELETE_TEST_ORDER" },
                    { 5, "Have right to review, modify test result of test order", "REVIEW_TEST_ORDER" },
                    { 6, "Have right to add a new comment for test result", "ADD_COMMENT" },
                    { 7, "Have right to modify a comment.", "MODIFY_COMMENT" },
                    { 8, "Have right to delete a comment.", "DELETE_COMMENT" },
                    { 9, "Have right to view, add, modify and delete configurations.", "VIEW_CONFIGURATION" },
                    { 10, "Have right to add a new configuration.", "CREATE_CONFIGURATION" },
                    { 11, "Have right to modify a configuration.", "MODIFY_CONFIGURATION" },
                    { 12, "Have right to delete a configuration.", "DELETE_CONFIGURATION" },
                    { 13, "Have right to view all user profiles", "VIEW_USER" },
                    { 14, "Have right to create a new user.", "CREATE_USER" },
                    { 15, "Have right to modify a user.", "MODIFY_USER" },
                    { 16, "Have right to delete a user.", "DELETE_USER" },
                    { 17, "Have right to lock or unlock a user.", "LOCK_UNLOCK_USER" },
                    { 18, "Have right to view all role privileges.", "VIEW_ROLE" },
                    { 19, "Have right to create a new custom role.", "CREATE_ROLE" },
                    { 20, "Have right to modify privileges of custom role.", "UPDATE_ROLE" },
                    { 21, "Have right to delete a custom role.", "DELETE_ROLE" },
                    { 22, "Have right to view event logs", "VIEW_EVENT_LOGS" },
                    { 23, "Have right to add new reagents.", "ADD_REAGENTS" },
                    { 24, "Have right to modify reagent information.", "MODIFY_REAGENTS" },
                    { 25, "Have right to delete a regents", "DELETE_REAGENTS" },
                    { 26, "Have right to add a new instrument into system management", "ADD_INSTRUMENT" },
                    { 27, "Have right to view all instrument and check instrument status.", "VIEW_INSTRUMENT" },
                    { 28, "Have right to activate or deactivate instrument", "ACTIVATE_DEACTIVATE_INSTRUMENT" },
                    { 29, "Have right to execute a blood testing", "EXECUTE_BLOOD_TESTING" }
                });

            migrationBuilder.InsertData(
                schema: "iam_service",
                table: "Roles",
                columns: new[] { "RoleId", "Code", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "", "Full administrative access.", "ADMIN" },
                    { 2, "", "Manager role for the lab.", "LAB_MANAGER" },
                    { 3, "", "Standard lab user.", "LAB_USER" },
                    { 4, "", "Service/Maintenance role.", "SERVICE" },
                    { 5, "", "Default minimal access role.", "CUSTOM_ROLE_DEFAULT" }
                });

            migrationBuilder.InsertData(
                schema: "iam_service",
                table: "RolePrivileges",
                columns: new[] { "PrivilegeId", "RoleId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 5, 1 },
                    { 6, 1 },
                    { 7, 1 },
                    { 8, 1 },
                    { 9, 1 },
                    { 10, 1 },
                    { 11, 1 },
                    { 12, 1 },
                    { 13, 1 },
                    { 14, 1 },
                    { 15, 1 },
                    { 16, 1 },
                    { 17, 1 },
                    { 18, 1 },
                    { 19, 1 },
                    { 20, 1 },
                    { 21, 1 },
                    { 22, 1 },
                    { 23, 1 },
                    { 24, 1 },
                    { 25, 1 },
                    { 26, 1 },
                    { 27, 1 },
                    { 28, 1 },
                    { 29, 1 },
                    { 1, 2 },
                    { 13, 2 },
                    { 14, 2 },
                    { 15, 2 },
                    { 16, 2 },
                    { 17, 2 },
                    { 18, 2 },
                    { 19, 2 },
                    { 20, 2 },
                    { 21, 2 },
                    { 22, 2 },
                    { 25, 2 },
                    { 26, 2 },
                    { 27, 2 },
                    { 28, 2 },
                    { 2, 3 },
                    { 3, 3 },
                    { 4, 3 },
                    { 5, 3 },
                    { 6, 3 },
                    { 7, 3 },
                    { 8, 3 },
                    { 22, 3 },
                    { 23, 3 },
                    { 24, 3 },
                    { 25, 3 },
                    { 26, 3 },
                    { 27, 3 },
                    { 28, 3 },
                    { 29, 3 },
                    { 9, 4 },
                    { 10, 4 },
                    { 11, 4 },
                    { 12, 4 },
                    { 22, 4 },
                    { 23, 4 },
                    { 24, 4 },
                    { 25, 4 },
                    { 26, 4 },
                    { 27, 4 },
                    { 28, 4 },
                    { 29, 4 },
                    { 1, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResets_UserId",
                schema: "iam_service",
                table: "PasswordResets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                schema: "iam_service",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "iam_service",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePrivileges_PrivilegeId",
                schema: "iam_service",
                table: "RolePrivileges",
                column: "PrivilegeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrivileges_PrivilegeId",
                schema: "iam_service",
                table: "UserPrivileges",
                column: "PrivilegeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                schema: "iam_service",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs",
                schema: "iam_service");

            migrationBuilder.DropTable(
                name: "PasswordResets",
                schema: "iam_service");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "iam_service");

            migrationBuilder.DropTable(
                name: "RolePrivileges",
                schema: "iam_service");

            migrationBuilder.DropTable(
                name: "UserPrivileges",
                schema: "iam_service");

            migrationBuilder.DropTable(
                name: "Privileges",
                schema: "iam_service");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "iam_service");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "iam_service");
        }
    }
}
