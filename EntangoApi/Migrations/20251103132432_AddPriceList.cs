using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EntangoApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PriceLists",
                columns: table => new
                {
                    PriceListId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PriceListDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PriceListAuthor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PriceListVersion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ProductId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProductDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProductExtendedDescription = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ProductUM = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ProductPrice = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ProductNetPrice = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ProductLabourPerc = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    ProductSafetyPerc = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    ProductMaterialPerc = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    ProductEquipmentPerc = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    ProductHirePerc = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    ProductGeneralExpensePerc = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    ProductBusinessProfitPerc = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    ProductClassification = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceLists", x => x.PriceListId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceLists");
        }
    }
}
