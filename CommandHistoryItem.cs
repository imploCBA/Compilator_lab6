using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace wpfCopilator
{
    internal class CommandHistoryItem
    {
        // Имя выполненной команды.
        private string commandName;
        public string CommandName
        {
            get { return commandName; }
            set { commandName = value; }
        }

        // Элемент, для которого была выполнена команда.
        private UIElement elementActedOn;
        public UIElement ElementActedOn
        {
            get { return elementActedOn; }
            set { elementActedOn = value; }
        }

        // Свойство, которое было изменено в целевом элементе.
        private string propertyActedOn;
        public string PropertyActedOn
        {
            get { return propertyActedOn; }
            set { propertyActedOn = value; }
        }

        // Предыдущего состояния.
        private object previousState;
        public object PreviousState
        {
            get { return previousState; }
            set { previousState = value; }
        }

        public CommandHistoryItem(string commandName, UIElement elementActedOn,
            string propertyActedOn, object previousState)
        {
            CommandName = commandName;
            ElementActedOn = elementActedOn;
            PropertyActedOn = propertyActedOn;
            PreviousState = previousState;
        }

        // Метод для отмены действия.
        public void Undo()
        {
            // Получаем Type элемента, для которого выполнялась команда.
            Type elementType = ElementActedOn.GetType();
            // Находим свойство по имени.
            PropertyInfo property = elementType.GetProperty(PropertyActedOn);
            // Устанавливаем свойству сохраненное значение.
            property.SetValue(ElementActedOn, PreviousState, null);
        }

        public override string ToString()
        {
            return commandName;
        }
    }
}
