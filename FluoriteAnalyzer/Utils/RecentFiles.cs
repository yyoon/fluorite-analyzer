using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace FluoriteAnalyzer.Utils
{
    [Serializable]
    public class RecentFiles
    {
        private static RecentFiles _instance;
        private static readonly string ParentPath;

        private static readonly int MAX_FILES = 15;
        private static readonly string FILE_NAME = "RecentFiles.xml";

        private readonly List<string> _list;

        static RecentFiles()
        {
            ParentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                      "FluoriteAnalyzer");
        }

        private RecentFiles()
        {
            _list = new List<string>();
        }

        public List<string> List
        {
            get { return _list; }
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public static RecentFiles GetInstance()
        {
            if (_instance == null)
            {
                _instance = new RecentFiles();
            }

            return _instance;
        }

        public static void Save()
        {
            // If Path doesn't exist, create it.
            var dinfo = new DirectoryInfo(ParentPath);
            if (!dinfo.Exists)
            {
                dinfo.Create();
            }

            var serializer = new XmlSerializer(typeof (RecentFiles));
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
                var serializer = new XmlSerializer(typeof (RecentFiles));
                _instance = (RecentFiles) serializer.Deserialize(textReader);
            }
            catch (Exception)
            {
                _instance = new RecentFiles();
            }
            finally
            {
                textReader.Close();

                if (_instance == null || _instance._list == null)
                {
                    _instance = new RecentFiles();
                }
            }
        }

        public void Touch(string filePath)
        {
            if (_list.Contains(filePath))
            {
                _list.Remove(filePath);
                _list.Add(filePath);
            }
            else
            {
                _list.Add(filePath);
                while (_list.Count > MAX_FILES)
                {
                    _list.RemoveAt(0);
                }
            }
        }

        public bool IsEmpty()
        {
            return _list.Count == 0;
        }
    }
}