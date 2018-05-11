using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace AIProject
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    //public partial class MainWindow : Window
    //{
    //    private Client client;
    //
    //    public MainWindow()
    //    {
    //        InitializeComponent();
    //        client = new Client("127.0.0.1", 808);
    //        client.Run();
    //    }
    //
    //    private void Button_Click(object sender, RoutedEventArgs e)
    //    {
    //        client.Run();
    //        client.StartUploading("Start");
    //    }
    //}
    public partial class MainWindow : Window
    {
        private Client client;
        private Proxy proxy;
        private string filepath = string.Empty;
        public string FilePath { get => filepath; set { filepath = value; UpdatePreview(); } }
        private string userName;

        private bool isProcessingOnline;

        public MainWindow()
        {
            InitializeComponent();
            client = new Client("127.0.0.1", 808);
            //тут надо посмотреть на каком порту запускается сервер
            proxy = new Proxy("http://localhost:60950/api/");
            //client.Run();


            UIAuthView.Visibility = Visibility.Visible;
            UIRegView.Visibility = Visibility.Collapsed;
        }

        private void Button_SelectedFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP4 files (*.mp4)|*.mp4|AVI files (*.avi)|*.avi";

            //If you need get File Path
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
            }
        }

        private void Button_Drop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                FilePath = files[0]; // You URL Drop file
            }
        }

        private void ProcessTypeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;
            isProcessingOnline = (bool)button.IsChecked;
            if (isProcessingOnline)
            {
                UIProcessingButton.Content = "Upload video";
            } 
            else
            {
                UIProcessingButton.Content = "Process video";
            }
        }

        private void ProcessingButton_Click(object sender, RoutedEventArgs e)
        {
            if (isProcessingOnline)
            {
                if (FilePath != string.Empty)
                {
                    client.StartUploading(FilePath);
                    client.Run();
                }

                var fileInfo = new FileInfo(FilePath);
                while (client.path != String.Empty)
                {

                }
                proxy.FinishFileUploading(userName, fileInfo.Name, CalculateMD5(fileInfo.FullName));
            }
            else
            {
                MessageBox.Show("Sorry", "Offline processing is not available now");
            }
        }

        private void Button_SaveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string downloadURL = proxy.GetLastUserFile(userName);
                string savePath = $@".\Files\{downloadURL.Split('\\').Last()}";
                WebClient wc = new WebClient();

                wc.DownloadFile(downloadURL, savePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        /// Helpers

        private void UpdatePreview()
        {
            if (FilePath != string.Empty)
            {
                ShellFile shellFile = ShellFile.FromFilePath(FilePath);
                Bitmap shellThumb = shellFile.Thumbnail.ExtraLargeBitmap;

                UIPlusTextView.Visibility = Visibility.Collapsed;
                UIImageView.Visibility = Visibility.Visible;
                UIImageView.Source = BitmapToImageSource(shellThumb);
            }
            else
            {
                UIImageView.Visibility = Visibility.Collapsed;
                UIPlusTextView.Visibility = Visibility.Visible;
                UIImageView.Source = null;
            }
        }


        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        
        
        
        // AUTHORIZATION

        private void ButtonAuth_Click(object sender, RoutedEventArgs e)
        {
            bool isAuth = false;
            isAuth = (UIPasswordBox.Password.Length > 0) || (UIUsernameTextBox.Text.Length > 0);
            //AuthMethod
            //Тыры-пыры тут короче код будет
            //isAuth = proxy.Login(UIUsernameTextBox.Text, UIPasswordBox.Password);
            if (isAuth)
            {
                UIAuthView.Visibility = Visibility.Collapsed;
                userName = UIUsernameTextBox.Text;
            }
            else
            {
                MessageBox.Show("Fields are empty", "Error");
            }
        }

        private void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            UIRegView.Visibility = Visibility.Visible;
        }



        
        // REGISTRATION

        private void RegButtonGoBack_Click(object sender, RoutedEventArgs e)
        {
            UIRegView.Visibility = Visibility.Collapsed;
        }

        private void RegButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            string email = UIRegEmailTextBox.Text;
            string username = UIRegUsernameTextBox.Text;
            string password = UIRegPasswordBox.Password;

            if (email.Length > 0 || username.Length > 0 || password.Length > 0)
            {
                bool regSuccess;
                //regSuccess = /*Тут нужно зарегаться*/
                regSuccess = true;

                if (regSuccess)
                {
                    UIRegView.Visibility = Visibility.Collapsed;
                    UIAuthView.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Unable to register with this details", "Registration error");
                }
            }
            else
            {
                MessageBox.Show("Fields are empty", "Error");
            }
        }
    }

}
