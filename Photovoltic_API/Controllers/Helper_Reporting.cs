using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Photovoltic_API.Controllers
{
    public class Helper_Reporting
    {

        public  PdfPTable CreateTable(float[] ColumnWidths, int Border, float PercentageWidth, float SpaceBefore, float SpaceAfter)
        {
            var tbl = new PdfPTable(ColumnWidths);
            //tbl.KeepTogether = true;
            tbl.DefaultCell.Border = Border;
            tbl.SpacingBefore = SpaceBefore;
            tbl.SpacingAfter = SpaceAfter;
            tbl.WidthPercentage = PercentageWidth;
            return tbl;
        }

        public PdfPTable CreateTable(int NumOfColumn, int Border, float PercentageWidth, float SpaceBefore, float SpaceAfter)
        {
            var tbl = new PdfPTable(NumOfColumn);
            //tbl.KeepTogether = true;
            tbl.DefaultCell.Border = Border;
            tbl.SpacingBefore = SpaceBefore;
            tbl.SpacingAfter = SpaceAfter;
            tbl.WidthPercentage = PercentageWidth;
            return tbl;
        }

        public PdfPCell CreatePaddingCellBorder(string text, Font f, int numberOfSpan, int HAlign, float paddingTop, float paddingBottom, float leftBorder, float TopBorder, float RighBorder, float BottomBorder, float paddingLeft, float paddingRight, BaseColor BGcolor)
        {
            PdfPCell cell = new PdfPCell(new Paragraph(text, f));
            cell.HorizontalAlignment = HAlign;
            //  cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = numberOfSpan;
            cell.PaddingBottom = paddingBottom;
            cell.PaddingTop = paddingTop;
            cell.PaddingLeft = paddingLeft;
            cell.PaddingRight = paddingRight;
            cell.BorderWidthLeft = leftBorder;
            cell.BorderWidthTop = TopBorder;
            cell.BorderWidthRight = RighBorder;
            cell.BorderWidthBottom = BottomBorder;
            // cell.Padding = 0f;
            // cell.BorderColor = new BaseColor(187, 185, 178);
            cell.BackgroundColor = BGcolor;


            //
            return cell;
        }

        public PdfPCell CreateCellPadding(IElement element, float BorderLeft, float BorderTop, float BorderRight, float BorderBottom, float Padding)
        {

            PdfPCell cell = new PdfPCell();
            cell.AddElement(element);
            cell.Padding = Padding;
            //  cell.BorderColor = new BaseColor(187, 185, 178);
            cell.BorderWidthLeft = BorderLeft;
            cell.BorderWidthRight = BorderRight;
            cell.BorderWidthTop = BorderTop;
            cell.BorderWidthBottom = BorderBottom;

            return cell;

        }
        public PdfPCell CreateCell(IElement element, float BorderLeft, float BorderTop, float BorderRight, float BorderBottom, float PaddingBottom)
        {

            PdfPCell cell = new PdfPCell();
            cell.AddElement(element);
            cell.PaddingBottom = PaddingBottom;
            cell.BorderWidthLeft = BorderLeft;
            cell.BorderWidthRight = BorderRight;
            cell.BorderWidthTop = BorderTop;
            cell.BorderWidthBottom = BorderBottom;
            return cell;

        }

        public PdfPCell CreateCell(string text, Font f, int numberOfSpan, int HAlign, bool border = false)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, f));
            cell.HorizontalAlignment = HAlign;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            // cell.PaddingTop = 2f;
            //cell.PaddingBottom = 5f;
            cell.Colspan = numberOfSpan;
            if (border)
            {
                cell.BorderWidthBottom = 1;

            }
            else
            {
                cell.BorderWidthBottom = 0;
                cell.BorderWidthTop = 0;
            }
            cell.BorderWidthLeft = 0;
            cell.BorderWidthRight = 0;
            //
            return cell;
        }
        public PdfPCell CreateCell(Phrase text, Font f, int numberOfSpan, int HAlign, bool border = false)
        {
            PdfPCell cell = new PdfPCell(text);
            cell.HorizontalAlignment = HAlign;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            // cell.PaddingTop = 2f;
            //cell.PaddingBottom = 5f;
            cell.Colspan = numberOfSpan;
            if (border)
            {
                cell.BorderWidthBottom = 1;

            }
            else
            {
                cell.BorderWidthBottom = 0;
                cell.BorderWidthTop = 0;
            }
            cell.BorderWidthLeft = 0;
            cell.BorderWidthRight = 0;
            //
            return cell;
        }

        public iTextSharp.text.Phrase _ParaChunks(string heading, string parahgraph, Font fontheading, Font fontparah)
        {
            // Chunk heading = new Chunk(heading, MethodAndLimitationHeading);
            var phconditiontest = new iTextSharp.text.Phrase() { Font = fontparah };

            var chheading = new Chunk(heading.ToString(), fontheading);
            phconditiontest.Add(chheading);

            var chInheritanceVal = new Paragraph(new Chunk(parahgraph, fontparah));
            chInheritanceVal.SetLeading(0f, 5f);

            phconditiontest.Add(chInheritanceVal);

            return phconditiontest;
        }
        public PdfPCell CreatePaddingCellBordercolor(string text, Font f, int numberOfSpan, int HAlign, float paddingTop, float paddingBottom, float leftBorder, float TopBorder, float RighBorder, float BottomBorder, float paddingLeft, float paddingRight, BaseColor BGcolor, BaseColor Bordercolor)
        {
            PdfPCell cell = new PdfPCell(new Paragraph(text, f));
            cell.HorizontalAlignment = HAlign;
            //  cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = numberOfSpan;
            cell.PaddingBottom = paddingBottom;
            cell.PaddingTop = paddingTop;
            cell.PaddingLeft = paddingLeft;
            cell.PaddingRight = paddingRight;
            cell.BorderWidthLeft = leftBorder;
            cell.BorderWidthTop = TopBorder;
            cell.BorderWidthRight = RighBorder;
            cell.BorderWidthBottom = BottomBorder;
            // cell.Padding = 0f;
            cell.BorderColor = Bordercolor;
            cell.BackgroundColor = BGcolor;


            //
            return cell;
        }
    }
}