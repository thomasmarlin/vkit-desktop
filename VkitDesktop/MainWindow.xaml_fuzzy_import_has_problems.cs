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

        public MainWindow()
        {
            InitializeComponent();

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


        private List<Card> GetSelectedCards(bool smallCards, bool bigCards)
        {
            List<Card> selectedCards = new List<Card>();
            foreach (var selection in mySelectedVcards.Items)
            {
                Card card = selection as Card;
                if (card != null)
                {


                    try
                    {
                        // See if the card is taller than 400 pixels
                        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(card.Path);
                        if ((smallCards && bmp.Height < 400) ||
                            (bigCards && bmp.Height >= 400))
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


            using (WaitDlg dlg = new WaitDlg())
            {

                Document document = new Document(PageSize.LETTER);

                //document.SetMargins(0, 0, 10, 0);
                // WORKING! document.SetMargins(document.LeftMargin, document.RightMargin, 25, 25);
                document.SetMargins(document.LeftMargin, document.RightMargin, 15, 10);


                PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.Create));


                // step 3: we open the document
                document.Open();



                iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph();
                PdfPTable table = new pdf.PdfPTable(3);
                table.SpacingBefore = 0;
                table.SpacingAfter = 10;
                table.TotalWidth = 175.5f * 3.0f + 30f;
                table.LockedWidth = true;


                List<Card> smallCards = GetSelectedCards(true, false);
                List<Card> bigCards = GetSelectedCards(false, true);


                WriteCards(bigCards, table);
                WriteCards(smallCards, table);
                

                table.AddCell(new PdfPCell());
                table.AddCell(new PdfPCell());
                table.AddCell(new PdfPCell());

                document.Add(table);

                document.Close();
            }

            MessageBox.Show("PDF Generation Complete!");
        }


        private void WriteCards(List<Card> cards, PdfPTable table)
        {
            foreach (var imageFile in cards)
            {
                if (File.Exists(imageFile.Path))
                {
                    System.Drawing.Bitmap image = new System.Drawing.Bitmap(imageFile.Path);

                    if (imageFile.WhiteBorder)
                    {
                        image = WhiteBorderConverter.ToWhiteBorder(image);
                    }
                    else
                    {
                        image = WhiteBorderConverter.CreateNonIndexedImage(image);
                    }

                    iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(image, System.Drawing.Imaging.ImageFormat.Png);

                    float scalePercent = 100.0f * 179.0f / (pic.Width);

                    //pic.ScalePercent(51.0f);
                    pic.ScalePercent(scalePercent);

                    PdfPCell cell = new PdfPCell(pic);
                    //cell.FixedHeight = pic.ScaledHeight;
                    cell.FollowingIndent = 0;
                    cell.Indent = 0;
                    cell.Padding = 0;
                    cell.SetLeading(0, 0);
                    cell.BorderColor = iTextSharp.text.BaseColor.WHITE;

                    table.AddCell(cell);
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

            //StreamWriter missing = new StreamWriter("missingImageFiles.txt");
            String assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            String installFolder = Path.GetDirectoryName(assemblyLocation);
            DirectoryInfo info = new DirectoryInfo(String.Format("{0}\\vcards", installFolder));
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
                    myFilteredVcards.Items.Add(new Card(cardName, card, false));
                }
            }
        }

        private void myMoveRightButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var selection in myFilteredVcards.SelectedItems)
            {
                Card card = selection as Card;
                AddCard(card, false);
            }
        }

        private void myAddWhiteBorderButtonClick(object sender, RoutedEventArgs e)
        {
            foreach (var selection in myFilteredVcards.SelectedItems)
            {
                Card card = selection as Card;
                AddCard(card, true);
            }
        }

        private void AddCard(Card card, bool whiteBorder)
        {
            Card copy = card.Copy();
            copy.WhiteBorder = whiteBorder;
            mySelectedVcards.Items.Add(copy);
        }

        private void myMoveLeftButton_Click(object sender, RoutedEventArgs e)
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
                    AddCard(selectedItem as Card, false);
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
                    AddCard(selection as Card, false);
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

                    AddHolodeckCard(card);
                }
            }
        }


        void AddHolodeckCard(String card)
        {
            card = card.Replace("(1 starting)", "");

            if (card.Contains("/"))
            {
                String[] cardNames = card.Split(new char[] { '/' });
                if (cardNames.Length > 1)
                {
                    AddHolodeckCard(cardNames[1]);
                }
            }

            int numberOfCard = 1;

            // Look for (xValue) or (XValue)
            int multipleIndex = card.IndexOf("(x");
            if (multipleIndex == -1) { multipleIndex = card.IndexOf("(X"); }

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


            int iShortestDistance = int.MaxValue;
            Card bestMatch = null;
            bool trimLightDark = false;


            // Unless original card had "(Light)" or "(Dark)", don't compare those names
            string trimmedCard = card;
            if (!card.Contains("(Light)") && !card.Contains("(Dark)"))
            {
                trimLightDark = true;
            }
            trimmedCard = removeNonAscii(trimmedCard);

            foreach (Card dbCard in myFilteredVcards.Items)
            {
                String dbTrimmedCard = dbCard.CardName;
                if (trimLightDark)
                {
                    dbTrimmedCard = dbTrimmedCard.Replace("(Dark)", "").Replace("(Light)", "");
                }

                dbTrimmedCard = removeNonAscii(dbTrimmedCard);
                

                int iDistance = StringCompare.CalcDistance(dbTrimmedCard, trimmedCard);
                if (iDistance <= iShortestDistance)
                {
                    iShortestDistance = iDistance;
                    bestMatch = dbCard;
                }
            }


            if ((bestMatch != null) && (iShortestDistance <= 4))
            {
                // Found the matching card!
                for (int i = 0; i < numberOfCard; i++)
                {
                    AddCard(bestMatch.Copy(), false);
                }
            }
            else
            {
                StreamWriter file = new StreamWriter(@"UnknownHolodeckCards.txt", true);
                file.WriteLine(card);
                file.Close();
            }
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
                    exportFile.WriteLine((card as Card).CardName);
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
                while (card != null)
                {
                    AddHolodeckCard(card);
                    card = reader.ReadLine();
                }
            }
        }
    }


    public class Card
    {
        public String CardNameWithBorder
        {
            get
            {
                return WhiteBorder ? CardName+" (WB)" : CardName+" (BB)";
            }
        }
        public String CardName { get; set; }
        public String Path { get; set; }
        public bool WhiteBorder { get; set; }
        public Card(String name, String path, bool whiteBorder)
        {
            CardName = name;
            Path = path;
            WhiteBorder = whiteBorder;
        }

        public Card Copy()
        {
            return new Card(CardName, Path, WhiteBorder);
        }
    };
}
