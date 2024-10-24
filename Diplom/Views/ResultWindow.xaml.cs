using System.Windows;
using Diplom.other;

namespace Diplom.Views
{
    /// <summary>
    /// Логика взаимодействия для ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        public ResultWindow(Results result)
        {
            InitializeComponent();

            ResultLabel.Content = $"";
        }
    }
}
