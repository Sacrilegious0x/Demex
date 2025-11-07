// Services/Pdf/OrderPdfGenerator.cs

using LAFABRICA.Data.DB;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

// Clase DTO para pasar los datos de forma limpia
public class OrderPdfModel
{
    public Order Order { get; set; }
    public Client Client { get; set; }
}

public class OrderPdfGenerator
{
    private readonly OrderPdfModel _model;

    public OrderPdfGenerator(OrderPdfModel model)
    {
        _model = model;
    }

    public byte[] GenerateDocument()
    {
        return Document.Create(container =>
        {
            // Configuración de la página
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30, Unit.Point);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                // Encabezado del documento
                page.Header().Element(ComposeHeader);

                // Contenido principal
                page.Content().Element(ComposeContent);

                // Pie de página
                page.Footer().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            //  Fila principal que contiene todo el contenido del encabezado (texto y logo)
            column.Item().Row(row =>
            {
                // Columna izquierda: Agrupa TODO el texto aquí
                row.RelativeItem().Column(textCol =>
                {
                    // Bloque de título
                    textCol.Item().Text("DEMEX: La fabrica del arte").Bold().FontSize(20);
                    textCol.Item().Text("Cotización de Productos y Servicios");
                    textCol.Item().Text("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy"));

                    // Espacio entre el título y la información de contacto
                    textCol.Spacing(15);

                    // Bloque de información de contacto (movido aquí)
                    textCol.Item().Text("600 Metros Oeste del Hospital y 25 Metros Sur, Guápiles, Costa Rica").FontSize(9);
                    textCol.Item().Text("Teléfono: 7297 9222").FontSize(9);
                    textCol.Item().Text("Email: demexartesanias@gmail.com").FontSize(9);
                });

                // Columna derecha: El logo se mantiene igual
                row.ConstantItem(150).Image("wwwroot/images/demex_logo.jpg");
            });

            //  Línea divisoria al final del encabezado
            column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(20).Column(column =>
        {
            // Sección de datos del cliente
            column.Item().Element(ComposeClientInfo);
            column.Spacing(20);

            // Tabla de productos
            column.Item().Element(ComposeProductsTable);
            column.Spacing(20);

            // Resumen financiero
            column.Item().AlignRight().Element(ComposeFinancialSummary);
        });
    }

    void ComposeClientInfo(IContainer container)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
        {
            col.Item().Text("Cliente").Bold().FontSize(12);
            col.Item().Text(_model.Client.Name);
            col.Item().Text(_model.Client.Email);

            //  Añadir número de contacto
            if (!string.IsNullOrEmpty(_model.Client.PhoneNumber))
                col.Item().Text("Teléfono: " + _model.Client.PhoneNumber);

            //  Dirección y Dirección Específica en la misma línea
            var location = _model.Client.Location;
            var specificLocation = _model.Client.SpecificLocation;
            string fullLocation = string.Empty;

            if (!string.IsNullOrEmpty(location))
                fullLocation += location;

            if (!string.IsNullOrEmpty(specificLocation))
            {
                if (!string.IsNullOrEmpty(fullLocation))
                    fullLocation += " / ";
                fullLocation += specificLocation;
            }

            col.Item().Text(fullLocation);
        });
    }

    void ComposeProductsTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3); // Nombre del producto
                columns.ConstantColumn(60); // Cantidad
                columns.RelativeColumn();   // Precio Unitario
                columns.RelativeColumn();   // Subtotal
            });

            // Encabezado de la tabla
            table.Header(header =>
            {
                header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Producto").Bold();
                header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignCenter().Text("Cantidad").Bold();
                header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("Precio Unit.").Bold();
                header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("Subtotal").Bold();
            });

            // Filas de productos
            foreach (var item in _model.Order.ProductOrders)
            {
                var product = item.IdProductNavigation;
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(product?.Name ?? "N/A");
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text(item.Quantity.ToString());
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(product?.PriceBase.ToString("C"));
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text((item.Quantity * (product?.PriceBase ?? 0)).ToString("C"));
            }
        });
    }

    void ComposeFinancialSummary(IContainer container)
    {
        container.Width(200).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            var total = _model.Order.TotalAmount;
            var advance = _model.Order.Advancement;

            table.Cell().Padding(2).Text("Total:");
            table.Cell().Padding(2).AlignRight().Text(total.ToString("C")).Bold();

            table.Cell().Padding(2).Text("Adelanto:");
            table.Cell().Padding(2).AlignRight().Text(advance.ToString("C")).Bold();

            table.Cell().ColumnSpan(2).BorderTop(1).BorderColor(Colors.Grey.Lighten2);

            table.Cell().Padding(2).Text("Saldo Pendiente:").Bold();
            table.Cell().Padding(2).AlignRight().Text((total - advance).ToString("C")).Bold();
        });
    }
}