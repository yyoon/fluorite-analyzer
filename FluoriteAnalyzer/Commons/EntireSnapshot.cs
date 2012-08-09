using System.Collections.Generic;

namespace FluoriteAnalyzer.Commons
{
    public class EntireSnapshot
    {
        public EntireSnapshot()
        {
            FilePaths = new List<string>();
            FileSnapshots = new Dictionary<string, FileSnapshot>();
        }

        public List<string> FilePaths { get; set; }
        public Dictionary<string, FileSnapshot> FileSnapshots { get; set; }

        public string CurrentFile
        {
            get
            {
                if (FilePaths != null && FilePaths.Count > 0) { return FilePaths[0]; }
                else { return null; }
            }
        }
    }
}
