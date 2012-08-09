using System.Collections.Generic;
using System.Windows.Forms;
using FluoriteAnalyzer.Common;

namespace FluoriteAnalyzer.PatternDetectors
{
    interface IPatternDetector
    {
        IEnumerable<PatternInstance> DetectAsPatternInstances(ILogProvider logProvider);
        IEnumerable<ListViewItem> DetectAsListViewItems(ILogProvider logProvider);
    }
}
