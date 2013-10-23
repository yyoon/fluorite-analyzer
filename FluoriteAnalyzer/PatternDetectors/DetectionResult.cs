using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace FluoriteAnalyzer.PatternDetectors
{
    /// <summary>
    /// Used for storing / restoring pattern detection results.
    /// </summary>
    [Serializable]
    class DetectionResult
    {
        public DetectionResult(
            string logPath,
            IEnumerable<PatternInstance> patternInstances)
        {
            if (logPath == null || patternInstances == null)
            {
                throw new ArgumentNullException();
            }

            LogPath = logPath;
            PatternInstances = patternInstances.ToList().AsReadOnly();
        }

        public string LogPath { get; private set; }
        public ReadOnlyCollection<PatternInstance> PatternInstances { get; private set; }

        public void SaveToFile(string filePath)
        {
            // Serialize
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(filePath, FileMode.Create))
            {
                formatter.Serialize(stream, this);
            }
        }

        public static DetectionResult LoadFromFile(string filePath)
        {
            // Deserialize
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(filePath, FileMode.Open))
            {
                DetectionResult result = formatter.Deserialize(stream) as DetectionResult;
                return result;
            }
        }

        public void ExportToCSV(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var pattern in PatternInstances)
                {
                    string filename = Path.GetFileNameWithoutExtension(LogPath);
                    int underscoreIndex = filename.IndexOf('_');
                    string pid = underscoreIndex == -1
                        ? filename
                        : filename.Substring(0, underscoreIndex);

                    writer.WriteLine(string.Join(", ", new object[]
                    {
                        pid,
                        pattern.CSVLine
                    }));
                }
            }
        }
    }
}
