using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Linq;
using System.Media;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;
using Path = System.IO.Path;
using LocalizatorHelper;
using System.Xml.Linq;
using System.Collections.Specialized;
using wpfCopilator.LocalizationResources;

namespace wpfCopilator
{
    public partial class MainWindow : Window
    {
        OpenFileDialog _openDialog = new OpenFileDialog();
        SaveFileDialog _saveDialog = new SaveFileDialog();
        private static RoutedUICommand applicationUndo;
        public static RoutedUICommand ApplicationUndo => applicationUndo;
        static MainWindow()
        {
            applicationUndo = new RoutedUICommand(
            "Application Undo", "ApplicationUndo", typeof(MainWindow));
        }
        public MainWindow()
        {
            InitializeComponent();
            ResourceManagerService.RegisterManager("LocalizationRes", LocalizationRes.ResourceManager, true);


            this.AddHandler(CommandManager.PreviewExecutedEvent,
               new ExecutedRoutedEventHandler(PreviewCommandExecute));

            lang.Content = "Язык ввода: " + InputLanguageManager.Current.CurrentInputLanguage.DisplayName;

            //Смена языка ввода
            System.Windows.Input.InputLanguageManager.Current.InputLanguageChanged += new InputLanguageEventHandler((sender, e) =>
            {
                lang.Content = "Язык ввода: " + e.NewLanguage.DisplayName;
            });

        }

        private void DataGrid_CollectionChange(DataTable table)
        {
            ErorsTable.ItemsSource = table.DefaultView;
            int[] arr = {2, 1, 1};
            int i=0;
            foreach (var column in ErorsTable.Columns)
            {
                column.Width = new DataGridLength(arr[i], DataGridLengthUnitType.Star);
                i++; 
            }
        }

        private string getFocusText()
        {
            if (mainTabControl.Items.Count >= 1)
            {
                TabItem item = mainTabControl.SelectedItem as TabItem;
                TextEditor txt = item.Content as TextEditor;
                return txt.Text;
            }
            else
            {
                return "";
            }
        }


        private void button_Play_Click(object sender, RoutedEventArgs e)
        {
            
            string input = getFocusText();

            DataTable table = Lexer.FindPassportNumbers(input);


            DataGrid_CollectionChange(table);
        }

        private void OpenNewFile(string Header, string pathFile, string Text)
        { 

            mainTabControl.Items.Add(new TabItem
            {
                Template = this.FindResource("CloseableTabItem") as ControlTemplate,
                Padding = new Thickness(3),
                Background = Brushes.White,
                AllowDrop = true,
                Tag = pathFile,
                Header = Header,
                Content = new TextEditor
                {
                    Name = "txt",
                    Text = Text,
                    AllowDrop = true,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    FontFamily = new FontFamily("Consolas"),
                    ShowLineNumbers = true,
                    SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("C#")
                }
            });
            TabItem item = mainTabControl.Items[mainTabControl.Items.Count - 1] as TabItem;
            item.Focus();

            Binding binding = new Binding();
            binding.ElementName = "sliderScale";
            binding.Path = new PropertyPath("Value");
            TextEditor tmp = item.Content as TextEditor;
            tmp.TextChanged += Tmp_TextChanged;
            tmp.SetBinding(TextEditor.FontSizeProperty, binding);
            


        }
        #region Commands
        private void PreviewCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            // Игнорируем, если отправителем является кнопка. Берем только поля ввода.
            if (e.Source is ICommandSource) return;

            // Игнорируем текущую команду ApplicationUndo
            if (e.Command == ApplicationUndo) return;


