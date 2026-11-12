using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunex.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "lunex");

            migrationBuilder.CreateTable(
                name: "users",
                schema: "lunex",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    password_hash = table.Column<byte[]>(type: "bytea", nullable: false),
                    password_salt = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "members",
                schema: "lunex",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    gender = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    city = table.Column<string>(type: "text", nullable: false),
                    country = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_active_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_members_users_id",
                        column: x => x.id,
                        principalSchema: "lunex",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "photos",
                schema: "lunex",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    url = table.Column<string>(type: "text", nullable: false),
                    public_id = table.Column<string>(type: "text", nullable: true),
                    member_id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_photos", x => x.id);
                    table.ForeignKey(
                        name: "fk_photos_members_member_id",
                        column: x => x.member_id,
                        principalSchema: "lunex",
                        principalTable: "members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_photos_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "lunex",
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_photos_member_id",
                schema: "lunex",
                table: "photos",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "ix_photos_user_id",
                schema: "lunex",
                table: "photos",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "photos",
                schema: "lunex");

            migrationBuilder.DropTable(
                name: "members",
                schema: "lunex");

            migrationBuilder.DropTable(
                name: "users",
                schema: "lunex");
        }
    }
}
