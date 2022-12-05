using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class fixOnDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntriesData_Entries_EntryId",
                table: "EntriesData");

            migrationBuilder.AddForeignKey(
                name: "FK_EntriesData_Entries_EntryId",
                table: "EntriesData",
                column: "EntryId",
                principalTable: "Entries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntriesData_Entries_EntryId",
                table: "EntriesData");

            migrationBuilder.AddForeignKey(
                name: "FK_EntriesData_Entries_EntryId",
                table: "EntriesData",
                column: "EntryId",
                principalTable: "Entries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
