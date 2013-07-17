using System.Collections.Generic;
using System.Windows.Forms;
using FluoriteAnalyzer.Commons;

namespace FluoriteAnalyzer.PatternDetectors
{
    interface IPatternDetector
    {
        IEnumerable<PatternInstance> DetectAsPatternInstances(ILogProvider logProvider);
        IEnumerable<ListViewItem> DetectAsListViewItems(ILogProvider logProvider);
    }
}
