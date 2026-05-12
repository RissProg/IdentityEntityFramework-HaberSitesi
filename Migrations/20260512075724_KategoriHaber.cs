using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityOrnek.Migrations
{
    /// <inheritdoc />
    public partial class KategoriHaber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Haberler_AspNetUsers_YazarId",
                table: "Haberler");

            migrationBuilder.AddColumn<string>(
                name: "Kategori",
                table: "Haberler",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Haberler_AspNetUsers_YazarId",
                table: "Haberler",
                column: "YazarId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Haberler_AspNetUsers_YazarId",
                table: "Haberler");

            migrationBuilder.DropColumn(
                name: "Kategori",
                table: "Haberler");

            migrationBuilder.AddForeignKey(
                name: "FK_Haberler_AspNetUsers_YazarId",
                table: "Haberler",
                column: "YazarId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
