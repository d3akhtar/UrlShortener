using QRCoder;

namespace UrlShortener.Utilities
{
    public static class Helpers
    {
        public static byte[] GetPngQrCodeImage(string code)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = qrCode.GetGraphic(20);
            return qrCodeImage;
        }

        public static string GetSvgQrCodeImage(string code)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
            SvgQRCode qrCode = new SvgQRCode(qrCodeData);
            string qrCodeAsSvg = qrCode.GetGraphic(20);
            return qrCodeAsSvg;
        }

        public static string GetAsciiQrCodeRepresentation(string code)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
            AsciiQRCode qrCode = new AsciiQRCode(qrCodeData);
            string qrCodeAsAsciiArt = qrCode.GetGraphic(1);
            return qrCodeAsAsciiArt;
        }

        public static string GetBaseUrlWithRequest(HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host}:{request.Path}/";
        }
    }
}
