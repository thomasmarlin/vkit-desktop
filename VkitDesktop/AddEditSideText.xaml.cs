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
    /// Interaction logic for AddEditSideText.xaml
    /// </summary>
    public partial class AddEditSideText : Window
    {

        Card myCard;

        /// <summary>
        /// DO NOT USE!  DESIGNER ONLY!
        /// </summary>
        public AddEditSideText()
        {
            InitializeComponent();
        }



        public AddEditSideText(Card card)
        {
            InitializeComponent();
            myCard = card;
            myCardText.Text = card.SideText;
        }

        private void myOkButton_Click(object sender, RoutedEventArgs e)
        {
            myCard.SideText = myCardText.Text;
            Close();
        }

        private void myCancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
