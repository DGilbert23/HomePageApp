using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomePageApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBillModelTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reoccurring = table.Column<bool>(type: "bit", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDue = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextDue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastPaid = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimatedAmountDue = table.Column<double>(type: "float", nullable: false),
                    PaymentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bills");
        }
    }
}
