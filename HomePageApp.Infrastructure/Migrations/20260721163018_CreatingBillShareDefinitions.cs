using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomePageApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreatingBillShareDefinitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillShareDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    ShareWithId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillShareDefinitions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillShareDefinitions_OwnerId_ShareWithId",
                table: "BillShareDefinitions",
                columns: new[] { "OwnerId", "ShareWithId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillShareDefinitions");
        }
    }
}
