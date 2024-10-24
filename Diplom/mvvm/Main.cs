using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using Diplom.mvvm.models;
using Diplom.mvvm.models.subModels;
using Diplom.other;
using Diplom.Views;
using Microsoft.Win32;

namespace Diplom.mvvm
{
    public class Main : BindableBase
    {
        private readonly Loader _loader;

        public bool TeacherMode { get; set; }

        public ObservableCollection<UploadedTestI> UpLoad => _loader.UpLoad;

        public Main()
        {
            #if DEBUG
                TeacherMode = true;
#endif

            _loader = new Loader();

            if (_loader.LoadExceptions.Any())
            {
                MessageBox.Show("Файлы повреждены : \n" + string.Join("\n", _loader.LoadExceptions));
            }

            TeacherCommand = new DelegateCommand(SwitchMode);
            ViewCommand = new DelegateCommand<object>(OpenVW);
            EditCommand = new DelegateCommand<object>(OpenEW);
            SolveCommand = new DelegateCommand<object>(OpenSW);
            CreateCommand = new DelegateCommand(Create);
            DTCommand = new DelegateCommand<UploadedTestI>(Delete);
            ImportCommand = new DelegateCommand(Import);
            ShowInstructionCommand = new DelegateCommand(ShowInstruction);
            CloseCommand = new DelegateCommand<Window>(Close);
            Results ress = new Results();
        }


        public DelegateCommand<Window> CloseCommand { get; }

        public void Close(Window window)
        {
            window?.Close();
        }

        public DelegateCommand TeacherCommand { get; }

        private void SwitchMode()
        {
            if (TeacherMode)
            {
                TeacherMode = false;
                RaisePropertiesChanged("TeacherMode");
                return;
            }

            var dial = new PasswordDialog();
            if (dial.ShowDialog() == true)    
            {
                if (dial.ResponseText == "teacher")
                    TeacherMode = true;
                else
                    MessageBox.Show("Пароль неверен, проверьте язык ввода и capslock, и повторите попытку");
            }
            RaisePropertiesChanged("TeacherMode");

        }

        public DelegateCommand<object> ViewCommand { get; }

        private static void OpenVW(object param)
        {
            if (!(param is UploadedTestI uploadedInfo))
            {
                MessageBox.Show("Внутреняя ошибка: Невозможно открыть данный тест");
                return;
            }

            try
            {
                var testModel = Loader.Load(uploadedInfo.AssociatedPath);
                var viewWindow = new TestViewWindow(testModel);

                viewWindow.ShowDialog();
            }
            catch (Exeptions e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show("Что-то пошло не так: невозможно загрузить файл");
            }
        }


        public DelegateCommand<object> EditCommand { get; }

        private static void OpenEW(object param)
        {
            if (!(param is UploadedTestI uploadedInfo))
            {
                MessageBox.Show("Внутреняя ошибка: Невозможно открыть данный тест");
                return;
            }

            try
            {
                var testModel = Loader.Load(uploadedInfo.AssociatedPath);

                var editWindow = new EditTestWindow(testModel, uploadedInfo);
                editWindow.ShowDialog();

            }
            catch (Exeptions e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show("Что-то пошло не так: невозможно загрузить файл");
            }
        }


        public DelegateCommand<object> SolveCommand { get; }
        
        private static void OpenSW(object param)
        {
            if (!(param is UploadedTestI uploadedInfo))
            {
                MessageBox.Show("Внутреняя ошибка: Невозможно открыть данный тест");
                return;
            }

            try
            {

                var testModel = Loader.Load(uploadedInfo.AssociatedPath);

                var solveWindow = new SolveTestWindow(testModel, uploadedInfo);
                solveWindow.ShowDialog();

            }
            catch (Exeptions e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show("Что-то пошло не так: невозможно загрузить файл");
            }
        }


        public DelegateCommand CreateCommand { get; }

        private void Create()
        {
            try
            {
                var testModel = new TModel();

                var editWindow = new EditTestWindow(testModel, new UploadedTestI(null, _loader.PTT));
                
                if (editWindow.ShowDialog() == true)
                {
                    _loader.UpLoad.Insert(0, ((Editor)editWindow.DataContext).Info);
                }

            }
            catch (Exeptions e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show("Что-то пошло не так: невозможно загрузить/сохранить файл");
            }
        }


        public DelegateCommand<UploadedTestI> DTCommand { get; }

        private void Delete(UploadedTestI info)
        {
            var result = MessageBox.Show("Востановить тест будет невозможно", "Вы уверены?", MessageBoxButton.OKCancel,
                MessageBoxImage.Question);

            if(result == MessageBoxResult.Cancel)
                return;

            try
            {
                Loader.DeleteTest(info.AssociatedPath);
            }
            catch (Exception e)
            {
                MessageBox.Show("Что-то пошло не так: Возможно, файл уже удален");
            }

            _loader.UpLoad.Remove(info);
        }


        public DelegateCommand ImportCommand { get; }

        private void Import()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == false)
                return;

            var filePath = openFileDialog.FileName;

            try
            {

                var testModel = Loader.Load(filePath);
                var UploadedTestI = new UploadedTestI(testModel.TName, filePath);
                _loader.UpLoad.Add(UploadedTestI);

                MessageBox.Show("Тест успешно импортирован!");
            }
            catch (Exeptions e)
            {
                var errorWindow = new ErrorWindow(e.Message + "\n" + e.AdditionalErrorInfo);
                errorWindow.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show("Что-то пошло не так: невозможно загрузить/сохранить файл");
            }
        }


        public DelegateCommand ShowInstructionCommand { get; }

        private void ShowInstruction()
        {
            try
            {
                



            }
            catch (Exception e)
            {
                MessageBox.Show("Что-то пошло не так: невозможно загрузить/сохранить файл");
            }
        }
    }
}
