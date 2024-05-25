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

namespace wpfCopilator.ManualPages
{
    /// <summary>
    /// Логика взаимодействия для PageMenuEdit.xaml
    /// </summary>
    public partial class PageMenuEdit : Page
    {
        public PageMenuEdit()
        {
            InitializeComponent();

            ContentLabel.Text = @"
В пункте меню ""Правка представлены стандартные инструменты для работы с текстом.""";
        }
    }
}
