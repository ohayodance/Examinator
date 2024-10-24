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
using Diplom.mvvm.models;
using Diplom.mvvm.models.subModels;

namespace Diplom.Views
{
    /// <summary>
    /// Interaction logic for SolveTestWindow.xaml
    /// </summary>
    public partial class SolveTestWindow : Window
    {
        public SolveTestWindow(TModel testModel, UploadedTestI preloadedInfo)
        {
            InitializeComponent();
            ((Solver)DataContext).SetData(testModel, preloadedInfo);
        }
    }
}
