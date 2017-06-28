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
using System.IO;
namespace Compression_Year_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load("../test.xml");

            //network to train

            DataSet ds = new DataSet();

            ds.Load((XmlElement)xdoc.DocumentElement.ChildNodes[0]);

            int[] layersizes = new int[3] { 2, 2, 1 };
            ActivationFunction[] activFunctions = new ActivationFunction[3]{ ActivationFunction.None, ActivationFunction.Sigmoid,
                ActivationFunction.Linear };

            BackPropNetwork bpnetwork = new BackPropNetwork(layersizes, activFunctions);

            //network trainer
            NetworkTrainer nt = new NetworkTrainer(bpnetwork, ds);

            nt.maxError = 0.001;
            nt.maxiterations = 1000000;

            nt.TrainDataset();

            nt.bpnetwork.Save("../Check.xml");

            //save error
            double[] err = nt.geteHistory();
            string[] filedata = new string[err.Length];

            for (int i = 0; i <err.Length; i++)
            {
                filedata[i] = i.ToString() + " " + err[i].ToString();
            }

            File.WriteAllLines("../xornetwrk.txt",filedata);           
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "All Usable Files (*.mp3;*.jpg;*txt;*.mp4)|*.mp3;*.jpg;*.txt;*.mp4|Audio Files (*.mp3)|*.mp3|Image Files (*.jpg)|*.jpg|Text Files (*.txt)|*.txt|Video Files (*.mp4)|*.mp4";
            if(opf.ShowDialog() == true)
            {
                txtMain.Text += "The file loaded is "+System.IO.Path.GetFileName(opf.FileName) + "\n";
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

    }
}
