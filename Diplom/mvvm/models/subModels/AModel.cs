using System;
using System.Text;
using System.Xml.Linq;
using DevExpress.Mvvm;
using Diplom.other;

namespace Diplom.mvvm.models.subModels
{
    public class AModel : BindableBase, ICloneable
    {
        public static string QDeffault = "Answer";

        public string AText { get; set; }
        public bool IsSelected { get; set; }
        public bool AIsRight { get; set;  }

        public AModel(string answerText, bool isRight)
        {
            AText = answerText;
            AIsRight = isRight;
        }

        public AModel(AModel answerModel)
        {
            AText = answerModel.AText;
            AIsRight = answerModel.AIsRight;
        }

        public XElement TXML(string elementName)
        {
            var el = new XElement(elementName);
            
            var txtField = new XAttribute("AnswerText", AText);
            el.Add(txtField);

            var rightField = new XAttribute("IsRight", AIsRight);
            el.Add(rightField);

            return el;
        }

        public static AModel FXML(XElement el)
        {
            var txtField = el.Attribute("AnswerText");
            if(txtField == null)
                throw new Exeptions("Фаил поврежден: отсутствует текст ответа");

            var isRightField = el.Attribute("IsRight");
            var isRight = false;
            if (isRightField != null)
                isRight = bool.Parse(isRightField.Value);

            return new AModel(txtField.Value, isRight);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var fst = AIsRight ? "+" : "-";
            sb.Append($"{fst}{AText}");

            return sb.ToString();
        }

        public object Clone()
        {
            return new AModel(this);
        }

        public bool IsCorrect()
        {
            return !string.IsNullOrEmpty(AText);
        }
    }
}
