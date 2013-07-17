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
            return ConvertToListViewItems(logProvider, DetectAsPatternInstances(logProvider));
        }

        public IEnumerable<ListViewItem> ConvertToListViewItems(ILogProvider logProvider, IEnumerable<PatternInstance> patternInstances)
        {
            return patternInstances.Select(x => new ListViewItem(
                new string[] {
                    x.PrimaryEvent.ID.ToString(),
                    x.PatternLength.ToString(),
                    logProvider == null ? "unknown" : logProvider.GetVideoTime(x.PrimaryEvent),
                    x.Description
                }) { Tag = x });
        }
    }
}
