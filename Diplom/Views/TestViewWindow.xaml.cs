using System.Windows;
using System.Windows.Documents;
using Diplom.mvvm.models.subModels;

namespace Diplom.Views
{
    /// <summary>
    /// Логика взаимодействия для TestViewWindow.xaml
    /// </summary>
    public partial class TestViewWindow : Window
    {
        private TModel _tModel;

        public TestViewWindow(TModel testmodel)
        {
            InitializeComponent();
            _tModel = testmodel;

            HeaderText.Text = _tModel.TName;
            //var xml = _tModel.TXML(TModel.QDeffault, QModel.QDeffault, AModel.QDeffault);

            DocumentViewer.Document = ToFlowDocument(_tModel.ToString());

        }

        private static FlowDocument ToFlowDocument(string input)
        {
            var result = new FlowDocument();

            var p = new Paragraph(new Run(input))
            {
                FontSize = 14,
                FontStyle = FontStyles.Normal,
                TextAlignment = TextAlignment.Left
            };

            result.Blocks.Add(p);

            return result;
        }


    }
}
