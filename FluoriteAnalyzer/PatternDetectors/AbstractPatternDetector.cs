using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FluoriteAnalyzer.Commons;

namespace FluoriteAnalyzer.PatternDetectors
{
    abstract class AbstractPatternDetector : IPatternDetector
    {
        public abstract IEnumerable<PatternInstance> DetectAsPatternInstances(ILogProvider logProvider);

        public virtual IEnumerable<ListViewItem> DetectAsListViewItems(ILogProvider logProvider)
        {
            return DetectAsPatternInstances(logProvider)
                .Select(x => new ListViewItem(new string[] {
                    x.PrimaryEvent.ID.ToString(),
                    x.PatternLength.ToString(),
                    logProvider.GetVideoTime(x.PrimaryEvent),
                    x.Description
                }) { Tag = x });
        }
    }
}