            TextEditor txt = e.Source as TextEditor;
            if (txt != null)
            {
                RoutedCommand cmd = (RoutedCommand)e.Command;

                // создаем запись в журнале истории.
                CommandHistoryItem historyItem = new CommandHistoryItem(
                    cmd.Name, txt, "Text", txt.Text);

                ListBoxItem item = new ListBoxItem();
                item.Content = historyItem;
                lstHistory.Items.Add(item);
            }
        }
        private void ApplicationUndoCommand_Executed(object sender, RoutedEventArgs e)
        {
            // Достаем из истории последнюю запись.
            CommandHistoryItem historyItem = ((ListBoxItem)lstHistory.Items[lstHistory.Items.Count - 1]).Content as CommandHistoryItem;
            // Выполняем отмену.
            historyItem.Undo();
            // Удаляем запись из истории.
            lstHistory.Items.Remove(lstHistory.Items[lstHistory.Items.Count - 1]);
        }
        private void ApplicationUndoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // Команда ApplicationUndo активна если журнал истории не пуст.
            if (lstHistory == null || lstHistory.Items.Count == 0)
                e.CanExecute = false;
            else
                e.CanExecute = true;
        }

        private void CommandNew_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        } //Всегда можно
        private void CommandOpen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _openDialog.Filter = "Text files (*.TXT)|*.txt|All Files (*.*)|*.*";
            if (_openDialog.ShowDialog() == true)
            {
                StreamReader reader = new StreamReader(_openDialog.FileName);
                OpenNewFile(Path.GetFileNameWithoutExtension(_openDialog.FileName), _openDialog.FileName, reader.ReadToEnd());
                reader.Close();
            }
        }
        private void CommandSaveAs_Executed(object sender, ExecutedRoutedEventArgs? e)
        {
            TabItem item = mainTabControl.SelectedItem as TabItem;
            TextEditor textBox = item.Content as TextEditor;
            _saveDialog.Filter = "Text files (*.TXT)|*.txt|All Files (*.*)|*.*";
            if (_saveDialog.ShowDialog() == true)
            {
                StreamWriter writer = new StreamWriter(_saveDialog.FileName);
                writer.WriteLine(textBox.Text);
                writer.Close();
            }
            item.Tag = _saveDialog.FileName;
            textBox.Background = Brushes.White;
        }
        private void CommandSave_Executed(object sender, ExecutedRoutedEventArgs? e)
        {
            TabItem item = mainTabControl.SelectedItem as TabItem;
            TextEditor textBox = item.Content as TextEditor;
            item.Background = Brushes.White;

            if (item.Tag == String.Empty)
            {
                CommandSaveAs_Executed(sender, e);
                if (item.Tag == String.Empty)
                    return;
            }
            StreamWriter writer = new StreamWriter(Convert.ToString(item.Tag));
            writer.WriteLine(textBox.Text);
            writer.Close();
            
        }
        private void CommandDelete(object sender, RoutedEventArgs e)
        {
            if (mainTabControl.Items.Count == 0) return;
            TabItem tabItem = (TabItem)mainTabControl.SelectedItem;

            if (tabItem.Content == null || !(tabItem.Content is TextEditor)) return;
            TextEditor textbox = (TextEditor)tabItem.Content;

            if (!string.IsNullOrEmpty(textbox.SelectedText))
            {
                textbox.Text = textbox.Text.Remove(textbox.SelectionStart, textbox.SelectionLength);
            }
        }
        private void CommandNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CreateNewWindow window = new CreateNewWindow();
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
            if (window.FileName != null)
            {
                string path = Environment.CurrentDirectory + "//" + "tempFilesDirectory//" + window.FileName + ".txt";
                OpenNewFile(window.FileName, String.Empty, String.Empty);
            }
        }
        #endregion
        #region Events
        private void Tmp_PreviewDragEnter(object sender, DragEventArgs e)
        {
            bool isCorrect = true;

            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                foreach (string filename in filenames)
                {
                    if (File.Exists(filename) == false)
                    {
                        isCorrect = false;
                        break;
                    }
                    FileInfo info = new FileInfo(filename);
                    if (info.Extension != ".txt")
                    {
                        isCorrect = false;
                        break;
                    }
                }
            }
            if (isCorrect == true)
                e.Effects = DragDropEffects.All;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }
        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                for (int i = 0; i < files.Length; i++)
                {
                    StreamReader reader = new StreamReader(files[i]);
                    OpenNewFile(Path.GetFileNameWithoutExtension(files[i]), files[i], reader.ReadToEnd());
                    reader.Close();
                }
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory + "//" + "tempFilesDirectory//");
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory + "//" + "tempFilesDirectory//");
            int i = 0;
            foreach (FileInfo file in di.GetFiles())
            {
                i++;
            }
            if (i != 0)
            {
                MessageBoxResult messageResult = MessageBox.Show("У вас " + Convert.ToString(i) + " несохраненных файлов, вы хотите их сохранить?", "Предупреждение", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (messageResult == MessageBoxResult.Yes)
                {
                    e.Cancel = false;
                }
                else if (messageResult == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
                else if(messageResult == MessageBoxResult.Cancel) 
                {
                    e.Cancel = true;
                }

                switch(messageResult)
                {
                    case MessageBoxResult.Yes:
                        CommandSaveAs_Executed(sender, null);
                        e.Cancel = false;
                        break;

                    case MessageBoxResult.No:
                        e.Cancel = false;
                        break;

                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;

                    default:
                        e.Cancel = true;
                        break;

                }
            }
        }
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Click_HelpButton(object sender, RoutedEventArgs e)
        {
            Manual manual = new Manual();
            manual.Show();
        }
        private void Click_AboutProgram(object sender, RoutedEventArgs e)
        {
            AboutProgram aboutProgram = new AboutProgram();

            aboutProgram.Owner = this;
            aboutProgram.Show();

        }
        private void comboBoxScale_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem tmp = comboBoxScale.SelectedItem as ComboBoxItem;
            sliderScale.Value = Convert.ToDouble(tmp.Tag);
        }
        private void buttonX_Click(object sender, RoutedEventArgs e)
        {
            TabItem closingItem = mainTabControl.SelectedItem as TabItem;
            if(closingItem.Background == Brushes.Gray)
            {
                MessageBoxResult messageResult = MessageBox.Show("Этот файл не сохранен, вы хотите его сохранить?", "Предупреждение", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (messageResult == MessageBoxResult.Yes)
                {
                    CommandSave_Executed(sender, null);
                }
                else if (messageResult == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            mainTabControl.Items.Remove(closingItem);
        }
        private void Tmp_TextChanged(object? sender, EventArgs e)
        {
            TabItem changeItem = mainTabControl.SelectedItem as TabItem;
            changeItem.Background = Brushes.Gray;
        }
        #endregion
        


        private void comboBoxLocalization_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox? comboBox = sender as ComboBox;
            ComboBoxItem? item = comboBox.SelectedItem as ComboBoxItem;
            switch (item?.Content)
            {
                case "RUS":
                    ResourceManagerService.ChangeLocale("ru-RU");
                    break;
                case "ENG":
                    ResourceManagerService.ChangeLocale("en-US");
                    break;
            }
        }
    }
}
