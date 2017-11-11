using System;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Text;

namespace Compression_Year_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        page1 p1;
        Annoverview an;
        public MainWindow()
        {
            InitializeComponent();
            p1 = new page1();
            an = new Annoverview();
            FMain.Content = p1;
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            FMain.Content = p1;
            p1.OpenFile_Click(sender, e);

        }
        private void Compress_Click(object sender, RoutedEventArgs e)
        {
            FMain.Content = p1;
            p1.Compress_Click(sender, e);
        }
        public void Extract_Click(object sender, RoutedEventArgs e)
        {
            FMain.Content = p1;
            p1.Extract_Click(sender, e);
        }
        public void Results_Click(object sender, RoutedEventArgs e)
        {
            FMain.Content = p1;
            p1.Results_Click(sender, e);
        }

        private void NN_Click(object sender, RoutedEventArgs e)
        {
            FMain.Content = an;
        }
    }
}
