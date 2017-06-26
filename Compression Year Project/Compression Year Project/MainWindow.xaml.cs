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
            int[] ls = new int[3] { 1, 2, 1 };
            ActivationFunction[] af = new ActivationFunction[3] { ActivationFunction.None,
                                                                ActivationFunction.RationalSig,
                                                                ActivationFunction.Linear };
            BackPropNetwork bp = new BackPropNetwork(ls,af);

            double[] input = new double[1] { 1.0};
            double[] desired = new double[1] { 2.5 };
            double[] output = new double[1];

            double error = 0.0;

            for(int i = 0; i < 1000; i++)
            {
                error = bp.train(ref input, ref desired, 0.15, 0.1);
                bp.run(ref input, out output);

                if (i % 100 == 0)
                {
                 txtMain.Text += "Iteration " + i + ":\n\tInput " + Math.Round(input[0],3) + " Output " + Math.Round(output[0],3) + " Error " + Math.Round(error,3) + '\n';
                }
            }

           bp.Save("../Ann.xml");

            BackPropNetwork bp2 = new BackPropNetwork("../Ann.xml");

            bp2.run(ref input, out output);

            txtMain.Text += "Input " + Math.Round(input[0], 3) + " Output " + Math.Round(output[0], 3);


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
