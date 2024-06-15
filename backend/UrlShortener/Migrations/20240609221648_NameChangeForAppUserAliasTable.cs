using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Migrations
{
    /// <inheritdoc />
    public partial class NameChangeForAppUserAliasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserAlias_Aliases_AliasCode",
                table: "ApplicationUserAlias");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserAlias_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserAlias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserAlias",
                table: "ApplicationUserAlias");

            migrationBuilder.RenameTable(
                name: "ApplicationUserAlias",
                newName: "ApplicationUserAliases");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserAlias_ApplicationUserId",
                table: "ApplicationUserAliases",
                newName: "IX_ApplicationUserAliases_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserAlias_AliasCode",
                table: "ApplicationUserAliases",
                newName: "IX_ApplicationUserAliases_AliasCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserAliases",
                table: "ApplicationUserAliases",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserAliases_Aliases_AliasCode",
                table: "ApplicationUserAliases",
                column: "AliasCode",
                principalTable: "Aliases",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserAliases_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserAliases",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserAliases_Aliases_AliasCode",
                table: "ApplicationUserAliases");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserAliases_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserAliases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserAliases",
                table: "ApplicationUserAliases");

            migrationBuilder.RenameTable(
                name: "ApplicationUserAliases",
                newName: "ApplicationUserAlias");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserAliases_ApplicationUserId",
                table: "ApplicationUserAlias",
                newName: "IX_ApplicationUserAlias_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserAliases_AliasCode",
                table: "ApplicationUserAlias",
                newName: "IX_ApplicationUserAlias_AliasCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserAlias",
                table: "ApplicationUserAlias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserAlias_Aliases_AliasCode",
                table: "ApplicationUserAlias",
                column: "AliasCode",
                principalTable: "Aliases",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserAlias_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserAlias",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
