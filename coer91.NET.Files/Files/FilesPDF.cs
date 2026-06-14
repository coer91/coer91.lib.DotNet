using iText.Html2pdf;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Event;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties; 
using Newtonsoft.Json;
using System.Text;
using System.Xml;
using Rectangle = iText.Kernel.Geom.Rectangle;

namespace coer91.NET.Files
{
    internal static class FilesPDF
    {
        private static readonly int marginDefaltHTML = 47;
        private static readonly int marginDefaltCANVAS = 35;


        #region GeneratePDF
        public static FileDTO GenerateDocument(DocumentPdf documentPdf, object data = null, int offset = 0)
        {
            (string header, string body) = GenerateHtml(documentPdf, data);

            using MemoryStream memoryStream = new();
            using PdfWriter pdfWriter = new(memoryStream);
            using PdfDocument pdfDocument = new(pdfWriter); 

            pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderHandler(documentPdf.Setup, header));
            Document document = HtmlConverter.ConvertToDocument(body, pdfDocument, new ConverterProperties());

            documentPdf.Setup.Footer = GetFooter(documentPdf.Setup, offset);
            pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterHandler(documentPdf.Setup)); 

            document.Close();
            pdfDocument.Close();
            pdfWriter.Close();
            memoryStream.Close();

            string content = CleanHTML(body
                .Replace("<main>", $"{header}<main>")
                .Replace("</style>", "main, header { max-width: 700px !important; margin: auto !important; }</style>")
            );

