using FluoriteAnalyzer.Commons;
using FluoriteAnalyzer.PatternDetectors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FluoriteAnalyzer.Pipelines
{
    public class DetectBacktrackingFilter : BasePipelineFilter<FileInfo, FileInfo>
    {
        public DetectBacktrackingFilter()
        {
            _settings = new DetectBacktrackingFilterSettings();
        }

        protected class DetectBacktrackingFilterSettings
        {
            public DetectBacktrackingFilterSettings()
            {
            }
        }

        private DetectBacktrackingFilterSettings _settings;
        public override object FilterSettings
        {
            get { return _settings; }
        }

        public override FileInfo Compute(FileInfo input)
        {
            LogProvider provider = new LogProvider();
            provider.OpenLog(input.FullName);

            BacktrackingDetector detector = new BacktrackingDetector();
            var patterns = detector.DetectAsPatternInstances(provider);

            // Save the results to a file.
            DetectionResult result = new DetectionResult(provider.LogPath, patterns);
            result.SaveToFile(GetSaveFileName(input.DirectoryName, input.Name));
            result.ExportToCSV(GetSaveFileName(input.DirectoryName, input.Name, "csv"));

            // Return the original input.
            return input;
        }
    }
}
