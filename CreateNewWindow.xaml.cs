using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace wpfCopilator
{
    /// <summary>
    /// Логика взаимодействия для CreateNewWindow.xaml
    /// </summary>
    public partial class CreateNewWindow : Window
    {
        public string FileName { get; set; }
        public CreateNewWindow()
        {
            InitializeComponent();

        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if(!Directory.Exists("tempFilesDirectory"))
            {
                Directory.CreateDirectory("tempFilesDirectory");
            }
            FileName = myTextBox.Text;
            File.Create(Environment.CurrentDirectory + "//" + "tempFilesDirectory//" + FileName + ".txt");
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
