using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FluoriteAnalyzer.Analyses;

namespace FluoriteAnalyzer.PatternDetectors
{
    interface IPatternDetector
    {
        IEnumerable<ListViewItem> Detect(ILogProvider logProvider);
    }
}
