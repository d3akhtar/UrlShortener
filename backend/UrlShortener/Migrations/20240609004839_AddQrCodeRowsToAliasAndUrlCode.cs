using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Migrations
{
    /// <inheritdoc />
    public partial class AddQrCodeRowsToAliasAndUrlCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UrlCodes",
                keyColumn: "Code",
                keyValue: "00000000");

            migrationBuilder.AddColumn<string>(
                name: "AsciiQrCodeRepresentation",
                table: "UrlCodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PngQrCodeImage",
                table: "UrlCodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SvgQrCodeImage",
                table: "UrlCodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AsciiQrCodeRepresentation",
                table: "Aliases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PngQrCodeImage",
                table: "Aliases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SvgQrCodeImage",
                table: "Aliases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AsciiQrCodeRepresentation",
                table: "UrlCodes");

            migrationBuilder.DropColumn(
                name: "PngQrCodeImage",
                table: "UrlCodes");

            migrationBuilder.DropColumn(
                name: "SvgQrCodeImage",
                table: "UrlCodes");

            migrationBuilder.DropColumn(
                name: "AsciiQrCodeRepresentation",
                table: "Aliases");

            migrationBuilder.DropColumn(
                name: "PngQrCodeImage",
                table: "Aliases");

            migrationBuilder.DropColumn(
                name: "SvgQrCodeImage",
                table: "Aliases");

            migrationBuilder.InsertData(
                table: "UrlCodes",
                columns: new[] { "Code", "Url" },
                values: new object[] { "00000000", "https://www.youtube.com/@opensand8228/videos" });
        }
    }
}
