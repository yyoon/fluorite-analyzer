using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    }
}
