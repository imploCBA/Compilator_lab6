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
using System.Windows.Shapes;
using wpfCopilator.ManualPages;

namespace wpfCopilator
{
    /// <summary>
    /// Логика взаимодействия для Manual.xaml
    /// </summary>
    public partial class Manual : Window
    {
        private Button? _previousButton = null;
        private List<Page> _pages = new List<Page>()
        {
            new PageMenuEdit(),
            new PagePanelTool(),
            new PageMenuFile()
        };
        public Manual()
        {
            InitializeComponent();
            
        }

        public void Click_PagePanelTool(object sender, RoutedEventArgs e)
        {
            if (_previousButton != null)
                _previousButton.IsEnabled = true;

            Button tmpButton = sender as Button;
            tmpButton.IsEnabled = false;
            _previousButton = tmpButton;

            _navigateFrame<PagePanelTool>();
        }

        public void Click_PageMenuFile(object sender, RoutedEventArgs e)
        {
            if (_previousButton != null)
                _previousButton.IsEnabled = true;

            Button tmpButton = sender as Button;
            tmpButton.IsEnabled = false;
            _previousButton = tmpButton;

            _navigateFrame<PageMenuFile>();
        }

        public void Click_PageMenuEdit(object sender, RoutedEventArgs e)
        {
            if (_previousButton != null)
                _previousButton.IsEnabled = true;

            Button tmpButton = sender as Button;
            tmpButton.IsEnabled = false;
            _previousButton = tmpButton;

            _navigateFrame<PageMenuEdit>();
        }

        private void _navigateFrame<T>()
        {
            foreach(Page page in _pages)
            {
                if(page is T)
                {
                    ContentFrame.Navigate(page);
                    break;
                }
            }
        }
    }
}
