using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Diplom.Views
{
    /// <summary>
    /// Логика взаимодействия для Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
        }

        public string ResponseText
        {
            get => RTextBox.Text;
            set => RTextBox.Text = value;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidInput(RTextBox.Text) && RTextBox.Text != "")
            {
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Введите ФИО!");
                DialogResult = false;
            }
        }

        private bool IsValidInput(string input)
        {
            string pattern = "^[A-Za-zА-Яа-яЁё]+$";
            return Regex.IsMatch(input, pattern);
        }
    }
}
