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
    /// Логика взаимодействия для PagePanelTool.xaml
    /// </summary>
    public partial class PagePanelTool : Page
    {
        public PagePanelTool()
        {
            InitializeComponent();
            ContentLabel.Text = @"
    1. Создать файл
 - При нажатии на кнопку ""Создать"" появится вспомогательное окно для ввода названия файла и последующее создание и открытие нового файла.
    2. Открыть файл
    3. Сохранение файла
    4. Отменить
 - Возвращение к предыдущему шагу.
    5. Вернуть
 - Переход на шаг вперед.
    6. Копировать
    7. Вставить
    8. Вырезать
    9. Запуск
    10. Вызов справки
    11. О программе
";

        }
    }
}
