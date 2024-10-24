using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DevExpress.Mvvm;
using Diplom.mvvm.models.subModels;
using Diplom.other;

namespace Diplom.mvvm.models
{
    internal class Loader : BindableBase
    {
        public ObservableCollection<UploadedTestI> UpLoad { get; }

        public List<Exeptions> LoadExceptions { get; }

        private const string dt = "Tests";
        private const string dr = "Results";
        private static string DeffaultPass = "19voenkr";
        private readonly string _baseDir;

        public string PTT => _baseDir + $"{dt}";
        public string PTR => _baseDir + $"{dr}";

        public Loader()
        {
            _baseDir = AppDomain.CurrentDomain.BaseDirectory;

            if (!StructureIsReady())
            {
                RecreateStructure();
            }



            UpLoad = new ObservableCollection<UploadedTestI>();

            LoadExceptions = UpLoads().ToList();
        }

        private void RecreateStructure()
        {
            Directory.CreateDirectory(PTT);
            Directory.CreateDirectory(PTR);
        }

        private bool StructureIsReady()
        {
            var isT = Directory.Exists(PTT);
            var isR = Directory.Exists(PTR);

            return isT && isR;
        }

        private IEnumerable<Exeptions> UpLoads()
        {
            var errorsList = new List<Exeptions>();
            var info = new DirectoryInfo(PTT);
            var files = info.GetFiles().OrderBy(p=>p.CreationTime);
            foreach (var file in files)
            {
                try
                {
                    var testName = GetName(file.FullName);

                    UpLoad.Add(new UploadedTestI(testName, file.FullName));
                }
                catch (Exeptions e)
                {
                    errorsList.Add(e);
                }
            }

            return errorsList;
        }

        static string GetName(string path)
        {
            var test = Load(path, true);
            return test.TName;
        }

        public static TModel Load(string path, bool loadOnlyHeader = false)
        {
            try
            {
                var text = Decrypt(path);
                var xdoc = XDocument.Parse(text);

                return TModel.FromXMl(xdoc, TModel.QDeffault, QModel.QDeffault, AModel.QDeffault, !loadOnlyHeader);
            }
            catch (CryptographicException e)
            {
                throw new Exeptions("Криптографическая ошибка при дешифровке.", e.Message);
            }
            catch (XmlException e)
            {
                throw new Exeptions("Ошибка XML-парсинга.", e.Message);
            }
            catch (Exception e)
            {
                throw new Exeptions("Ошибка при загрузке теста.", e.Message);
            }
        }

        public static void Saving(string path, TModel model)
        {
            var xdoc = model.TXML(TModel.QDeffault, QModel.QDeffault, AModel.QDeffault);

            EncryptToFile(xdoc.ToString(), path);
            
        }

        private static void EncryptToFile(string input, string outputFile)
        {
            var ue = new UnicodeEncoding();
            var key = ue.GetBytes(DeffaultPass);

            var filecrypt = outputFile;
            var qwcrypt = new FileStream(filecrypt, FileMode.Create);

            var qwcrypto = new RijndaelManaged();

            var cs = new CryptoStream(qwcrypt,
                qwcrypto.CreateEncryptor(key, key),
                CryptoStreamMode.Write);

            var m = new MemoryStream(Encoding.Default.GetBytes(input));
            int data;
            while ((data = m.ReadByte()) != -1)
                cs.WriteByte((byte)data);
            
            m.Close();
            cs.Close();
            qwcrypt.Close();
        }

        public static string Saving(TModel testModel, string destinationFolder)
        {
            var path = SavingWay(testModel.TName, destinationFolder);

            Saving(path, testModel);

            return path;
        }

        private static string SavingWay(string name, string destinationFolder, string extension = ".xml")
        {
            var dirinfo = new DirectoryInfo(destinationFolder);
            var files = dirinfo.GetFiles();

            var typedName = $"{name}_{DateTime.Now:MM_dd_yyyy}";
            

            var additinalPrefix = 0;
            string finalName;
            do
            {
                finalName = typedName + (additinalPrefix == 0 ? "" : $"_{additinalPrefix}") + extension;
                additinalPrefix++;

            } while (files.Any(s => string.Equals(s.Name, finalName, StringComparison.CurrentCultureIgnoreCase)));

            return $"{destinationFolder}\\{finalName}";

        }

        private static string Decrypt(string inputFile)
        {
            try
            {
                var ue = new UnicodeEncoding();
                var key = ue.GetBytes(DeffaultPass);

                using (var qwcrypt = new FileStream(inputFile, FileMode.Open))
                {
                    using (var qwcrypto = new RijndaelManaged())
                    {
                        using (var cs = new CryptoStream(qwcrypt, qwcrypto.CreateDecryptor(key, key), CryptoStreamMode.Read))
                        {
                            var bytes = new List<byte>();
                            int data;
                            while ((data = cs.ReadByte()) != -1)
                            {
                                bytes.Add((byte)data);
                            }

                            return Encoding.Default.GetString(bytes.ToArray());
                        }
                    }
                }
            }
            catch (CryptographicException e)
            {
                throw new Exeptions(e.Message);
            }
            catch (Exception e)
            {
                throw new Exeptions(e.Message);
            }
        }

        public static void DeleteTest(string infoAssociatedPath)
        {
            File.Delete(infoAssociatedPath);
        }
    }
}
