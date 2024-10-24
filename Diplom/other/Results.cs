using System;
using DevExpress.Mvvm;
using Diplom.mvvm.models.subModels;

namespace Diplom.other
{
    public class Results
    {

        public string Student { get; set; } = "";

        public int CAnswers { get; set; } = 0;

        public int CQuestions { get; set; } = 0;

        public int Mark { get; set; } = 0;

        public string Date { get; set; } = DateTime.Now.ToString("g");

        public int Time { get; set; } = 0;
    }
}
