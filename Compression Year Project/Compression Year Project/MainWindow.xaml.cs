﻿using System;
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
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO.Compression;

namespace Compression_Year_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NormaliseText norma;
        NormaliseImage normaImage;
        long compressedLength;
        long orignaLength;
        string fileType;
        public MainWindow()
        {
            InitializeComponent();

        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            txtMain.Foreground = System.Windows.Media.Brushes.Black;
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "All Usable Files (*.mp3;*.jpg;*txt;*.mp4)|*.mp3;*.jpg;*.txt;*.mp4|Audio Files (*.mp3)|*.mp3|Image Files (*.jpg)|*.jpg|Text Files (*.txt)|*.txt|Video Files (*.mp4)|*.mp4";
            if (opf.ShowDialog() == true)
            {
                txtMain.Text += "The file loaded is " + System.IO.Path.GetFileName(opf.FileName) + "\n";

                fileType = System.IO.Path.GetExtension(opf.FileName);

                switch (fileType)
                {
                    case ".txt":
                        norma = new NormaliseText(opf.FileName);
                        norma.saveToXML();
                        break;
                    case ".mp4":
                        break;
                    case ".mp3":
                        break;
                    case ".jpg":
                        normaImage = new NormaliseImage(opf.FileName);
                        normaImage.saveToXML();
                        break;
                    case ".JPG":
                        normaImage = new NormaliseImage(opf.FileName);
                        normaImage.saveToXML();
                        break;
                }
                orignaLength = new System.IO.FileInfo(opf.FileName).Length;
            }
        }


        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Canvas_PreviewDrop(object sender, DragEventArgs e)
        {

            txtMain.Foreground = System.Windows.Media.Brushes.Black;
            string fileName = System.IO.Path.GetFullPath((string)((DataObject)e.Data).GetFileDropList()[0]);
            txtMain.Text += "The file loaded is " + System.IO.Path.GetFileName((string)((DataObject)e.Data).GetFileDropList()[0]) + "\n";
            fileType = System.IO.Path.GetExtension((string)((DataObject)e.Data).GetFileDropList()[0]);
            switch (fileType)
            {
                case ".txt":
                    norma = new NormaliseText(fileName);
                    norma.saveToXML();
                    break;
                case ".mp4":
                    break;
                case ".mp3":
                    break;
                case ".jpg":
                    normaImage = new NormaliseImage(fileName);
                    normaImage.saveToXML();
                    break;
                case ".JPG":
                    normaImage = new NormaliseImage(fileName);
                    normaImage.saveToXML();
                    break;
            }
            orignaLength = new System.IO.FileInfo(fileName).Length;
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
                case ".mp3":
                    check = true;
                    break;
                case ".jpg":
                    check = true;
                    break;
                case ".JPG":
                    check = true;
                    break;
                case ".cmx":
                    check = true;
                    break;
                case ".cmi":
                    check = true;
                    break;
                case ".cma":
                    check = true;
                    break;
                default:
                    check = false;
                    break;
            }

            if (!check)
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
            if (norma == null && normaImage == null)
            {
                txtMain.Foreground = System.Windows.Media.Brushes.Red;
                txtMain.Text += "No file was loaded \n";
            }
            else
            {

                txtMain.Foreground = System.Windows.Media.Brushes.Black;

                LoadingIndicator.IsBusy = true;

                LoadingIndicator.BusyContent = "Compressing Data";

                switch (fileType)
                {
                    case ".txt":
                        Task.Factory.StartNew(() =>
                                {
                                    CompressText cmp = new CompressText();
                                    return cmp;
                                }
                        ).ContinueWith((task) =>
                        {

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
                                compressedLength = new System.IO.FileInfo(dlg.FileName).Length;
                            }

                            txtMain.Text += "File saved" + '\n';
                        }, TaskScheduler.FromCurrentSynchronizationContext()

                       );

                        break;
                    case ".mp3":
                        break;
                    case ".jpg":

                        break;
                    case ".JPG":
                        Task.Factory.StartNew(() =>
                        {
                            ImageCompress cmp = new ImageCompress();
                            return cmp;
                        }
               ).ContinueWith((task) =>
               {

                   LoadingIndicator.IsBusy = false;
                   task.Result.compressFile(normaImage);
                   Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                   dlg.FileName = "CImage"; //default file name
                   dlg.DefaultExt = ".cmi"; //default file extension
                   dlg.Filter = "Compressed Images (*.cmi)|*.cmi";

                   // Show save file dialog box
                   Nullable<bool> result = dlg.ShowDialog();

                   byte[] compressedImagefile = SerializeAndCompress(task.Result._ci);
                   if (result == true)
                   {


                       //serialize


                       File.WriteAllBytes(dlg.FileName, compressedImagefile);
                       compressedLength = new System.IO.FileInfo(dlg.FileName).Length;
                       //  task.Result._ci.Save(dlg.FileName,jpgEncoder,myEncoderParameters);
                   }

                   txtMain.Text += "File saved" + '\n';
               }, TaskScheduler.FromCurrentSynchronizationContext()

                );
                        break;
                }
            }
        }
        private void Extract_Click(object sender, RoutedEventArgs e)
        {

            txtMain.Foreground = System.Windows.Media.Brushes.Black;
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Compressed Files (*.cmx;*.cmi)|*.cmx;*.cmi";


            if (opf.ShowDialog() == true)
            {
                fileType = System.IO.Path.GetExtension(opf.FileName);
                switch (fileType)
                {
                    case ".cmi":
                        CImage cimage = DecompressAndDeserialize<CImage>(File.ReadAllBytes(opf.FileName));
                        ExtractImage extI = new ExtractImage(cimage);

                        Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                        dlg.FileName = "Original Image"; //default file name
                        dlg.DefaultExt = ".jpg"; //default file extension
                        dlg.Filter = "Image File (*.jpg)|*.jpg"; //filter files by extension

                        // Show save file dialog box
                        Nullable<bool> result = dlg.ShowDialog();

                        if (result == true)
                        {

                            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                            //Create an Encoder object based on the GUID
                            // for the Quality parameter category.  
                            System.Drawing.Imaging.Encoder myEncoder =
                            System.Drawing.Imaging.Encoder.Quality;

                            // Create an EncoderParameters object.  
                            // An EncoderParameters object has an array of EncoderParameter  
                            // objects. In this case, there is only one  
                            // EncoderParameter object in the array.  
                            EncoderParameters myEncoderParameters = new EncoderParameters(1);
                            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 90L);
                            myEncoderParameters.Param[0] = myEncoderParameter;

                            extI.extract().Save(dlg.FileName,jpgEncoder,myEncoderParameters);
                            txtMain.Text += "The file has been saved \n";
                        }
                        break;
                    case ".cmx":

                        ExtractText ext = new ExtractText(File.ReadAllBytes(opf.FileName));

                        Microsoft.Win32.SaveFileDialog dlg1 = new Microsoft.Win32.SaveFileDialog();
                        dlg1.FileName = "Original Text"; //default file name
                        dlg1.DefaultExt = ".txt"; //default file extension
                        dlg1.Filter = "Text File (*.txt)|*.txt"; //filter files by extension

                        // Show save file dialog box
                        Nullable<bool> result1 = dlg1.ShowDialog();

                        if (result1 == true)
                        {
                            File.WriteAllText(dlg1.FileName, ext.extract());

                            txtMain.Text += "The file has been saved \n";
                        }
                        break;
                }
            }
        }
        // Code adapted from http://www.morgantechspace.com/2013/08/convert-object-to-byte-array-and-vice.html
        //private byte[] ObjectToByteArray(Object obj)
        //{
        //    if (obj == null)
        //        return null;

        //    BinaryFormatter bf = new BinaryFormatter();
        //    MemoryStream ms = new MemoryStream();
        //    bf.Serialize(ms, obj);

        //    return ms.ToArray();
        //}

        //// Convert a byte array to an Object
        //private Object ByteArrayToObject(byte[] arrBytes)
        //{
        //    MemoryStream memStream = new MemoryStream();
        //    BinaryFormatter binForm = new BinaryFormatter();
        //    memStream.Write(arrBytes, 0, arrBytes.Length);
        //    memStream.Seek(0, SeekOrigin.Begin);
        //    Object obj = (Object)binForm.Deserialize(memStream);

        //    return obj;
        //}
        private void Results_Click(object sender, RoutedEventArgs e)
        {
            Results results = new Results(orignaLength, compressedLength);
            txtMain.Text += results.ShowRatio() + "\n";
            txtMain.Text += results.ShowSavedData() + "\n";
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        //code adapted from https://stackoverflow.com/questions/23651650/is-there-a-way-to-compress-an-object-in-memory-and-use-it-transparently
        private static byte[] SerializeAndCompress(object obj)
        {
            MemoryStream ms = new MemoryStream();
            using (GZipStream zs = new GZipStream(ms, CompressionMode.Compress, true))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(zs, obj);
            }            
            return ms.ToArray();
        }
        public static T DecompressAndDeserialize<T>(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (GZipStream zs = new GZipStream(ms, CompressionMode.Decompress, true))
            {
                BinaryFormatter bf = new BinaryFormatter();
                ms.Position = 0;
                return (T)bf.Deserialize(zs);
            }
        }
    }
    }
