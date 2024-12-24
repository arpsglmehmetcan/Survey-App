using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EHM_Survey_App.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStoreIdFromUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "UserRole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "UserRole",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
