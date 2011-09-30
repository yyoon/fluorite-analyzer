using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FluoriteAnalyzer.Events;
using FluoriteAnalyzer.Analyses;

namespace FluoriteAnalyzer.PatternDetectors
{
    class ErrorRecoveryDetector : IPatternDetector
    {
        private static ErrorRecoveryDetector _instance = null;
        internal static ErrorRecoveryDetector GetInstance()
        {
            return _instance ?? (_instance = new ErrorRecoveryDetector());
        }

        private static readonly int _threshold = 2000;  // find all the create/terminate pairs within one second.

        public IEnumerable<ListViewItem> Detect(ILogProvider logProvider)
        {
            List<RunCommand> runCommands = logProvider.LoggedEvents.OfType<RunCommand>().ToList();

            bool lastRunWasErroneous = false;

            foreach (RunCommand runCommand in runCommands.Where(x => !x.IsTerminate))
            {
                bool erroneous = IsErroneous(runCommands, runCommand);
                if (!erroneous && lastRunWasErroneous)
                {
                    yield return new ListViewItem(new string[] {
                        runCommand.ID.ToString(),
                        "",
                        logProvider.GetVideoTime(runCommand).ToString(),
                        ""
                    });
                }

                lastRunWasErroneous = erroneous;
            }
        }

        private bool IsErroneous(List<RunCommand> runCommands, RunCommand runCommand)
        {
            return IsErroneous(runCommands, runCommands.IndexOf(runCommand));
        }

        private bool IsErroneous(List<RunCommand> runCommands, int index)
        {
            if (index < 0 || index >= runCommands.Count - 1)
            {
                return false;
            }

            return !runCommands[index].IsTerminate &&
                runCommands[index + 1].IsTerminate &&
                runCommands[index + 1].Timestamp - runCommands[index].Timestamp < _threshold;
        }
    }
}
