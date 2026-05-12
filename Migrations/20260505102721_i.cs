using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityOrnek.Migrations
{
    /// <inheritdoc />
    public partial class i : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Haberler_AspNetUsers_YazarId",
                table: "Haberler");

            migrationBuilder.AlterColumn<string>(
                name: "YazarId",
                table: "Haberler",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "GorselUrl",
                table: "Haberler",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Haberler_AspNetUsers_YazarId",
                table: "Haberler",
                column: "YazarId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Haberler_AspNetUsers_YazarId",
                table: "Haberler");

            migrationBuilder.DropColumn(
                name: "GorselUrl",
                table: "Haberler");

            migrationBuilder.AlterColumn<string>(
                name: "YazarId",
                table: "Haberler",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Haberler_AspNetUsers_YazarId",
                table: "Haberler",
                column: "YazarId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
