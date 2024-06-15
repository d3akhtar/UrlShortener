using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Migrations
{
    /// <inheritdoc />
    public partial class AddingUserList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AliasApplicationUser",
                columns: table => new
                {
                    ApplicationUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    aliasesCode = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AliasApplicationUser", x => new { x.ApplicationUsersId, x.aliasesCode });
                    table.ForeignKey(
                        name: "FK_AliasApplicationUser_Aliases_aliasesCode",
                        column: x => x.aliasesCode,
                        principalTable: "Aliases",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AliasApplicationUser_AspNetUsers_ApplicationUsersId",
                        column: x => x.ApplicationUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserUrlCode",
                columns: table => new
                {
                    ApplicationUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    urlCodesCode = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserUrlCode", x => new { x.ApplicationUsersId, x.urlCodesCode });
                    table.ForeignKey(
                        name: "FK_ApplicationUserUrlCode_AspNetUsers_ApplicationUsersId",
                        column: x => x.ApplicationUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserUrlCode_UrlCodes_urlCodesCode",
                        column: x => x.urlCodesCode,
                        principalTable: "UrlCodes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AliasApplicationUser_aliasesCode",
                table: "AliasApplicationUser",
                column: "aliasesCode");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserUrlCode_urlCodesCode",
                table: "ApplicationUserUrlCode",
                column: "urlCodesCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AliasApplicationUser");

            migrationBuilder.DropTable(
                name: "ApplicationUserUrlCode");
        }
    }
}
