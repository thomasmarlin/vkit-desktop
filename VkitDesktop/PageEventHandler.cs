using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace VkitDesktop
{
    class PageEventHandler : iTextSharp.text.pdf.PdfPageEventHelper
    {

        public static bool PrintingBlackBorder = false;
        public static bool UsingCubeFeatures = false;
        public static iTextSharp.text.BaseColor BackColor = iTextSharp.text.BaseColor.WHITE;

        public override void OnStartPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {

            if (!UsingCubeFeatures)
            {
                return;
            }

            iTextSharp.text.BaseColor lineColor = iTextSharp.text.BaseColor.BLACK;
            if (PrintingBlackBorder)
            {
                lineColor = iTextSharp.text.BaseColor.WHITE;
            }


            // Vertical bars at:
            // 13, 114, 223, 312

            // Horizontal bars at:
            // 12, 137, 263, 388

            float halfWidth = 1.0f;

            // Do it twice, once on top, once on bottom
            for (int i = 0; i < 2; i++)
            {
                float top = 17;
                float bottom = 0;
                if (i == 1)
                {
                    top = 783;
                    bottom = 800;
                }
                iTextSharp.text.Rectangle Vert1 = new iTextSharp.text.Rectangle(27.5f - halfWidth, top, 27.5f + halfWidth, bottom);
                Vert1.BackgroundColor = lineColor;
                document.Add(Vert1);

                iTextSharp.text.Rectangle Vert2 = new iTextSharp.text.Rectangle(206.2f - halfWidth, top, 206.2f + halfWidth, bottom);
                Vert2.BackgroundColor = lineColor;
                document.Add(Vert2);

                iTextSharp.text.Rectangle Vert3 = new iTextSharp.text.Rectangle(385.3f - halfWidth, top, 385.3f + halfWidth, bottom);
                Vert3.BackgroundColor = lineColor;
                document.Add(Vert3);

                iTextSharp.text.Rectangle Vert4 = new iTextSharp.text.Rectangle(564.1f - halfWidth, top, 564.1f + halfWidth, bottom);
                Vert4.BackgroundColor = lineColor;
                document.Add(Vert4);
            }



            // Horizontal bars at:
            // 12, 137, 263, 388

            for (int i = 0; i < 2; i++)
            {
                float right = 0;
                float left = 17;
                if (i == 1)
                {
                    right = 575;
                    left = 650;
                }
                iTextSharp.text.Rectangle Horz1 = new iTextSharp.text.Rectangle(right, 27 - halfWidth, left, 27 + halfWidth);
                Horz1.BackgroundColor = lineColor;
                document.Add(Horz1);

                iTextSharp.text.Rectangle Horz2 = new iTextSharp.text.Rectangle(right, 277 - halfWidth, left, 277 + halfWidth);
                Horz2.BackgroundColor = lineColor;
                document.Add(Horz2);

                iTextSharp.text.Rectangle Horz3 = new iTextSharp.text.Rectangle(right, 527 - halfWidth, left, 527 + halfWidth);
                Horz3.BackgroundColor = lineColor;
                document.Add(Horz3);

                iTextSharp.text.Rectangle Horz4 = new iTextSharp.text.Rectangle(right, 777 - halfWidth, left, 777 + halfWidth);
                Horz4.BackgroundColor = lineColor;
                document.Add(Horz4);
            }
            /*
            int iVert = 0;

            for (int i = 0; i < document.PageSize.Width; i+=20)
            {
                iTextSharp.text.Rectangle Vert = new iTextSharp.text.Rectangle(i, 20, i+2, 0 );
                Vert.BackgroundColor = iTextSharp.text.BaseColor.RED;
                document.Add(Vert);
            }

            for (int i = 0; i < document.PageSize.Height; i += 20)
            {
                iTextSharp.text.Rectangle Vert = new iTextSharp.text.Rectangle(0, i, 20, i+2);
                Vert.BackgroundColor = iTextSharp.text.BaseColor.BLUE;
                document.Add(Vert);
            }

            /*
            float top = document.PageSize.Height - 10.0f;
            iTextSharp.text.Rectangle TopLeft = new iTextSharp.text.Rectangle(iHorz1 - 1, top+20, iHorz1 + 1, top);

            //iTextSharp.text.Rectangle TopLeft = new iTextSharp.text.Rectangle(iHorz1-1, iVert4, iHorz1 + 1, iVert4);
            TopLeft.BackgroundColor = iTextSharp.text.BaseColor.BLUE;

            iTextSharp.text.Rectangle BottomRight = new iTextSharp.text.Rectangle(iHorz4-1, iVert1, iHorz4 + 1, iVert1 -20);
            BottomRight.BackgroundColor = iTextSharp.text.BaseColor.RED;


            iTextSharp.text.Rectangle LeftTop = new iTextSharp.text.Rectangle(iHorz1-20, iVert4-1, iHorz1, iVert4+1);
            TopLeft.BackgroundColor = iTextSharp.text.BaseColor.GRAY;

            iTextSharp.text.Rectangle RightBottom = new iTextSharp.text.Rectangle(iHorz4, iVert1-1, iHorz4 + 20, iVert1 +1);
            RightBottom.BackgroundColor = iTextSharp.text.BaseColor.GREEN;


            document.Add(TopLeft);
            document.Add(BottomRight);

            document.Add(LeftTop);
            document.Add(RightBottom);
             */
        }


        /*

        public override void onStartPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            iTextSharp.text.Rectangle TopLeft = new iTextSharp.text.Rectangle(iHorz1, iVert4, iHorz1 + 1, iVert4 + 1);
            TopLeft.BackgroundColor = iTextSharp.text.BaseColor.GRAY;

            iTextSharp.text.Rectangle RightBottom = new iTextSharp.text.Rectangle(iHorz4, iVert1, iHorz4 + 1, iVert1 + 1);
            RightBottom.BackgroundColor = iTextSharp.text.BaseColor.GRAY;

            document.Add(TopLeft);
            document.Add(RightBottom);

            
            iTextSharp.text.Image imgfoot =
iTextSharp.text.Image.getInstance("FooterImage.jpg");
            iTextSharp.text.Image imghead =
iTextSharp.text.Image.getInstance("HeaderImage.jpg");

            imgfoot.setAbsolutePosition(0, 0);
            imghead.setAbsolutePosition(0, 0);

            PdfContentByte cbhead = writer.DirectContent;
            PdfTemplate tp = cbhead.createTemplate(600, 250);
            tp.addImage(imghead);

            PdfContentByte cbfoot = writer.DirectContent;
            PdfTemplate tpl = cbfoot.createTemplate(600, 250);
            tpl.addImage(imgfoot);

            cbhead.addTemplate(tp, 0, 715);
            cbfoot.addTemplate(tpl, 0, 0);

            Phrase headPhraseImg = new Phrase(cbhead + "",
FontFactory.getFont(FontFactory.TIMES_ROMAN, 7,
iTextSharp.text.Font.NORMAL));
            Phrase footPhraseImg = new Phrase(cbfoot + "",
FontFactory.getFont(FontFactory.TIMES_ROMAN, 7,
iTextSharp.text.Font.NORMAL));

            HeaderFooter header = new HeaderFooter(headPhraseImg, true);
            HeaderFooter footer = new HeaderFooter(footPhraseImg, true);


            base.onStartPage(writer, document);
             
        } 

        public override void OnGenericTag(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc,
            iTextSharp.text.Rectangle rect, string s)
        {
            // Do Nothing
        }
        public override void OnSectionEnd(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc,
            float f)
        {
            // Do Nothing
        }
        public override void OnSection(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc,
            float f, int i, iTextSharp.text.Paragraph p)
        {
            // Do Nothing
        }
        public override void OnChapter(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc,
            float f, iTextSharp.text.Paragraph p)
        {
            // Do Nothing
        }

    */

    }
}
