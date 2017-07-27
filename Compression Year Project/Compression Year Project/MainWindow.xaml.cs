using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using ANeuralNetwork;
using System.Xml;
namespace Compression_Year_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Normalise norma;
        public MainWindow()
        {
            InitializeComponent();    
           
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "All Usable Files (*.mp3;*.jpg;*txt;*.mp4)|*.mp3;*.jpg;*.txt;*.mp4|Audio Files (*.mp3)|*.mp3|Image Files (*.jpg)|*.jpg|Text Files (*.txt)|*.txt|Video Files (*.mp4)|*.mp4";
            if(opf.ShowDialog() == true)
            {
                txtMain.Text += "The file loaded is "+System.IO.Path.GetFileName(opf.FileName) + "\n";
                norma = new Normalise(opf.FileName);
                norma.saveToXML();
                
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Canvas_PreviewDrop(object sender, DragEventArgs e)
        {
            txtMain.Text += "The file loaded is "+ System.IO.Path.GetFileName((string)((DataObject)e.Data).GetFileDropList()[0]) + "\n";
        }

        private void Canvas_PreviewDragOver(object sender, DragEventArgs e)
        {
            bool check = false;
            FileInfo f = new FileInfo((string)((DataObject)e.Data).GetFileDropList()[0]);
            
            switch (f.Extension)
            {
                case ".txt":
                    check = true;
                    break;
                case ".mp4":
                    check = true;
                    break;
                case ".mp3":
                    check = true;
                    break;
                case ".jpg":
                    check = true;
                    break;
                case ".JPG":
                    check = true;
                    break;
                default:
                    check = false;
                 break;
            }

            if(!check)
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.All;
            }
            e.Handled = true;

        }

        private void Compress_Click(object sender, RoutedEventArgs e)
        {
            LoadingIndicator.IsBusy = true;

            LoadingIndicator.BusyContent = "Compressing Data";
            
            Task.Factory.StartNew(() =>
                 {
                     Compress cmp = new Compress();
                     return cmp;
                 }
             ).ContinueWith((task) =>{

                 LoadingIndicator.IsBusy = false;
                 task.Result.compressFile(norma);

                 Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                 dlg.FileName = "CompressedText"; //default file name
                 dlg.DefaultExt = ".cmx"; //default file extension
                 dlg.Filter = "Compressed Files (.cmx)|*.cmx"; //filter files by extension

                 // Show save file dialog box
                 Nullable<bool> result = dlg.ShowDialog();

                 byte[] compressedfile = SmazSharp.Smaz.Compress(task.Result._compressedString);
                 if (result == true)
                 {
                     File.WriteAllBytes(dlg.FileName, compressedfile);
                 }

                 txtMain.Text += "File saved" + '\n';
             }, TaskScheduler.FromCurrentSynchronizationContext()

            );
            
            
        }

        private void Extract_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Compressed FIles (*.cmx)|*.cmx";

            if (opf.ShowDialog() == true)
            {
               txtMain.Text += "The file loaded is " + System.IO.Path.GetFileName(opf.FileName) + "\n";
                        
            
            Extract ext = new Extract(File.ReadAllBytes(opf.FileName));

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Original Text"; //default file name
            dlg.DefaultExt = ".txt"; //default file extension
            dlg.Filter = "Text File (*.txt)|*.txt"; //filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            
            if (result == true)
            {
                File.WriteAllText(dlg.FileName, ext.extract());
            }
            }
        }
    }
}
