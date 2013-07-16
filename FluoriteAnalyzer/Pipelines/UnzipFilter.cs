using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FluoriteAnalyzer.Pipelines
{
    /// <summary>
    /// Takes a DirectoryInfo (e.g., p101) and unzip all the archives in it,
    /// collects all the xml files under the directory itself,
    /// and returns the DirectoryInfo object as is.
    /// </summary>
    public class UnzipFilter : BasePipelineFilter<DirectoryInfo, DirectoryInfo>
    {
        public UnzipFilter()
        {
            _settings = new UnzipFilterSettings();
        }

        // Settings class for this filter.
        protected class UnzipFilterSettings
        {
            public UnzipFilterSettings()
            {
            }
        }

        private UnzipFilterSettings _settings;
        public override object FilterSettings
        {
            get { return _settings; }
        }

        public override DirectoryInfo Compute(DirectoryInfo input)
        {
            // Collect all the *.zip files.
            // TODO do the same for *.7z files.
            var archives = input.GetFiles("*.zip");

            foreach (var archive in archives)
            {
                FastZip fz = new FastZip();
                fz.ExtractZip(
                    archive.FullName,
                    input.FullName,
                    FastZip.Overwrite.Always,
                    null,
                    @"+\.xml$",
                    null,
                    true
                );

                // Assuming the files are extracted.
                var logFiles = input.GetFiles("*.xml", SearchOption.AllDirectories);
                foreach (var logFile in logFiles)
                {
                    if (logFile.DirectoryName != input.FullName)
                    {
                        // Move the file!
                        logFile.CopyTo(Path.Combine(input.FullName, logFile.Name), true);
                        logFile.Delete();
                    }
                }
            }

            // Now all the files should be under the desired dir.
            var subDirs = input.GetDirectories();
            foreach (var subDir in subDirs)
            {
                subDir.Delete();
            }

            return input;
        }
    }
}