            return new FileDTO
            {
                Content = content, 
                File    = memoryStream
            };
        }


        private class HeaderHandler(DocumentPdfSetup _setup, string _html) : AbstractPdfDocumentEventHandler
        {
            protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event)
            {
                PdfDocumentEvent pdfEvent = (PdfDocumentEvent)@event;
                PdfPage pdfPage = pdfEvent.GetPage();
                PdfCanvas pdfCanvas = new(pdfPage);

                float x = marginDefaltCANVAS;
                float y = pdfPage.GetPageSize().GetTop() - marginDefaltCANVAS - _setup.HeaderHeight;
                float width = (float)(pdfPage.GetPageSize().GetWidth() - (marginDefaltCANVAS * 2) - 1.5);
                Canvas canvas = new(pdfCanvas, new Rectangle(x, y, width, _setup.HeaderHeight));

                _html = $"<header>{GenerateStyles(0, 0, 0, 0)} {_html}</header>";
                canvas.Add((IBlockElement)HtmlConverter.ConvertToElements(_html)[0]);
                canvas.Close();
                pdfCanvas.Release();
            }
        }


        private class FooterHandler(DocumentPdfSetup _setup) : AbstractPdfDocumentEventHandler
        {
            private int _pageNumber = 0;
            private int _totalPages = 0;

            protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event)
            {
                PdfDocumentEvent pdfEvent = (PdfDocumentEvent)@event;
                PdfDocument pdfDocument = pdfEvent.GetDocument();
                PdfPage pdfPage = pdfEvent.GetPage();
                PdfCanvas pdfCanvas = new(pdfPage.NewContentStreamBefore(), pdfPage.GetResources(), pdfDocument);
                _totalPages = pdfDocument.GetNumberOfPages();

                Cell cell_left = new Cell().Add(new Paragraph(_setup.Footer));
                cell_left.SetFontColor(ColorConstants.GRAY);
                cell_left.SetTextAlignment(TextAlignment.LEFT);
                cell_left.SetBorder(Border.NO_BORDER);
                cell_left.SetPaddingTop(5);

                Paragraph paragraph_right = new(_setup.ShowPageNumber ? $"Page {++_pageNumber} of {_totalPages}" : string.Empty);
                Cell cell_Right = new Cell().Add(paragraph_right);
                cell_Right.SetFontColor(ColorConstants.GRAY);
                cell_Right.SetTextAlignment(TextAlignment.RIGHT);
                cell_Right.SetBorder(Border.NO_BORDER);
                cell_Right.SetPaddingTop(5);

                Table table = new Table(2).UseAllAvailableWidth();
                table.SetBorderTop(new SolidBorder(ColorConstants.GRAY, 1));
                table.AddCell(cell_left);
                table.AddCell(cell_Right);

                float y = 0;
                float width = (float)(pdfPage.GetPageSize().GetWidth() - (marginDefaltCANVAS * 2) - 1.5);
                float height = marginDefaltCANVAS;
                new Canvas(pdfCanvas, new Rectangle(marginDefaltCANVAS, y, width, height)).Add(table).Close();
            }
        }


        private static string GetFooter(DocumentPdfSetup _setup, int offset = 0)
        {
            string footer = _setup.Footer;

            if (!string.IsNullOrWhiteSpace(footer))
            {
                if (footer.StartsWith("DATE(") || footer.StartsWith("TIME(") || footer.StartsWith("DATETIME("))
                {
                    if (footer.Equals("DATE()"))
                        footer = Dates.GetCurrentDateTimeUTC().AddHours(offset).ToFormatMDY();

                    else if (footer.Equals("TIME()"))
                        footer = Dates.GetCurrentDateTimeUTC().AddHours(offset).ToFormatTime();

                    else if (footer.Equals("DATETIME()"))
                        footer = Dates.GetCurrentDateTimeUTC().AddHours(offset).ToFormatMDYTime();

                    else if (footer.EndsWith(")"))
                    {
                        var format = footer.Split("(")[1].Replace(")", "").Trim();
                        footer = DateTime.UtcNow.ToString(format);
                    }
                }
            }

            return footer;
        }
        #endregion


        #region GenerateHtml
        private static (string header, string html) GenerateHtml(DocumentPdf documentPdf, object data)
        {
            string section;
            string header = $"<header style='height: {documentPdf.Setup.HeaderHeight}px;'></header>";
            string main = "<main></main>";
            string lastPage = "<div class='new-page'></div>";

            foreach (DocumentPdfTemplate template in documentPdf.Templates.OrderBy(x => x.Sequence))
            {
                Dictionary<string, object> mappedData = MapObjectData(template.Data, data);
                section = GetTemplate(template.Html, mappedData);

                switch (template.Type)
                {
                    case PDF_TEMPLATE_TYPE.HEADER.Name:
                        header = header.Replace("</header>", $"{section}</header>");
                        break;

                    case PDF_TEMPLATE_TYPE.BODY.Name:
                        main = main.Replace("</main>", $"{section}</main>");
                        break;

                    case PDF_TEMPLATE_TYPE.LAST_PAGE.Name:
                        lastPage = lastPage.Replace("</div>", $"</div><section>{section}</section>");
                        break;
                }
            }

            int marginTop = marginDefaltHTML + documentPdf.Setup.HeaderHeight;
            int marginRight = marginDefaltHTML;
            int marginBottom = marginDefaltHTML;
            int marginLeft = marginDefaltHTML;

            return (header, CleanHTML(@$"
                <!DOCTYPE html>
                <html lang='en'>
                    <head>
                        <meta charset='utf-8'>
                        <meta http-equiv='Content-Type' content='text/html'>
                        <title>coer91.NET</title>
                        {GenerateStyles(marginTop, marginRight, marginBottom, marginLeft)}
                    </head>
                    <body>{main}</body>
                </html>"
            .Replace("</main>", $"{lastPage}</main>")
            ));
        }


        private static Dictionary<string, object> MapObjectData(string xml, object data)
        {  
            if (string.IsNullOrWhiteSpace(xml))
                return [];

            var nodeAttributes = GetNodeAttributes(xml);
            string json = GetJsonString(nodeAttributes);
            List<object> objectList = JsonConvert.DeserializeObject<List<object>>(json);
            if (objectList.Count <= 0) return [];

            json = JsonConvert.SerializeObject(data ?? new object());
            Dictionary<string, object> dataDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return GetObjectValue(objectList[0], dataDictionary);
        }


        private static Dictionary<string, Dictionary<string, string>> GetNodeAttributes(string xml)
        {
            //Get XML Reader
            using MemoryStream memoryStream = new(Encoding.UTF8.GetByteCount(xml));

            using StreamWriter streamWriter = new(memoryStream);
            streamWriter.Write(xml);
            streamWriter.Flush();
            memoryStream.Position = 0;

            using XmlTextReader xmlReader = new(memoryStream);

            //Get Node Attributes
            string parent, grandParent, node, attribute;
            Dictionary<string, int> nodeParents = [];
            Dictionary<string, Dictionary<string, string>> nodeAttributes = [];

            //Rules
            string[] attributesAllowed = ["IGNORE", "OUTPUT"];
            string[] outputsAllowed = ["STRING", "ARRAY", "NUMBER", "CURRENCY", "DATE", "TIME", "DATETIME"];

            while (xmlReader.Read())
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    //Add Node
                    node = xmlReader.Name.FirstCharToUpper();
                    nodeAttributes.Add(node, []);
                    nodeParents.Add(node, xmlReader.Depth);

                    //Parent Attributes
                    parent = nodeParents.LastOrDefault(x => x.Value == (xmlReader.Depth - 1)).Key ?? string.Empty;
                    grandParent = nodeParents.LastOrDefault(x => x.Value == (xmlReader.Depth - 2)).Key ?? string.Empty;
                    nodeAttributes[node].Add("Parent", parent);

                    if (!string.IsNullOrWhiteSpace(grandParent))
                        nodeAttributes[parent]["Output"] = "[]";

                    //Attributes
                    while (xmlReader.MoveToNextAttribute())
                    {
                        if (attributesAllowed.Any(x => x.Equals(xmlReader.Name.ToUpper())))
                        {
                            attribute = xmlReader.Name;
                            attribute = attribute.ToLower();
                            attribute = attribute.FirstCharToUpper();

                            nodeAttributes[node].Add(attribute, xmlReader.Value);
                        }
                    }

                    //Ignore Attribute
                    if (nodeAttributes[node].Any(x => x.Key.Equals("Ignore")))
                        nodeAttributes[node]["Ignore"] = nodeAttributes[node]["Ignore"].ToLower();
                    else
                        nodeAttributes[node].Add("Ignore", "false");

                    //Output Attribute
                    if (nodeAttributes[node].Any(x => x.Key.Equals("Output")))
                    {
                        if (outputsAllowed.Any(x => x.Equals(nodeAttributes[node]["Output"].ToUpper())))
                            nodeAttributes[node]["Output"] = nodeAttributes[node]["Output"].ToUpper().Equals("ARRAY")
                                ? "[]" : nodeAttributes[node]["Output"].ToUpper();

                        else
                            nodeAttributes[node]["Output"] = "STRING";
                    }

                    else
                        nodeAttributes[node].Add("Output", "STRING");
                }

            xmlReader.Close();
            xmlReader.Dispose();
            streamWriter.Close();
            streamWriter.Dispose();
            memoryStream.Close();
            memoryStream.Dispose();

            return nodeAttributes;
        }


        private static string GetJsonString(Dictionary<string, Dictionary<string, string>> nodeAttributes)
        {
            string json = "[{}]";
            bool ignore;
            string parent, output, child;
            string firstElement = string.Empty;
            HashSet<string> ignored = [];
            HashSet<string> parents = [];
            Dictionary<string, Dictionary<string, string>> childrens = [];

            //Add Properties
            foreach (string key in nodeAttributes.Keys)
            {
                parent = nodeAttributes[key].FirstOrDefault(x => x.Key.Equals("Parent")).Value;
                ignore = nodeAttributes[key].FirstOrDefault(x => x.Key.Equals("Ignore")).Value == "true";
                output = nodeAttributes[key].FirstOrDefault(x => x.Key.Equals("Output")).Value;

                //Ignore First Element
                if (string.IsNullOrWhiteSpace(firstElement))
                {
                    firstElement = key;
                    continue;
                }

                //Father is first element?
                if (parent.Equals(firstElement))
                    parent = string.Empty;

                //Ignore this elemet?
                if (ignore)
                {
                    ignored.Add(key);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(parent))
                {
                    json = json.Replace("}]", $"'{key}':'{output}'" + ",}]");
                }

                else
                {
                    parents.Add(parent);
                    childrens.Add(key, new Dictionary<string, string>()
                    {
                        { "Parent", parent },
                        { "Ignore", $"{ignore}" },
                        { "Output", output }
                    });
                }
            }

            //Add Arrays
            foreach (string arrayProperty in parents)
            {
                child = "{}";
                foreach (string key in childrens.Keys)
                {
                    parent = nodeAttributes[key].FirstOrDefault(x => x.Key.Equals("Parent")).Value;
                    ignore = nodeAttributes[key].FirstOrDefault(x => x.Key.Equals("Ignore")).Value == "true";
                    output = nodeAttributes[key].FirstOrDefault(x => x.Key.Equals("Output")).Value;

                    if (arrayProperty.Equals(parent))
                    {
                        if (ignore || ignored.Contains(parent)) continue;
                        child = child.Replace("}", $"'{key}':'{output}'" + ",}");
                    }
                }

                child = child.Replace(",}", "}");
                json = json.Replace($"'{arrayProperty}':'[]'", $"'{arrayProperty}':[{child}]");
            }

            json = json.Replace(",}]", "}]");
            json = json.Replace("'", "\"");
            return json;
        }


        private static Dictionary<string, object> GetObjectValue(dynamic data, Dictionary<string, object> dataDictionary)
        {
            dynamic value;
            Dictionary<string, object> mappedData = [];

            foreach (string property in Validations.GetProperties(data))
            {
                if (dataDictionary.Any(x => x.Key.ToUpper().Equals(property.ToUpper())))
                {
                    value = dataDictionary.TryGetValue(property.FirstCharToUpper(), out dynamic _value)
                        ? _value : dataDictionary[property.FirstCharToLower()];

                    if (value is null)
                    {
                        value = string.Empty;
                    }

                    //Array Property
                    else if (value.GetType().Name == "JArray")
                    {
                        List<dynamic> valueList = [];

                        for (int i = 0; i < value.Count; i++)
                        {
                            var innerData = data[property][0];
                            var json = JsonConvert.SerializeObject(value[i]);
                            var innerDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                            valueList.Add(GetObjectValue(innerData, innerDataDictionary));
                        }

                        value = valueList;
                    }

                    //Format Property
                    else
                        switch (data[property].Value)
                        {
                            case "NUMBER":
                                value = Numbers.ToNumericFormat(value);
                                break;

                            case "CURRENCY":
                                value = Numbers.ToCurrencyFormat(value);
                                break;

                            case "DATE":
                                value = DateTime.TryParse(value, out DateTime date) ? date.ToString("dd MMM, yyyy") : string.Empty;
                                break;

                            case "TIME":
                                value = DateTime.TryParse(value, out DateTime time) ? time.ToString("hh:mm tt") : string.Empty;
                                break;

                            case "DATETIME":
                                value = DateTime.TryParse(value, out DateTime dateTime) ? dateTime.ToString("dd MMM, yyyy - hh:mm tt").Replace("-", "at") : string.Empty;
                                break;
                        }
                }

                else
                    value = string.Empty;

                mappedData.Add(property.FirstCharToUpper(), value);
            }

            return mappedData;
        }


        private static string GetTemplate(string html, Dictionary<string, object> data)
        { 
            html = html.Replace("\"", "'");
            html = html.Replace("\n", "");
            html = html.Replace("\r", "");
            html = html.Replace("\t", "");

            html = html.Replace("<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='1.0'>", "");
            html = html.Replace("</xsl:stylesheet>", "");
            html = html.Replace("<div class='division' />", "<div class='division'></div>");
            html = html.Replace("<div class='division'/>", "<div class='division'></div>");
            html = html.CleanUpBlanks();

            foreach (string property in data.Keys)
            {
                if (data[property].GetType().Namespace == "System.Collections.Generic")
                {
                    string from = $"<tr array='{property}'>";
                    string to = "</tr>";

                    string section = html[(html.IndexOf(from) + from.Length)..];
                    section = section[..section.IndexOf(to)];

                    string sectionData = string.Empty;
                    string elementArray = string.Empty;

                    foreach (var row in data[property] as List<dynamic>)
                    {
                        sectionData = section;
                        var json = JsonConvert.SerializeObject(row);
                        Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                        foreach (string innerProperty in dictionary.Keys)
                            sectionData = SetValue(sectionData, innerProperty, dictionary[innerProperty]);

                        elementArray += $"{from}{sectionData}{to}";
                        elementArray = elementArray.Replace($"array='{property}'", "");
                    }

                    html = html.Replace($"{from}{section}{to}", elementArray);
                }

                else
                    html = SetValue(html, property, data[property]);
            }

            return html;
        }


        private static string SetValue(string html, string key, object value)
        {
            //Value
            if (html.Contains($"data='{key}'"))
            {
                int index = html.IndexOf($"<value data='{key}'");
                string element = html[index..].Split("/>")[0] += "/>";

                if (html.Contains(element))
                    html = html.Replace(element, $"{value}".CleanUpBlanks());
            }

            //Barcode
            if (html.Contains($"barcode='{key}'"))
            {
                int index              = html.IndexOf($"<value barcode='{key}'");
                string element         = html[index..].Split("/>")[0] += "/>";
                string[] properties    = element.Split(" ");
                string width           = properties.FirstOrDefault(x => x.Contains("width="))?.Split('=')[1].Replace("'", "").Replace('"', ' ').Trim();
                string height          = properties.FirstOrDefault(x => x.Contains("height="))?.Split('=')[1].Replace("'", "").Replace('"', ' ').Trim();
                string showCaption     = properties.FirstOrDefault(x => x.Contains("show-caption="))?.Split('=')[1].Replace("'", "").Replace('"', ' ').Trim();
                string captionAlign    = properties.FirstOrDefault(x => x.Contains("caption-align="))?.Split('=')[1].Replace("'", "").Replace('"', ' ').Trim();
                string captionTemplate = properties.FirstOrDefault(x => x.Contains("caption-template="))?.Split('=')[1].Replace("'", "").Replace('"', ' ').Trim();

                if (html.Contains(element))
                    html = html.Replace(element, CodeGenerator.BarcodeHTML(value, width, height, showCaption, captionAlign, captionTemplate));
            }

            //QR
            if (html.Contains($"qr='{key}'"))
            {
                int index              = html.IndexOf($"<value qr='{key}'");
                string element         = html[index..].Split("/>")[0] += "/>";
                string[] properties    = element.Split(" ");
                string size            = properties.FirstOrDefault(x => x.Contains("size="))?.Split('=')[1].Replace("'", "").Replace('"', ' ').Trim();
                string showCaption     = properties.FirstOrDefault(x => x.Contains("show-caption="))?.Split('=')[1].Replace("'", "").Replace('"', ' ').Trim();
                string captionAlign    = properties.FirstOrDefault(x => x.Contains("caption-align="))?.Split('=')[1].Replace("'", "").Replace('"', ' ').Trim();
                string captionTemplate = properties.FirstOrDefault(x => x.Contains("caption-template="))?.Split('=')[1].Replace("'", "").Replace('"', ' ').Trim();

                if (html.Contains(element))
                    html = html.Replace(element, CodeGenerator.QRHTML(value, size, showCaption, captionAlign, captionTemplate));
            }

            return html;
        }


        private static string GenerateStyles(int marginTop, int marginRight, int marginBottom, int marginLeft) => CleanHTML(@$"             
            <style> 
                * {{ font-family: Arial, sans-serif; font-size: 16px; }}
                body {{ margin: 0px; padding: 0px; }}
                h1, h2, h3, h4, h5, h6, p, pre, table, header, section, footer {{ width: 100%; margin: 0px; padding: 0px; }}
                pre {{ white-space: pre-wrap; font-family: Consolas, monaco, monospace !important; }}
                img {{ min-width: 25px; min-height: 25px; }}
                main{{ margin: auto; }}
                header, section {{ margin-bottom: 20px; }} 
                th, td {{ word-break: break-all; }}
                                                                
                /* CONTROL */ 
                .display-none {{ display: none; }} 
                .display-inline-block {{ display: inline-block; }}
                .new-page {{ page-break-after: always; }} 
                
                /* <table> */
                .table-auto      {{ table-layout:    auto;     }}
                .table-fixed     {{ table-layout:    fixed;    }}
                .border-collapse {{ border-collapse: collapse; }}  
                .border-separate {{ border-collapse: separate; }}
                .separate-1      {{ border-spacing:  1.0px;    }}
                .separate-2      {{ border-spacing:  1.5px;    }}
                .separate-3      {{ border-spacing:  2.0px;    }}
                .separate-4      {{ border-spacing:  2.5px;    }}
                .separate-5      {{ border-spacing:  3.0px;    }}
                .separate-6      {{ border-spacing:  3.5px;    }}
                .separate-7      {{ border-spacing:  4.0px;    }}
                .separate-8      {{ border-spacing:  4.5px;    }}
                .separate-9      {{ border-spacing:  5.0px;    }} 

                /* border */
                .border         {{ border:        1px solid black; }}
                .border-top     {{ border-top:    1px solid black; }}
                .border-right   {{ border-right:  1px solid black; }}
                .border-bottom  {{ border-bottom: 1px solid black; }}
                .border-left    {{ border-left:   1px solid black; }}
                .border-dashed  {{ border-style:  dashed;          }}

                .border-width-1px {{ border-width: 1px; }}
                .border-width-2px {{ border-width: 2px; }}
                .border-width-3px {{ border-width: 3px; }}
                .border-width-4px {{ border-width: 4px; }}
                .border-width-5px {{ border-width: 5px; }} 

                .border-color-smoke {{ border-color: whitesmoke; }}

                /* Text */
                .text-left      {{ text-align:      left;         }}
                .text-center    {{ text-align:      center;       }}
                .text-right     {{ text-align:      right;        }}
                .text-top       {{ vertical-align:  top;          }}
                .text-middle    {{ vertical-align:  center;       }}
                .text-bottom    {{ vertical-align:  bottom;       }}   
                .text-small     {{ font-size:       12px;         }} 
                .text-normal    {{ font-size:       18px;         }} 
                .text-large     {{ font-size:       20px;         }} 
                .text-bold      {{ font-weight:     bold;         }}
                .text-italic    {{ font-style:      italic;       }}
                .text-deleted   {{ text-decoration: line-through; }}
                .text-underline {{ text-decoration: underline;    }}
                .text-overline  {{ text-decoration: overline;     }}

                /* Word */
                .word-keep  {{ word-break: keep-all; }}  
                .word-break {{ word-break: break-all; }}  

                /* Word */
                .height-100 {{ height:100%; max-height:100%; }}  
                .width-100  {{ width:100%;  max-width:100%;  }}

                /* Margin */
                .margin-top-5px     {{ margin-top: 5px;     }}
                .margin-top-10px    {{ margin-top: 10px;    }}
                .margin-right-10px  {{ margin-right: 10px;  }}
                .margin-bottom-5px  {{ margin-bottom: 5px;  }}  
                .margin-bottom-10px {{ margin-bottom: 10px; }}  
                .margin-left-5px    {{ margin-left: 5px;    }} 
                .margin-left-10px   {{ margin-left: 10px;   }}

                 @page {{
                    margin-top: {marginTop}px !important; 
                    margin-right: {marginRight}px !important; 
                    margin-bottom: {marginBottom}px !important; 
                    margin-left: {marginLeft}px !important; 
                }}
            </style>"
        );


        private static string CleanHTML(string html) => string.Join(' ', html
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .Replace("\t", string.Empty)
            .Replace("\"", "\'")
            .Split(' ')
            .Where(x => x.Length > 0)
            .ToArray()
        );
        #endregion
    }
}