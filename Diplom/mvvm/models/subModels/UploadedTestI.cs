using DevExpress.Mvvm;

namespace Diplom.mvvm.models.subModels
{
    public class UploadedTestI : BindableBase
    {
        public string TName { get; set; }

        public string AssociatedPath { get; set; }

        public UploadedTestI(string testName, string associatedPath)
        {
            TName = testName;
            AssociatedPath = associatedPath;
        }

        public override string ToString()
        {
            return TName;
        }
    }
}
