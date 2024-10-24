using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DevExpress.Mvvm;
using Diplom.other;

namespace Diplom.mvvm.models.subModels
{
    public class TModel : BindableBase
    {
        public static string QDeffault = "Test";

        public TModel()
        {
            Questions = new ObservableCollection<QModel>();
            DTCommand = new DelegateCommand<QModel>(QDelete);
            ССCommand = new DelegateCommand<QModel>(QCopy);
            QEmptyCommand = new DelegateCommand(QEmpty);
        }


        public bool Skips { get; set; } = true;

        public string TName { get; set; }

        public string TAuthor { get; set; }

        public string CreatedDate { get; set; }

        public int TTime { get; set; } = 10;

        public int TQuestions { get; set; } = 1;

        public ObservableCollection<QModel> Questions { get; }


        public XDocument TXML(string documentName, string questionName, string answerName)
        {
            var xdoc = new XDocument();

            var test = new XElement(documentName);
            var testNameAttr = new XAttribute("Name", TName);
            var testTimeAttr = new XAttribute("Time", TTime);
            var testCountAttr = new XAttribute("Count", TQuestions);
            var testSkipsAttr = new XAttribute("Skips", Skips);
            test.Add(testNameAttr);
            test.Add(testTimeAttr);
            test.Add(testCountAttr);
            test.Add(testSkipsAttr);

            if (!string.IsNullOrEmpty(CreatedDate))
            {
                var testDateAttr = new XAttribute("Date", CreatedDate);
                test.Add(testDateAttr);
            }

            if (!string.IsNullOrEmpty(TAuthor))
            {
                var testTAuthorAttr = new XAttribute("TAuthor", TAuthor);
                test.Add(testTAuthorAttr);
            }

            foreach (var questionModel in Questions)
            {
                var question = questionModel.TXML(questionName, answerName);
                test.Add(question);
            }

            xdoc.Add(test);

            return xdoc;
        }

        public static TModel FromXMl(XDocument xdoc, string documentName, string questionName, string answerName,
            bool needToParseQuestions = true)
        {
            var result = new TModel();

            var testElement = xdoc.Element(documentName);
            if (testElement == null)
                throw new Exeptions("Фаил испорчен: невозможно прочитать заголовок");

            var nameAttr = testElement.Attribute("Name");
            var dateAttr = testElement.Attribute("Date");
            var timeAttr = testElement.Attribute("Time");
            var countAttr = testElement.Attribute("Count");
            var skipableAttr = testElement.Attribute("Skips");
            var authorAttr = testElement.Attribute("TAuthor");

            result.TName = nameAttr?.Value ??
                              throw new Exeptions("Фаил испорчен: невозможно прочитать название теста");

            if (timeAttr == null)
                throw new Exeptions("Файл испорчен: невозможно определить время на прохождение теста");
            result.TTime = int.Parse(timeAttr.Value);

            if (countAttr == null)
                throw new Exeptions("Файл испорчен: невозможно определить кол-во вопросов на тест");
            result.TQuestions = int.Parse(countAttr.Value);

            if (skipableAttr != null)
            {
                result.Skips = bool.Parse(skipableAttr.Value);
            }

            if (authorAttr != null)
            {
                result.TAuthor = authorAttr.Value;
            }

            if (dateAttr != null)
            {
                result.CreatedDate = dateAttr.Value;
            }

            var xquestions = testElement.Elements(questionName);

            if (needToParseQuestions)
            {
                var questions = QFromXElement(xquestions, answerName);
                foreach (var questionModel in questions)
                {
                    result.Questions.Add(questionModel);
                }
            }

            return result;
        }

        public static IEnumerable<QModel> QFromXElement(IEnumerable<XElement> elements,
            string answerName)
        {
            var result = new List<QModel>();

            foreach (var questionElemnt in elements)
            {
                var question = QModel.FXML(questionElemnt, answerName);
                result.Add(question);
            }

            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Название = {TName}");
            if(!string.IsNullOrEmpty(TAuthor))
                sb.AppendLine($"Автор = {TAuthor}");

            sb.AppendLine($"Дата создания = {CreatedDate}");

            sb.AppendLine($"Время на вопрос = {TTime}");
            sb.AppendLine($"Кол-во вопросов = {TQuestions}");
            sb.AppendLine($"Пропуск вопросов = {Skips}\n");

            foreach (var questionModel in Questions)
            {
                sb.AppendLine(questionModel.ToString());
            }

            return sb.ToString();
        }


        public DelegateCommand<QModel> DTCommand { get; }

        public void QDelete(QModel question)
        {
            Questions.Remove(question);
        }


        public DelegateCommand QEmptyCommand { get; }

        public void QEmpty()
        {
            Questions.Add(new QModel(""));
        }


        public DelegateCommand<QModel> ССCommand { get; }

        public void QCopy(QModel question)
        {
            Questions.Add(new QModel(question));
        }

        public Tuple<string, bool> Checker()
        {
            var sb = new StringBuilder();

            var critical = false;

            if (string.IsNullOrEmpty(TName))
            {
                sb.AppendLine("*<Отсутствует название теста>");
                critical = true;
            }

            if (string.IsNullOrEmpty(TAuthor))
            {
                sb.AppendLine("<Отсутствует Автор>");
            }

            if (TTime < 10)
            {
                sb.AppendLine("<Время на вопрос < 10 сек>");
            }

            if (TQuestions < 1)
            {
                sb.AppendLine("<Кол-во вопросов для теста < 1 >");
            }

            var correctCounter = 0;
            foreach (var questionModel in Questions)
            {
                var res = questionModel.Checker();

                if (res.Item2 == false)
                    correctCounter++;
                else if (!string.IsNullOrEmpty(res.Item1))
                {
                    sb.AppendLine(res.Item1);

                    if (res.Item2)
                        critical = true;
                }
            }

            if (correctCounter == 0)
            {
                sb.AppendLine("*<Отсутствуют корректные вопросы>");
                critical = true;
            }

            return new Tuple<string, bool>(sb.ToString(), critical);
        }

        public void Clean()
        {
            if (string.IsNullOrEmpty(TName))
            {
                TName = "Заголовок теста";
            }

            if (string.IsNullOrEmpty(TAuthor))
            {
                TAuthor = "";
            }

            if (TTime < 10)
            {
                TTime = 10;
            }

            if (TQuestions < 1)
            {
                TQuestions = 20;
            }

            for (var i = 0; i < Questions.Count; i++)
            {
                var questionModel = Questions[i];
                questionModel.Clean();

                if(!questionModel.Answers.Any())
                    Questions.Remove(questionModel);
            }

        }
    }        
}
