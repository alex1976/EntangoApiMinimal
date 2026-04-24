using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EntangoApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentStores",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentDescription = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DocumentCategory = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DocumentExtension = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DocumentSize = table.Column<long>(type: "bigint", nullable: false),
                    DocumentAttachment = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentStores", x => x.DocumentId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentStores");
        }
    }
}
