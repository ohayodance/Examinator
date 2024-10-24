using System.ComponentModel;
using System.Windows;
using Diplom.mvvm.models;
using Diplom.mvvm.models.subModels;

namespace Diplom.Views
{
    /// <summary>
    /// Логика взаимодействия для EditTestWindow.xaml
    /// </summary>
    public partial class EditTestWindow : Window
    {
        public EditTestWindow(TModel testModel, UploadedTestI preloadedInfo)
        {
            InitializeComponent();

            ((Editor) DataContext).SetData(testModel, preloadedInfo);
        }

        private void EditTestWindow_OnClosing(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show("Убедитесь, что сохранили изменения, иначе они будут утеряны", "Вы уверены?", MessageBoxButton.OKCancel,
                MessageBoxImage.Question);


            if (result == MessageBoxResult.Cancel)
                e.Cancel = true;

            if (((Editor) DataContext).Info.TName != null)
                DialogResult = true;
        }
    }
}
