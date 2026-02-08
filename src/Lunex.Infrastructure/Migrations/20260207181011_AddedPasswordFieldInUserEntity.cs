using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunex.Infrastructure.src.Lunex.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPasswordFieldInUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "password_hash",
                schema: "lunex",
                table: "users",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "password_salt",
                schema: "lunex",
                table: "users",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password_hash",
                schema: "lunex",
                table: "users");

            migrationBuilder.DropColumn(
                name: "password_salt",
                schema: "lunex",
                table: "users");
        }
    }
}
