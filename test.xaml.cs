using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PlayMusic
{
    /// <summary>
    /// Interaction logic for test.xaml
    /// </summary>
    public partial class test : Window
    {
        private bool isBig = false;
        public test()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isBig)
            {
                TestWin.MinHeight = 300;
                TestWin.Height = 300;
                TestWin.MaxHeight = 300;
                isBig = false;
            }
            else
            {
                TestWin.MaxHeight = int.MaxValue; //Because infinity doesn't work.
                TestWin.Height = 600;
                TestWin.MinHeight = 600;
                
                isBig = true;
            }
        }
    }
}
