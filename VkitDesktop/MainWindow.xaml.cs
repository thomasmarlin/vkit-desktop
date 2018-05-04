using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using pdf = iTextSharp.text.pdf;
using System.Runtime;
using System.Runtime.InteropServices;
using System.IO;
using Image = System.Drawing.Image;
using Path = System.IO.Path;
using System.Drawing;
using System.Drawing.Text;
using System.ComponentModel;

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing;
using System.Collections.Generic;
using System.Threading;

namespace VkitDesktop
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<String> myImageList = new List<String>();
        System.Drawing.KnownColor AlternateColor = System.Drawing.KnownColor.White;
        double HorizontalSpacing = 10.0;
        bool BackgroundMatchesFirstCard = false;
        int StartNumberingAt = 1;
        bool ShowCubeFeatures = false;
        bool ShowCardNumbers = true;

        PageEventHandler myEvents;

        public MainWindow()
        {
            InitializeComponent();

            myCubeFeaturesCheckbox.IsChecked = false;

            Dispatcher.UnhandledException += OnUnhandledException;
        }


        void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            System.Windows.MessageBox.Show(string.Format("Uncaught exception: {0}\n\nPlease send this to Tom.", e.Exception), "Error");        
        }


        /*
        private void myLoadImagesButton_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo info = new DirectoryInfo(@"C:\VkitDownloads");
            foreach (var dir in info.GetDirectories())
            {
                myImageList.Add(String.Format("{0}\\image.png", dir.FullName));
            }
        }
         */


        private List<Card> GetSelectedCards
            (bool smallCards, bool bigCards)
        {
            List<Card> selectedCards = new List<Card>();
            foreach (var selection in mySelectedVcards.Items)
            {
                Card card = selection as Card;
                if (card != null)
                {


                    try
                    {
                        // See if the card is Tall or wide  (full-template vs half-slip)
                        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(card.Path);
                        if ((smallCards && (bmp.Height < bmp.Width)) ||
                            (bigCards && (bmp.Height >= bmp.Width)))
                        {
                            selectedCards.Add(card);
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    
                }
            }

            return selectedCards;
        }


        private String SaveFileDialog()
        {
            String fileName = "";
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "VirtualCards_" + DateTime.Now.ToString("yyyy-MMM-dd_hh-mm"); // Default file name
            dlg.DefaultExt = ".pdf"; // Default file extension
            dlg.Filter = "PDF documents (.pdf)|*.pdf"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                fileName = dlg.FileName;
            }

            return fileName;
        }


        private void myWriteButton_Click(object sender, RoutedEventArgs e)
        {

            String fileName = SaveFileDialog();
            if (String.IsNullOrWhiteSpace(fileName))
            {
                return;
            }


            System.Drawing.KnownColor backColorToUse = System.Drawing.KnownColor.White;

            int iCardNum = StartNumberingAt;
            foreach (var item in mySelectedVcards.Items)
            {
                (item as Card).CardNumber = iCardNum;

                if (BackgroundMatchesFirstCard)
                {
                    backColorToUse = (item as Card).BorderColor;
                }

                iCardNum++;
            }

            iTextSharp.text.BaseColor backColorToDraw = new iTextSharp.text.BaseColor(WhiteBorderConverter.GetCompatibleColor(backColorToUse)); //new iTextSharp.text.BaseColor(System.Drawing.Color.FromKnownColor(backColorToUse));
            PageEventHandler.PrintingBlackBorder = (backColorToUse == KnownColor.Black);
            PageEventHandler.BackColor = backColorToDraw;
            PageEventHandler.UsingCubeFeatures = (myCubeFeaturesCheckbox.IsChecked == true);

            using (WaitDlg dlg = new WaitDlg())
            {

                Document sizeDoc = new Document(PageSize.LETTER);
                iTextSharp.text.Rectangle backgroundRect = new iTextSharp.text.Rectangle(sizeDoc.PageSize);

                Document document;

                if (BackgroundMatchesFirstCard)
                {
                    backgroundRect.BackgroundColor = backColorToDraw;
                    document = new Document(backgroundRect);
                }
                else
                {
                    document = new Document(PageSize.LETTER);
                }
                

                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.Create));
                myEvents = new PageEventHandler();
                writer.PageEvent = myEvents;
                
                


                //document.SetMargins(0, 0, 10, 0);
                // WORKING! document.SetMargins(document.LeftMargin, document.RightMargin, 25, 25);

                //document.SetMargins(0.0f, 0.0f, 0.0f, 0.0f);
                document.SetMargins(document.LeftMargin, document.RightMargin, 15, 10);

                // step 3: we open the document
                document.Open();
                
                //iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph();
                PdfPTable table = new pdf.PdfPTable(3);
                table.SpacingBefore = 0;
                table.SpacingAfter = 10;
                //table.TotalWidth = 175.5f * 3.0f + 30f;
                table.TotalWidth = 179.0f * 3.0f + 20f;
                table.LockedWidth = true;
                table.AbsoluteWidths[0] = 179.0f + (float)HorizontalSpacing;
                table.AbsoluteWidths[1] = 179.0f + (float)HorizontalSpacing;
                table.AbsoluteWidths[2] = 179.0f + (float)HorizontalSpacing;                         


                List<Card> smallCards = GetSelectedCards(true, false);
                List<Card> bigCards = GetSelectedCards(false, true);


                WriteCards(bigCards, table, document, backColorToUse);
                WriteCards(smallCards, table, document, backColorToUse);

                PdfPCell emptyCell = new PdfPCell();
                emptyCell.BackgroundColor = backColorToDraw;
                emptyCell.BorderColor = backColorToDraw;

                table.AddCell(emptyCell);
                table.AddCell(emptyCell);
                table.AddCell(emptyCell);


                foreach (var row in table.Rows)
                {
                    //pdf.PdfPCell cell = row.GetCells()[0];
                    //float top = row.GetCells()[0].Bottom;

                    //iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(-30,top, 50, top+3);
                    //document.Add(rect);
                }

                document.Add(table);
                document.Close();
            }

            MessageBox.Show("PDF Generation Complete!");
        }


        //System.Drawing.Bitmap AddCardNumber(System.Drawing.Bitmap image, Card imageFile)
        void AddCardNumber(ref System.Drawing.Bitmap image, Card imageFile)
        {
            float rectHeight = 13;
            float rectWidth = 45;
            float elipseWidth = 3;
            float right = image.Width - 15;
            float top = (float)(image.Height - rectHeight);
            float left = (float)(right - rectWidth);
            

            SolidBrush background = new SolidBrush(System.Drawing.Color.Black);
            SolidBrush textColor = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            if (imageFile.BorderColor != KnownColor.Black)
            {
                background = new SolidBrush(System.Drawing.Color.White);
                textColor = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            }


            System.Drawing.Font fn = new System.Drawing.Font("Lucida Fax", 10, System.Drawing.FontStyle.Bold);

            using (Graphics gr = Graphics.FromImage(image))
            {

                //
                // Bottom Text
                //
                if (true)
                {
                    System.Drawing.Rectangle backgroundRect = new System.Drawing.Rectangle(
                        (int)left, (int)top, (int)rectWidth, (int)rectHeight - 2);

                    gr.DrawRectangle(new System.Drawing.Pen(background), backgroundRect);
                    gr.FillRectangle(background, backgroundRect);
                    gr.FillEllipse(background, new System.Drawing.Rectangle(
                        (int)(left + rectWidth - elipseWidth), (int)top, (int)(elipseWidth * 2), (int)rectHeight - 2));
                    gr.FillEllipse(background, new System.Drawing.Rectangle(
                        (int)(left - elipseWidth), (int)top, (int)(elipseWidth * 2), (int)rectHeight - 2));

                    gr.DrawString(imageFile.CardNumber.ToString(), fn, textColor, new System.Drawing.PointF(
                        right - gr.MeasureString(imageFile.CardNumber.ToString(), fn).Width,
                        //top - 1));
                        top-1.6f));
                }
            }

            using (Graphics grSide = Graphics.FromImage(image))
            {

                //
                // SideText
                //
                if (!string.IsNullOrWhiteSpace(imageFile.SideText))
                {

                    // Find the length of the text
                    System.Drawing.SizeF size = grSide.MeasureString(imageFile.SideText, fn);

                    int xMiddle = image.Height / 2;
                    int xStart = xMiddle + (int)size.Width / 2;
                    //Orig: int yStart = 13;
                    int yStart = 15;

                    grSide.RotateTransform(-90);
                    grSide.DrawString(imageFile.SideText, fn, textColor, new System.Drawing.PointF(
                        -xStart, 0));
                }

                //grSide.RotateTransform(-90);
            }

            //return image;
        }

        private void WriteCards(List<Card> cards, PdfPTable table, iTextSharp.text.Document document, KnownColor backColor)
        {
            foreach (var imageFile in cards)
            {
                if (File.Exists(imageFile.Path))
                {
                    System.Drawing.Bitmap originalImage = new System.Drawing.Bitmap(imageFile.Path);
                    System.Drawing.Bitmap image = null;

                    if (imageFile.BorderColor != System.Drawing.KnownColor.Black)
                    {
                        image = WhiteBorderConverter.ToWhiteBorder(originalImage, imageFile.BorderColor);
                    }
                    else
                    {
                        image = WhiteBorderConverter.CreateNonIndexedImage(originalImage);
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    originalImage.Dispose();
                    originalImage = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    if (myCubeFeaturesCheckbox.IsChecked == true)
                    {
                        WhiteBorderConverter.FixCorners(ref image, imageFile.BorderColor);
                        
                        // Do I need this for anything
                        //if (image.Width > image.Height)
                        //{
                        //    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        //}

                        if (ShowCardNumbers)
                        {
                            //image = AddCardNumber(ref image, imageFile);
                            AddCardNumber(ref image, imageFile);
                        }
                    }
                    //iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(image, System.Drawing.Imaging.ImageFormat.Png);
                    //iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(image, System.Drawing.Imaging.ImageFormat.Gif);
                    iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(image, System.Drawing.Imaging.ImageFormat.Bmp);


                    float scalePercent = 100.0f * 179.0f / (pic.Width);

                    // For cube, also only make sure that the heights are correct
                    if (true == this.myCubeFeaturesCheckbox.IsChecked)
                    {
                        scalePercent = 100.0f * 250.0f / (pic.Height);
                        /*
                        float newHeight = pic.Height * scalePercent / 100.0f;
                        
                        // Limit height to 251 at the most
                        if (newHeight > 251)
                        {
                            scalePercent = 100.0f * 250.0f / (pic.Height);
                        }

                        int p = 0;*/
                    }

                    //pic.ScalePercent(51.0f);
                    pic.ScalePercent(scalePercent);

                    PdfPCell cell = new PdfPCell(pic);
                    //cell.FixedHeight = pic.ScaledHeight;
                    cell.FollowingIndent = 0;
                    cell.Indent = 0;
                    cell.Padding = 0;
                    cell.SetLeading(0, 0);
                    //cell.BorderColor = BaseColor.WHITE;
                    //cell.BorderColor = new iTextSharp.text.BaseColor(System.Drawing.Color.FromKnownColor(backColor));
                    //cell.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.Color.FromKnownColor(backColor));
                    cell.BorderColor = new iTextSharp.text.BaseColor(WhiteBorderConverter.GetCompatibleColor(backColor));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(WhiteBorderConverter.GetCompatibleColor(backColor));

                    table.AddCell(cell);

                    // Draw a rectangle at the right of the doc
                    //float vert = document.GetTop(0) - table.TotalHeight;
                    //iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(0, vert, 50, vert+30);
                    //rect.BackgroundColor = BaseColor.RED;
                    //document.Add(rect);

                    GC.Collect();
                }
            }
        }



        private void FixPaths()
        {
            String assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            String installFolder = Path.GetDirectoryName(assemblyLocation);
            DirectoryInfo info = new DirectoryInfo(String.Format("{0}\\vcards", installFolder));
            foreach (var dir in info.GetDirectories())
            {
                if (dir.FullName.Contains("-renamed"))
                {
                    dir.MoveTo(dir.FullName.Substring(0, dir.FullName.IndexOf("-renamed")));
                }
                
                string newFolderName = dir.FullName;

                char[] arr = newFolderName.ToCharArray();

                arr = Array.FindAll<char>(arr, (c => (
                    char.IsLetterOrDigit(c) || 
                    char.IsWhiteSpace(c) || 
                    c == '-' ||
                    c == '!' ||
                    c == '?' ||
                    c == '\\' ||
                    c == '/' ||
                    c == ':' ||
                    c == ',' ||
                    c == '.' ||
                    c == '&' ||
                    c == '"' ||
                    c == ')' ||
                    c == '(')));
                newFolderName = new string(arr);

                dir.MoveTo(newFolderName + "-renamed");
                 
             
            }
        }


        String removeNonAscii(String original)
        {
            char[] arr = original.ToCharArray();

            arr = Array.FindAll<char>(arr, (c => (
                char.IsLetterOrDigit(c))));
            return new string(arr);
        }



        private void OnLoaded(object sender, RoutedEventArgs e)
        {

            MessageBoxResult result = MessageBox.Show(
                "This program may ONLY be used to print out Virtual Cards " +
                "which have been officially sanctioned by the Star Wars Players Committee. \n\n"+
                "All other use of this program is strictly prohibitted." +
                "\n\nClick 'Yes' to agree",
                "Terms of Use",
                MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }


            //StreamWriter missing = new StreamWriter("missingImageFiles.txt");
            String assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            String installFolder = Path.GetDirectoryName(assemblyLocation);

            DirectoryInfo info = new DirectoryInfo(installFolder);
            foreach (var dir in info.GetDirectories())
            {
                if (dir.Name.ToLower() == "cube")
                {
                    ShowCubeFeatures = true;
                }
            }

            if (ShowCubeFeatures)
            {
                MessageBox.Show("Printing of full image templates is not allowed! Remove your 'cube' files");
                ShowCubeFeatures = false;
            }


            if (ShowCubeFeatures)
            {
                myCubeFeaturesCheckbox.Visibility = System.Windows.Visibility.Visible;
                myOptionsButton.Visibility = System.Windows.Visibility.Visible;
                myCubeFormatMenu.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                myCardFormatMenu.Items.Remove(myCubeFormatMenu);
            }

            // Temporarily No Standard format
            String standardFolder = Path.GetDirectoryName(assemblyLocation) + "/standard";
            if (Directory.Exists(standardFolder))
            {
                myStandardFormatMenu.IsChecked = true;
                LoadNewFormat("standard");
            }
            else
            {
                myCardFormatMenu.Items.Remove(myStandardFormatMenu);
                myLegacyFormatMenu.IsChecked = true;
                LoadNewFormat("legacy");
            }
            

        }

        private void LoadNewFormat(string format)
        {

            myImageList.Clear();

            String assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            String installFolder = Path.GetDirectoryName(assemblyLocation);

            DirectoryInfo info = new DirectoryInfo(String.Format("{0}\\{1}", installFolder, format));
            foreach (var dir in info.GetDirectories())
            {
                
                string imageFile = String.Format("{0}\\image.png", dir.FullName);
                if (File.Exists(imageFile))
                {
                    myImageList.Add(imageFile);
                }
                else
                {
                    //missing.WriteLine(imageFile);
                }
            }

            //missing.Close();
            ApplyFilter();
        }


        private void ApplyFilter()
        {
            String filter = myFilterText.Text;

            myFilteredVcards.Items.Clear();
            foreach (var card in myImageList)
            {
                String folder = Path.GetDirectoryName(card);
                String cardName = Path.GetFileName(folder);

                if (cardName.ToLower().Contains(filter.ToLower()))
                {
                    myFilteredVcards.Items.Add(new Card(cardName, card, KnownColor.Black, ""));
                }
            }

            if (myFilteredVcards.Items.Count > 0)
            {
                myFilteredVcards.SelectedItem = myFilteredVcards.Items.GetItemAt(0);
            }
        }

        private void myMoveRightButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var selection in myFilteredVcards.SelectedItems)
            {
                Card card = selection as Card;
                AddCard(card, KnownColor.Black);
            }
        }

        private void myAddWhiteBorderButtonClick(object sender, RoutedEventArgs e)
        {
            foreach (var selection in myFilteredVcards.SelectedItems)
            {
                Card card = selection as Card;
                AddCard(card, AlternateColor);
            }
        }

        private void AddCard(Card card, KnownColor borderColor)
        {
            Card copy = card.Copy();
            copy.BorderColor = borderColor;
            mySelectedVcards.Items.Add(copy);
        }

        private void myMoveLeftButton_Click(object sender, RoutedEventArgs e)
        {
            MoveCardsLeft();
        }

        private void MoveCardsLeft()
        {
            while (mySelectedVcards.SelectedItems.Count > 0)
            {
                mySelectedVcards.Items.Remove(mySelectedVcards.SelectedItems[0]);
            }
        }

        private void OnFilterChange(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void OnVcardDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UIElement elem = (UIElement)myFilteredVcards.InputHitTest(e.GetPosition(myFilteredVcards));
            while (elem != myFilteredVcards)
            {
                if (elem is ListBoxItem)
                {
                    object selectedItem = ((ListBoxItem)elem).Content;
                    
                    // Handle the double click here
                    AddCard(selectedItem as Card, KnownColor.Black);
                    return;
                }
                elem = (UIElement)VisualTreeHelper.GetParent(elem);
            }
        }

        public void FilterdCards_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                foreach (var selection in myFilteredVcards.SelectedItems)
                {
                    AddCard(selection as Card, KnownColor.Black);
                }
            }
        }

        private void OnSelectedCardKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                while (mySelectedVcards.SelectedItems.Count > 0)
                {
                    mySelectedVcards.Items.Remove(mySelectedVcards.SelectedItems[0]);
                }
            }
        }

        private void mySelectedVcards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void myImportButton_Click(object sender, RoutedEventArgs e)
        {
            // Browse out to the TXT file
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".txt"; // Default file extension 
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension 
            dlg.CheckFileExists = true;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;

                // Remove filter
                myFilterText.Text = "";
                ApplyFilter();

                // Parse the file one line at a time
                StreamReader reader = new StreamReader(filename);
                while (!reader.EndOfStream)
                {
                    String card = reader.ReadLine();

                    AddHolodeckCard(card, KnownColor.Black, "");
                }
            }
        }


        void AddHolodeckCard(String card, KnownColor border, String sideText)
        {
            card = card.Replace("(1 starting)", "");

            if (card.Contains("/"))
            {
                String[] cardNames = card.Split(new char[] { '/' });
                if (cardNames.Length > 1)
                {
                    AddHolodeckCard(cardNames[1], border, sideText);
                }
            }

            int numberOfCard = 1;

            int multipleIndex = card.IndexOf("(x");
            if ((multipleIndex != -1) && (multipleIndex + 3 < card.Length))
            {
                int copies = 0;
                String count = card.Substring(multipleIndex + 2, 1);
                if (Int32.TryParse(count, out copies))
                {
                    numberOfCard = copies;
                }
                card = card.Substring(0, multipleIndex);
            }

            int iLongestMatch = -1;
            Card bestMatch = null;
            String trimmedCard = removeNonAscii(card);
            foreach (Card dbCard in myFilteredVcards.Items)
            {
                String dbTrimmedCard = removeNonAscii(dbCard.CardName);
                int maxLength = Math.Min(dbTrimmedCard.Length, trimmedCard.Length);

                if (dbTrimmedCard.Substring(0, maxLength).ToLower() == trimmedCard.Substring(0, maxLength).ToLower())
                {
                    if (maxLength >= iLongestMatch)
                    {
                        iLongestMatch = maxLength;
                        bestMatch = dbCard;

                        if (dbTrimmedCard.Length == trimmedCard.Length)
                        {
                            // Perfect match!
                            break;
                        }
                    }
                }
            }


            /*
            string bestMatchString = "";
            if (bestMatch != null)
            {
                bestMatchString = bestMatch.CardName.Replace("(light)", "");
                bestMatchString = bestMatch.CardName.Replace("(Light)", "");
                bestMatchString = bestMatch.CardName.Replace("(LIGHT)", "");
                bestMatchString = bestMatch.CardName.Replace("(LS)", "");
                bestMatchString = bestMatch.CardName.Replace("(ls)", "");
                bestMatchString = bestMatch.CardName.Replace("(l)", "");
                bestMatchString = bestMatch.CardName.Replace("(L)", "");
                bestMatchString = bestMatch.CardName.Replace("(Dark)", "");
                bestMatchString = bestMatch.CardName.Replace("(dark)", "");
                bestMatchString = bestMatch.CardName.Replace("(DARK)", "");
                bestMatchString = bestMatch.CardName.Replace("(DS)", "");
                bestMatchString = bestMatch.CardName.Replace("(ds)", "");
                bestMatchString = bestMatch.CardName.Replace("(D)", "");
                bestMatchString = bestMatch.CardName.Replace("(d)", "");
            }
             * */

            if ( (bestMatch != null) && 
                 (iLongestMatch > 0) &&
                 (
                    //(bestMatch.CardName.ToLower().Contains("(light)")) ||
                    //(bestMatch.CardName.ToLower().Contains("(dark)")) ||
                    ( ( (double)iLongestMatch) / ((double)(removeNonAscii(bestMatch.CardName).Length)) > 0.90) // must be 90 percent correct
                 )
               )
            {
                // Found the matching card!
                for (int i = 0; i < numberOfCard; i++)
                {
                    Card copy = bestMatch.Copy();
                    copy.SideText = sideText;
                    AddCard(copy, border);
                }
            }
            else
            {
                StreamWriter file = new StreamWriter(@"UnknownHolodeckCards.txt", true);
                file.WriteLine(card);
                file.Close();
            }
            /* Old method
            String trimmedCard = removeNonAscii(card);
            foreach (Card dbCard in myFilteredVcards.Items)
            {
                String dbTrimmedCard = removeNonAscii(dbCard.CardName);
                int maxLength = Math.Min(dbTrimmedCard.Length, trimmedCard.Length);

                if (dbTrimmedCard == trimmedCard.Substring(0, maxLength))
                {
                    // Found the matching card!
                    for (int i = 0; i < numberOfCard; i++)
                    {
                        AddCard(dbCard.Copy(), false);
                    }
                }
            }
             */
        }

        private void myExportListButton_Click(object sender, RoutedEventArgs e)
        {
            String fileName = "";
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Cardlist_" + DateTime.Now.ToString("yyyy-MMM-dd_hh-mm"); // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "TXT documents (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                fileName = dlg.FileName;

                StreamWriter exportFile = new StreamWriter(fileName);
                foreach (var card in mySelectedVcards.Items)
                {
                    exportFile.WriteLine((card as Card).CardName + " BORDER:" + (card as Card).BorderColor + " <SIDETEXT>" + (card as Card).SideText + "</SIDETEXT>");
                }
                exportFile.Close();
            }
        }



        private void myImportListButton_Click(object sender, RoutedEventArgs e)
        {
            // Browse out to the TXT file
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".txt"; // Default file extension 
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension 
            dlg.CheckFileExists = true;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                myFilterText.Text = "";
                ApplyFilter();
                mySelectedVcards.Items.Clear();

                // Open document
                string filename = dlg.FileName;

                StreamReader reader = new StreamReader(filename);
                string card = reader.ReadLine();
                string sideText = "";
                while (card != null)
                {
                    System.Drawing.KnownColor border = KnownColor.Black;


                    if (card.Contains("<SIDETEXT>"))
                    {
                        int indexOfSideText = card.IndexOf("<SIDETEXT>");
                        int iEndOfBracket = indexOfSideText + 10;
                        int closingSideText = card.IndexOf("</SIDETEXT>", indexOfSideText);

                        sideText = card.Substring(iEndOfBracket, closingSideText - iEndOfBracket);
                    }

                    if (card.Contains("BORDER:"))
                    {
                        int indexOfBorder = card.IndexOf("BORDER:");
                        int iEndOfBorder = indexOfBorder + 7;
                        int spaceAfterBorder = card.IndexOf(" ", indexOfBorder);

                        if (spaceAfterBorder != -1)
                        {
                            string borderColor = card.Substring(iEndOfBorder, spaceAfterBorder - iEndOfBorder);
                            card = card.Substring(0, indexOfBorder - 1);

                            border = (KnownColor)System.Enum.Parse(typeof(System.Drawing.KnownColor), borderColor, true);
                        }
                    }

                    
                    AddHolodeckCard(card, border, sideText);
                    card = reader.ReadLine();
                }
            }
        }

        private void myOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            OptionsDialog dlg = new OptionsDialog(AlternateColor, HorizontalSpacing, BackgroundMatchesFirstCard, StartNumberingAt, ShowCardNumbers);
            dlg.ShowDialog();

            if (dlg.DialogResult == true)
            {
                AlternateColor = dlg.AlternateColor;
                BackgroundMatchesFirstCard = dlg.BackgroundMatchesFirstColor;
                StartNumberingAt = dlg.StartNumberingAt;
                HorizontalSpacing = dlg.HorizontalSpacing;
                ShowCardNumbers = dlg.ShowNumbers;

                myMoveWhiteBorderRightButton.Content = "Add " + AlternateColor;
            }
        }

        private void OnDeleteItemCommand(object sender, RoutedEventArgs e)
        {
            MoveCardsLeft();
        }

        private void OnSwitchBorderCommand(object sender, RoutedEventArgs e)
        {
            foreach (var item in mySelectedVcards.SelectedItems)
            {
                Card card = item as Card;
                card.BorderColor = AlternateColor;
            }
        }

        private void OnAddEditText(object sender, RoutedEventArgs e)
        {
            if (mySelectedVcards.SelectedItems.Count == 1)
            {
                Card card = mySelectedVcards.SelectedItem as Card;
                AddEditSideText textDlg = new AddEditSideText(card);
                textDlg.ShowDialog();
            }
            else
            {
                MessageBox.Show("This only works with a single card selected.");
            }
        }

        private void myExitMenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void myInstructionMenu_Click(object sender, RoutedEventArgs e)
        {
            Instructions dlg = new Instructions();
            dlg.ShowDialog();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void myStandardFormatMenuClick(object sender, RoutedEventArgs e)
        {
            myCubeFormatMenu.IsChecked = false;
            myLegacyFormatMenu.IsChecked = false;
            myStandardFormatMenu.IsChecked = true;
            LoadNewFormat("standard");
        }

        private void myLegacyFormatMenuClick(object sender, RoutedEventArgs e)
        {
            myCubeFormatMenu.IsChecked = false;
            myLegacyFormatMenu.IsChecked = true;
            myStandardFormatMenu.IsChecked = false;
            LoadNewFormat("legacy");
        }

        private void myCubeFormatMenuClick(object sender, RoutedEventArgs e)
        {
            myCubeFormatMenu.IsChecked = true;
            myLegacyFormatMenu.IsChecked = false;
            myStandardFormatMenu.IsChecked = false;
            LoadNewFormat("cube");
        }
    }


    public class Card : System.ComponentModel.INotifyPropertyChanged
    {
        public String CardNameWithBorder
        {
            get
            {
                return CardName + " (" + BorderColor + ")";
            }
        }
        public String CardName { get; set; }
        public String Path { get; set; }
        public int CardNumber { get; set; }
        public String SideText { get; set; }

        private System.Drawing.KnownColor myBorderColor;
        public System.Drawing.KnownColor BorderColor 
        { 
            get
            {
                return myBorderColor;
            }

            set
            {
                myBorderColor = value;
                NotifyPropertyChanged("BorderColor");
                NotifyPropertyChanged("CardNameWithBorder");
            }
        }

        
        public Card(String name, String path, System.Drawing.KnownColor borderColor, String sideText)
        {
            CardName = name;
            Path = path;
            BorderColor = borderColor;
            SideText = sideText;
        }

        public Card Copy()
        {
            return new Card(CardName, Path, BorderColor, SideText);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    };
}

