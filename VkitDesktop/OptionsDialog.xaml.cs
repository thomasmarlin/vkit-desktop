using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VkitDesktop
{
    /// <summary>
    /// Interaction logic for OptionsDialog.xaml
    /// </summary>
    public partial class OptionsDialog : Window
    {

        public System.Drawing.KnownColor AlternateColor { get; set; }
        public double HorizontalSpacing { get; set; }
        public bool BackgroundMatchesFirstColor { get; set; }
        public int StartNumberingAt { get; set; }
        public bool ShowNumbers { get; set; }


        /// <summary>
        /// DO NOT USE. DESIGNER ONLY!!
        /// </summary>
        public OptionsDialog()
        {
            InitializeComponent();
        }

        public OptionsDialog(System.Drawing.KnownColor color, double spacing, bool backgroundMatchesCardColor, int startNumberingAt, bool showNumbers)
        {
            InitializeComponent();
            AlternateColor = color;
            HorizontalSpacing = spacing;
            BackgroundMatchesFirstColor = backgroundMatchesCardColor;
            ShowNumbers = showNumbers;

            myBackgroundMatchesFirstCardCheck.IsChecked = BackgroundMatchesFirstColor;
            mySpacingText.Text = HorizontalSpacing.ToString();
            myStartNumberingAtText.Text = "" + StartNumberingAt;
            myUsePageNumberCheck.IsChecked = ShowNumbers;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<System.Drawing.KnownColor> colors = SupportedColors.GetSupportedColors();
            foreach (var knownColor in colors)
            {
                RadioButton radio = new RadioButton();
                radio.Content = knownColor.ToString();
                radio.Tag = knownColor;

                if (knownColor == AlternateColor)
                {
                    radio.IsChecked = true;
                }

                System.Drawing.Color color = System.Drawing.Color.FromKnownColor(knownColor);
                System.Windows.Media.Color mediaColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
                myColorPanel.Children.Add(radio);

                Rectangle colorSwatch = new Rectangle();
                colorSwatch.Height = 16;
                colorSwatch.Width = 100;
                colorSwatch.Fill = new SolidColorBrush(mediaColor);
                myColorSwatchPanel.Children.Add(colorSwatch);
            }
        }

        private void myCancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void myOkButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in myColorPanel.Children)
            {
                if (child is RadioButton)
                {
                    RadioButton radio = child as RadioButton;
                    if (radio.IsChecked == true)
                    {
                        AlternateColor = (System.Drawing.KnownColor)radio.Tag;
                    }
                }
            }

            try
            {
                HorizontalSpacing = double.Parse(mySpacingText.Text);
                StartNumberingAt = int.Parse(myStartNumberingAtText.Text);
                ShowNumbers = (bool)myUsePageNumberCheck.IsChecked;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid values. Please make sure to enter an integer value for spacing and numbering.");
                return;
            }


            BackgroundMatchesFirstColor = (bool)myBackgroundMatchesFirstCardCheck.IsChecked;
            DialogResult = true;
        }
    }
}
