using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Linq;
using DevExpress.Mvvm;

namespace Diplom.mvvm.models.subModels
{
    public class QModel : BindableBase, ICloneable
    {
        public static string QDeffault = "Question";   

        public string QText { get; set; }

        public int QNumber { get; set; }

        public ObservableCollection<AModel> Answers { get; }

        public bool IsCurrent { get; set; }

        public bool IsSolved { get; set; }


        public QModel(string questionText)
        {
            QText = questionText;
            Answers = new ObservableCollection<AModel>();
            DTCommand = new DelegateCommand<AModel>(Delete);
            AddEmptyCommand = new DelegateCommand(AddEmpty);
        }

        public QModel(QModel question)
        {
            QText = question.QText;
            Answers = new ObservableCollection<AModel>(question.Answers);

            DTCommand = new DelegateCommand<AModel>(Delete);
            AddEmptyCommand = new DelegateCommand(AddEmpty);
        }


        public XElement TXML(string blockName, string answersBlockName)
        {
            var element = new XElement(blockName);
            var textAttr = new XAttribute("Text", QText);
            element.Add(textAttr);

            foreach (var answer in Answers)
            {
                var xmlAnswer = answer.TXML(answersBlockName);
                element.Add(xmlAnswer);
            }

            return element;
        }

        public static QModel FXML(XElement questElement, string answerName)
        {
            var text = questElement.FirstAttribute.Value;
            var result = new QModel(text);

            var answers = questElement.Elements(answerName);

            foreach (var answerElement in answers)
            {
                var answer = AModel.FXML(answerElement);
                result.Answers.Add(answer);
            }
            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"={QText}");

            foreach (var ansModel in Answers)
            {
                sb.AppendLine(ansModel.ToString());
            }

            return sb.ToString();
        }


        public object Clone()
        {
            return new QModel(this);
        }


        public DelegateCommand<AModel> DTCommand { get; }

        public void Delete(AModel ans)
        {
            Answers.Remove(ans);
        }


        public DelegateCommand AddEmptyCommand { get; }

        public void AddEmpty()
        {
            Answers.Add(new AModel("", false));
        }


        public Tuple<string, bool> Checker()
        {
            var sb = new StringBuilder();

            var critical = false;
            if (string.IsNullOrEmpty(QText))
            {
                sb.AppendLine("*<Отсутствует текст вопроса>");
                sb.AppendLine(ToString());
                critical = true;
            }

            var c = 0;
            var isRightA = 0;

            foreach (var ansModel in Answers)
            {
                var isCorrect = ansModel.IsCorrect();
                if (isCorrect)
                {
                    c++;
                    if (ansModel.AIsRight)
                        isRightA++;
                }
            }

            if (isRightA == 0)
            {
                sb.AppendLine("*<Не отмечен Верный ответ> : " + QText);
                critical = true;
            }

            if (c == 0)
            {
                sb.AppendLine("*<Отсутствуют ответы> : " + QText);
                critical = true;
            }

            return new Tuple<string, bool>(sb.ToString(), critical);
        }

        public void Clean()
        {
            if (string.IsNullOrEmpty(QText))
                QText = "Заголовок вопроса";

            for (var i = 0; i < Answers.Count; i++)
            {
                var ansModel = Answers[i];

                if (!ansModel.IsCorrect())
                    Answers.Remove(ansModel);
            }

        }
    }
}
