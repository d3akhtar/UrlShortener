using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Migrations
{
    /// <inheritdoc />
    public partial class JoinTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUserAlias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AliasCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserAlias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUserAlias_Aliases_AliasCode",
                        column: x => x.AliasCode,
                        principalTable: "Aliases",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserAlias_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserUrlCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrlCodeCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserUrlCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUserUrlCodes_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserUrlCodes_UrlCodes_UrlCodeCode",
                        column: x => x.UrlCodeCode,
                        principalTable: "UrlCodes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserAlias_AliasCode",
                table: "ApplicationUserAlias",
                column: "AliasCode");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserAlias_ApplicationUserId",
                table: "ApplicationUserAlias",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserUrlCodes_ApplicationUserId",
                table: "ApplicationUserUrlCodes",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserUrlCodes_UrlCodeCode",
                table: "ApplicationUserUrlCodes",
                column: "UrlCodeCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserAlias");

            migrationBuilder.DropTable(
                name: "ApplicationUserUrlCodes");

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
    }
}
