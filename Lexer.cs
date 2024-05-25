using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;

namespace wpfCopilator
{
    internal class Lexer
    {

        public static DataTable FindPassportNumbers(string text)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Название выражения", typeof(string));
            table.Columns.Add("Выражение", typeof(string));
            table.Columns.Add("Расположение", typeof(string));

            Regex regex = new Regex(@"\b\d{2} \d{2} \d{6}\b");
            MatchCollection matches = regex.Matches(text);

            foreach (Match match in matches)
            {
                AddRowToTable(table, match.Value, match.Index + 1, match.Index + match.Length, "Серия и номер паспорта");
            }


            Regex regexCoffe = new Regex(@"\b\w*кофе\w*\b", RegexOptions.IgnoreCase);
            MatchCollection matchesCoffe = regexCoffe.Matches(text);

            foreach (Match match in matchesCoffe)
            {
                if (match.Value.Length == 10)
                {
                    AddRowToTable(table, match.Value, match.Index + 1, match.Index + match.Length, "Слово с подстрокой 'кофе' и длиной 10 символов");
                }
            }

            string patternLetter = @"[АВЕКМНОРСТУХ]";
            string patternDigits = @"\d";

            int state = 0;
            int startIndex = -1;
            int currentIndex = 0;

            while (currentIndex < text.Length)
            {
                char c = text[currentIndex];

                switch (state)
                {
                    case 0: // Начальное состояние
                        if (Regex.IsMatch(c.ToString(), patternLetter))
                        {
                            state = 1;
                            startIndex = currentIndex;
                        }
                        break;

                    case 1: // Прочитана первая буква
                        if (Regex.IsMatch(c.ToString(), patternDigits) &&
                            currentIndex + 2 < text.Length &&
                            Regex.IsMatch(text.Substring(currentIndex, 3), patternDigits + "{3}"))
                        {
                            state = 2;
                            currentIndex += 2; 
                        }
                        else
                        {
                            state = 0;
                        }
                        break;

                    case 2: // Прочитаны три цифры
                        if (Regex.IsMatch(c.ToString(), patternLetter) &&
                            currentIndex + 1 < text.Length &&
                            Regex.IsMatch(text.Substring(currentIndex, 2), patternLetter + "{2}"))
                        {
                            state = 3;
                            currentIndex += 1;
                        }
                        else
                        {
                            state = 0;
                        }
                        break;

                    case 3: // Прочитаны две буквы
                        if (Regex.IsMatch(c.ToString(), patternDigits) &&
                            currentIndex + 1 < text.Length &&
                            Regex.IsMatch(text.Substring(currentIndex, 2), patternDigits + "{2}"))
                        {
                            state = 4;
                        }
                        else
                        {
                            state = 0;
                        }
                        break;

                    case 4: // Прочитаны две цифры региона
                        if (Regex.IsMatch(c.ToString(), patternDigits))
                        {
                            currentIndex++;
                        }
                        string expression = text.Substring(startIndex, currentIndex - startIndex + 1);
                        AddRowToTable(table, expression, startIndex + 1, currentIndex, "Российский автомобильный номер");
                        state = 0;
                        break;
                }

                currentIndex++;
            }
            if (state == 4)
            {
                string expression = text.Substring(startIndex, text.Length - startIndex);
                AddRowToTable(table, expression, startIndex + 1, text.Length, "Российский автомобильный номер");
            }

            return table;
        }

        static void AddRowToTable(DataTable table, string expression, int startIndex, int endIndex, string expressionName)
        {
            string location = $"С {startIndex} символ по {endIndex}";

            DataRow row = table.NewRow();
            row["Название выражения"] = expressionName;
            row["Выражение"] = expression;
            row["Расположение"] = location;
            table.Rows.Add(row);
        }


        public static DataTable Analyze(string input)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Код", typeof(string));
            table.Columns.Add("Тип", typeof(string));
            table.Columns.Add("Содержание", typeof(string));
            table.Columns.Add("Место положения", typeof(string));

            var patterns = new (string pattern, string type, string code)[]
            {
            (@"\bdict\b", "ключевое слово", "20"),
            (@"\bfor\b", "ключевое слово", "21"),
            (@"\bin\b", "ключевое слово", "22"),
            (@"\n", "конец строки", "30"),
            (@"'[^']*'", "строка", "40"),
            (@"=", "оператор присваивания", "10"),
            (":", "двоеточие", "11"),
            (",", "запятая", "12"),
            (@"{", "левая фигурная скобка", "13"),
            (@"}", "правая фигурная скобка", "14"),
            (@"\[", "левая квадратная скобка", "15"),
            (@"\]", "правая квадратная скобка", "16"),
            (@"\(", "левая круглая скобка", "17"),
            (@"\)", "правая круглая скобка", "18"),
            (@"\d+", "целое без знака", "1"),
            };

            void AddRow(string code, string type, string content, int start, int end)
            {
                table.Rows.Add(code, type, content, $"С {start + 1} по {end + 1} символ");
            }


            int position = 0;


            while (position < input.Length)
            {
                bool matched = false;

                foreach (var (pattern, type, code) in patterns)
                {
                    var match = Regex.Match(input.Substring(position), pattern);

                    if (match.Success && match.Index == 0)
                    {
                        AddRow(code, type, match.Value, position, position + match.Length - 1);
                        position += match.Length;
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    position++;
                }
            }

            return table;
        }
    }
}
