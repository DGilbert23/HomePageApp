using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomePageApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModelBillAddAutoDraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastPaid",
                table: "Bills",
                newName: "LastPaidOrSeen");

            migrationBuilder.AddColumn<bool>(
                name: "AutoDraft",
                table: "Bills",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoDraft",
                table: "Bills");

            migrationBuilder.RenameColumn(
                name: "LastPaidOrSeen",
                table: "Bills",
                newName: "LastPaid");
        }
    }
}
