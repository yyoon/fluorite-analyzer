using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FluoriteAnalyzer.Utils
{
    [Serializable]
    public class Profiles
    {
        private static Profiles _instance;
        private static readonly string ParentPath;

        private static readonly string FILE_NAME = "Profiles.xml";

        public string LastLogClosingFixPath;

        static Profiles()
        {
            ParentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FluoriteAnalyzer");
        }

        public static Profiles GetInstance()
        {
            return _instance ?? (_instance = new Profiles());
        }

        private Profiles()
        {
        }

        public static void Save()
        {
            // If Path doesn't exist, create it.
            var dinfo = new DirectoryInfo(ParentPath);
            if (!dinfo.Exists)
            {
                dinfo.Create();
            }

            var serializer = new XmlSerializer(typeof(Profiles));
            TextWriter textWriter = new StreamWriter(Path.Combine(ParentPath, FILE_NAME));
            serializer.Serialize(textWriter, GetInstance());
            textWriter.Close();
        }

        public static void Load()
        {
            if (new FileInfo(Path.Combine(ParentPath, FILE_NAME)).Exists == false)
            {
                return;
            }

            TextReader textReader = new StreamReader(Path.Combine(ParentPath, FILE_NAME));

            try
            {
                var serializer = new XmlSerializer(typeof(Profiles));
                _instance = (Profiles)serializer.Deserialize(textReader);
            }
            catch (Exception)
            {
                _instance = new Profiles();
            }
            finally
            {
                textReader.Close();

                if (_instance == null)
                {
                    _instance = new Profiles();
                }
            }
        }
    }
}
