using System;
using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Mvvm;
using Diplom.mvvm.models.subModels;
using Diplom.other;
using Diplom.Views;
using MaterialDesignThemes.Wpf;
using MessageBox = System.Windows.MessageBox;

namespace Diplom.mvvm.models
{
    public class Editor : BindableBase
    {
        private TModel _tModel;
        private UploadedTestI _tInfo;

        public ObservableCollection<QModel> Questions => _tModel?.Questions;

        public Editor()
        {
            CloseCommand = new DelegateCommand<Window>(Close);
            SaveCommand = new DelegateCommand(Save);
        }


        public TModel TModel => _tModel;

        public UploadedTestI Info => _tInfo;

        public string AvgTime
        {
            get
            {
                if (_tModel == null)
                    return "0 мин";
                var ttl =  _tModel.TTime * Math.Min(_tModel.TQuestions, _tModel.Questions.Count);
                int secs = ttl % 60;
                int mins = ttl / 60;

                return $"{mins} мин :  {secs} сек";
            }
        }

        public int MinutToTest
        {
            get
            {
                return _tModel != null ? _tModel.TTime : 0; }

            set
            {
                if (_tModel != null)
                {
                    _tModel.TTime = value;
                    RaisePropertiesChanged("AvgTime");
                }
            }
        }

        public int CQuest
        {
            get
            {
                return _tModel != null ? _tModel.TQuestions : 0;
            }

            set
            {
                if (_tModel != null)
                {
                    _tModel.TQuestions = value;
                    RaisePropertiesChanged("AvgTime");
                }
            }
        }

        public void SetData(TModel testModel, UploadedTestI preloadedInfo)
        {
            _tModel = testModel;
            _tInfo = preloadedInfo;
            RaisePropertiesChanged("Questions");
            RaisePropertiesChanged("TModel");
            RaisePropertiesChanged("AvgTime");
            RaisePropertiesChanged("MinutToTest");
            RaisePropertiesChanged("CQuest");
        }


        public DelegateCommand<Window> CloseCommand { get; }

        public void Close(Window window)
        {
            window?.Close();
        }

        public DelegateCommand SaveCommand { get; }

        public void Save()
        {
            var result = TModel.Checker();
            if (result.Item2)
            {
                var errorWindow = new ErrorWindow("<Критическая ошибка = *>\n" + result.Item1);
                errorWindow.ShowDialog();
                return;
            }

            try
            {
                TModel.Clean();
                TModel.CreatedDate = DateTime.Now.ToString("MM/dd/yyyy");

                if(!string.IsNullOrEmpty(Info.TName))
                    Loader.Saving(_tInfo.AssociatedPath, TModel);
                else
                {
                    var path = Loader.Saving(TModel, Info.AssociatedPath);
                    _tInfo = new UploadedTestI(TModel.TName, path);
                }

                _tInfo.TName = TModel.TName;
                MessageBox.Show("Успешно сохранено!", "Результат");
            }
            catch (Exception e)
            {
                MessageBox.Show("Что-то пошло не по плану, непредвиденная ошибка");
            }
        }
    }
}
