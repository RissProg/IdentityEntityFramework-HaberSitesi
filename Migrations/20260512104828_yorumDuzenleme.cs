using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityOrnek.Migrations
{
    /// <inheritdoc />
    public partial class yorumDuzenleme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Onay",
                table: "Yorumlar",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Yorumlar_HaberId",
                table: "Yorumlar",
                column: "HaberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Yorumlar_Haberler_HaberId",
                table: "Yorumlar",
                column: "HaberId",
                principalTable: "Haberler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Yorumlar_Haberler_HaberId",
                table: "Yorumlar");

            migrationBuilder.DropIndex(
                name: "IX_Yorumlar_HaberId",
                table: "Yorumlar");

            migrationBuilder.DropColumn(
                name: "Onay",
                table: "Yorumlar");
        }
    }
}
