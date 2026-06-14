using NetBarcode;
using QRCoder.Core.Generators;
using QRCoder.Core.Models;
using QRCoder.Core.Renderers;

namespace coer91.NET.Files
{
    public static class CodeGenerator
    {
        public static string BarcodeBase64(object value, NetBarcode.Type barcodeType = NetBarcode.Type.Code128)
        {
            try
            {
                value ??= string.Empty;
                value = $"{value}".RemoveAccents().Replace(",", "").Replace("$", "").CleanUpBlanks();

                Barcode barcode = new($"{value}", barcodeType);
                byte[] bytes = barcode.GetByteArray();
                string base64 = Convert.ToBase64String(bytes);
                return $"data:image/png;base64,{base64}";
            }

            catch
            {
                return $"{value}";
            }
        }


        public static string BarcodeHTML(object value, string width = null, string height = null, string showCaption = null, string captionAlign = null, string caption = null)
        {
            string base64 = BarcodeBase64(value);
            int _width = int.TryParse(width, out int widthValue) ? widthValue : 100;
            int _height = int.TryParse(height, out int heightValue) ? heightValue : 30;
            bool _showCaption = $"{showCaption}".Equals("true", StringComparison.OrdinalIgnoreCase);
            string _captionAlign = string.IsNullOrWhiteSpace(captionAlign) ? "left" : captionAlign;
            string _caption = string.IsNullOrWhiteSpace(caption) ? $"{value}" : caption;


            string stylesWidth = $"width:{_width}px;   min-width:50px;";
            string stylesHeight = $"height:{_height}px; min-height:25px;";
            string stylesPM = $"padding:0px; margin:0px;";

            string stylesAlign = _captionAlign switch
            {
                "left" => "text-align:left;",
                "center" => "text-align:center;",
                "right" => "text-align:right;",
                _ => "text-align:left;",
            };

            _caption = !_showCaption ? string.Empty : @$"
                <figcaption style='{stylesWidth} {stylesAlign} word-break: keep-all;'>{_caption}</figcaption>
            ";

            return string.Join(' ', @$"
                <figure style='{stylesWidth} {stylesHeight} {stylesPM} display:inline-block;'>
                    <img src='{base64}' style='{stylesWidth} {stylesHeight} {stylesPM}' />
                    {_caption}
                </figure>
            "
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .Replace("\t", string.Empty)
            .Replace("\"", "\'")
            .Split(' ')
            .Where(x => x.Length > 0)
            .ToArray());
        }


        public static string QRBase64(object value)
        {
            try
            {
                value ??= string.Empty;
                value = $"{value}".RemoveAccents().Replace(",", "").Replace("$", "").CleanUpBlanks();

                using QRCodeGenerator generator = new();
                using QRCodeData data = generator.CreateQrCode($"{value}", QRCodeGenerator.ECCLevel.M, true);
                using PngByteQRCode png = new(data);

                byte[] bytes = png.GetGraphic(10);
                png.Dispose();
                data.Dispose();
                generator.Dispose();

                string base64 = Convert.ToBase64String(bytes);
                return $"data:image/png;base64,{base64}";
            }

            catch
            {
                return $"{value}";
            }
        }


        public static string QRHTML(object value, string size = null, string showCaption = null, string captionAlign = null, string caption = null)
        {
            string base64 = QRBase64(value);
            int _size = int.TryParse(size, out int sizeValue) ? sizeValue : 25;
            bool _showCaption = $"{showCaption}".Equals("true", StringComparison.OrdinalIgnoreCase);
            string _captionAlign = string.IsNullOrWhiteSpace(captionAlign) ? "left" : captionAlign;
            string _caption = string.IsNullOrWhiteSpace(caption) ? $"{value}" : caption;

            string stylesWidth = $"width:{_size}px;  min-width:25px;";
            string stylesHeight = $"height:{_size}px; min-height:25px;";
            string stylesPM = $"padding:0px; margin:0px;";

            string stylesAlign = captionAlign switch
            {
                "left" => "text-align:left;",
                "center" => "text-align:center;",
                "right" => "text-align:right;",
                _ => "text-align:center;",
            };

            _caption = !_showCaption ? string.Empty : @$"
                <figcaption style='{stylesWidth} {stylesAlign} word-break: keep-all;'>{_caption}</figcaption>
            ";

            return string.Join(' ', @$"
                <figure style='{stylesWidth} {stylesHeight} {stylesPM} display:inline-block;'>
                    <img src='{base64}' style='{stylesWidth} {stylesHeight} {stylesPM}' />
                    {_caption}
                </figure>
            "
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .Replace("\t", string.Empty)
            .Replace("\"", "\'")
            .Split(' ')
            .Where(x => x.Length > 0)
            .ToArray());
        }
    }
} 