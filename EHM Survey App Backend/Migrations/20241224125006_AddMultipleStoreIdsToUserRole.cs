using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EHM_Survey_App.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleStoreIdsToUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoreIds",
                table: "UserRole",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreIds",
                table: "UserRole");
        }
    }
}
