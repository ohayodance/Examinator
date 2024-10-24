using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Threading;
using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using DevExpress.Mvvm;
using Diplom.Extensions;
using Diplom.mvvm.models.subModels;
using Diplom.other;
using Diplom.Views;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace Diplom.mvvm.models
{
    class Solver : BindableBase
    {
        private UploadedTestI _tInfo;
        private DispatcherTimer Timer;
        public DelegateCommand<int> CQCommand { get; set; }
        public DelegateCommand ETCommand { get; set; }
        public DelegateCommand NQCommand { get; set; }
        public DelegateCommand PQCommand { get; set; }

        public int TLeft { get; set; }

        public String TLeftStr { get; set; }

        public Results Result { get; set; }
        public Results result1 = new Results();
        public QModel QSelected { get; set; }

        public TModel TModel { get; set; }

        public ObservableCollection<QModel> Questions { get; set; }

        public void SetData(TModel testModel, UploadedTestI preloadedInfo)
        {
            TModel = testModel;
            _tInfo = preloadedInfo;
            Questions = TModel?.Questions;
            Randomizer();
            ETCommand = new DelegateCommand(EndingByCommand);
            NQCommand = new DelegateCommand(NextQuestion);
            CQCommand = new DelegateCommand<int>(QChange);
            PQCommand = new DelegateCommand(PreviousQuestion);
            Questions = new ObservableCollection<QModel>(Questions.ToList()
                .GetRange(0, Math.Min(testModel.TQuestions, testModel.Questions.Count)));
            QSelected = Questions.FirstOrDefault();
            QSelected.IsCurrent = true;
            RaisePropertiesChanged("Questions");


            TLeft = TModel.Skips
                ? TModel.TTime * TModel.TQuestions
                : TModel.TTime;
            var dialog = new Window2();
            while ((bool)!dialog.ShowDialog())
            {
                MessageBox.Show("Введите ФИО!");
            }
            result1.Student = dialog.ResponseText;
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += timer_Tick;
            Timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            TLeft--;

            var tspan = TimeSpan.FromSeconds(TLeft);

            if (TLeft < 0 && TModel.Skips)
            {
                Ending();
                return;
            }

            if (TLeft < 0 && !TModel.Skips)
            {
                QSwitch(QSelected.QNumber + 1);
                if (QSelected.QNumber >= TModel.TQuestions)
                {
                    Ending();
                    return;
                }

                TLeft = TModel.TTime;
            }

            TLeftStr = tspan.ToString(@"mm\:ss");
        }

        private int Calculator(IEnumerable<QModel> Questions)
        {
            var rightAnswers = 0;
            foreach (var quest in Questions)
            {
                bool questResult = true;
                foreach (var ans in quest.Answers)
                {
                    if (ans.IsSelected != ans.AIsRight)
                        questResult = false;
                }

                if (questResult)
                    rightAnswers++;
            }

            return rightAnswers;
        }

        public void Randomizer()
        {
            Questions.Shuffle();
            int num = 1;
            foreach (var quest in Questions)
            {
                quest.QNumber = num++;
                quest.Answers.Shuffle();
            }
        }

        private void QChange(int num)
        {
            QSwitch(num);
        }

        private void QSwitch(int num)
        {
            if (Questions != null && num > 0 && Questions.Count > num - 1)
            {
                QSelected.IsSolved = false;
                foreach (var ans in QSelected.Answers)
                {
                    if (ans.IsSelected)
                    {
                        QSelected.IsSolved = true;
                        break;
                    }
                }

                QSelected.IsCurrent = false;
                QSelected = Questions[num - 1];
                QSelected.IsCurrent = true;
                RaisePropertyChanged("Questions");
            }
        }

        private void EndingByCommand()
        {
            var result =  MessageBox.Show("Завершить тест и получить результат?", "Вы уверены?", MessageBoxButton.OKCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                Ending();
                
            }
        }

        private void Ending()
        {
            try
            {
                Timer.Stop();
                var results = Calculator(Questions);
                result1.Time = TModel.TTime * TModel.TQuestions - TLeft;
                result1.CAnswers = results;
                result1.CQuestions = Math.Min(TModel.TQuestions, TModel.Questions.Count);
                double ratio = (double)result1.CAnswers / result1.CQuestions;
                result1.Mark = (int)Math.Round(ratio * 10);
                MessageBox.Show($"Количество правильных ответов: {result1.CAnswers}/{result1.CQuestions}");
                var windows = App.Current.Windows;
                foreach (var window in windows)
                {
                    if (window is SolveTestWindow thisWindow)
                    {
                        thisWindow.Close();
                    }
                }

                SaveDB(result1);

                string folderPath = @"C:\Users\Alex\Diplom\Diplom\bin\Debug\Results";
                string currentTime = DateTime.Now.ToString("dd.MM.yy-HH.mm");
                string fileName = $"Результат_{currentTime}.txt";
                string filePath = Path.Combine(folderPath, fileName);
                StreamWriter sw = new StreamWriter(filePath);
                sw.WriteLine("ФИО: " + result1.Student);
                sw.WriteLine("Количество правильных ответов: " + result1.CAnswers + "/" + result1.CQuestions);
                int minutes = result1.Time / 60;
                int seconds = result1.Time % 60;
                if (minutes > 0)
                {
                    sw.WriteLine("Время прохождения: " + minutes + " минут " + seconds + " секунд");
                }
                else
                {
                    sw.WriteLine("Время прохождения: " + result1.Time + " секунд");
                }
                sw.WriteLine("Отметка: " + result1.Mark);
                sw.WriteLine("Дата: " + result1.Date);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка! Файл не сохранен... " + e.Message);
            }
        }

        private void SaveDB(Results result)
        {
            string connectionString = "server=localhost;database=TestingDB;user=root";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO ResultInfo (Student, Time, CAnswers, CQuestions, Mark, Date) 
                         VALUES (@Student, @Time, @CAnswers, @CQuestions, @Mark, @Date)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {

                    cmd.Parameters.AddWithValue("@Student", result.Student);
                    cmd.Parameters.AddWithValue("@Time", result.Time);
                    cmd.Parameters.AddWithValue("@CAnswers", result.CAnswers);
                    cmd.Parameters.AddWithValue("@CQuestions", result.CQuestions);
                    cmd.Parameters.AddWithValue("@Mark", result.Mark);

                    DateTime parsedDate = DateTime.Parse(result.Date);
                    cmd.Parameters.AddWithValue("@Date", parsedDate);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        private void NextQuestion()
        {
            QSwitch(QSelected.QNumber + 1);
            if (!TModel.Skips)
            {
                if (QSelected.QNumber >= TModel.TQuestions)
                {
                    Ending();
                }
                else
                {
                    TLeft = TModel.TTime;
                }
            }
        }

        private void PreviousQuestion()
        {
            QSwitch(QSelected.QNumber - 1);
        }
    }
}