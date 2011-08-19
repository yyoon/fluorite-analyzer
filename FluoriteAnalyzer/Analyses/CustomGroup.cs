using System;
using System.Collections.Generic;

namespace FluoriteAnalyzer.Analyses
{
    [Serializable]
    public class CustomGroup
    {
        public CustomGroup()
        {
            Name = null;
            Patterns = new List<string>();
        }

        public CustomGroup(string name)
        {
            Name = name;
            Patterns = new List<string>();
        }

        public string Name { get; set; }
        public List<string> Patterns { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}