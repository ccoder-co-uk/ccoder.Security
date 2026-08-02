using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cCoder.Security.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApiMetadataPrivilege : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                sql: """
                    UPDATE [Roles]
                    SET [Privs] = CONCAT([Privs],
                        CASE WHEN NULLIF([Privs], '') IS NULL THEN '' ELSE ',' END,
                        'api_metadata_read')
                    WHERE CONCAT(',', [Privs], ',') LIKE '%,tenant_admin,%'
                      AND CONCAT(',', [Privs], ',') NOT LIKE '%,api_metadata_read,%';
                    """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                sql: """
                    UPDATE [Roles]
                    SET [Privs] = (
                        SELECT STRING_AGG([value], ',')
                        FROM STRING_SPLIT([Privs], ',')
                        WHERE [value] <> 'api_metadata_read')
                    WHERE CONCAT(',', [Privs], ',') LIKE '%,api_metadata_read,%';
                    """);
        }
    }
}