using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cCoder.Security.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class HashTokenSecrets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecretHash",
                table: "Tokens",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecretHash",
                table: "Tokens");
        }
    }
}
