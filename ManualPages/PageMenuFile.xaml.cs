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
    /// Логика взаимодействия для PageMenuFile.xaml
    /// </summary>
    public partial class PageMenuFile : Page
    {
        public PageMenuFile()
        {
            InitializeComponent();

            ContentLabel.Text = @"
    Для создания нового файла нажмите сочетание клавиш ctrl+N или в пункте меню ""Файл"" - Создать. После чего появится окно для ввода имени файла, нажмите 'создать' для подтверждения.

    Для открытия ранее созданного файла нажмите ctrl+O или соответствующий подпункт ""Создать"". Выберите в открывшемся окне свой файл.

    Для сохранения файла, нажмите ctrl+S или подпункт - ""Сохранить"", если файл не был сохранен ранее, то вам будет необходимо выбрать место для сохранения. 

    Нажмите ""Для завершения работы программы.""";
        }
    }
}
