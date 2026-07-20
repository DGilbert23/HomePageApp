using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomePageApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBills_UserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Bills",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bills");
        }
    }
}
