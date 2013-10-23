using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using FluoriteAnalyzer.Utils;

namespace FluoriteAnalyzer.Pipelines
{
    public delegate void AnalysisStartHandler();
    public delegate void AnalysisFinishedHandler();
    public delegate void AnalysisExceptionHandler(Exception ex);

    public class PipelinedAnalysis
    {
        public static Thread PerformAnalysis(
            DirectoryInfo dinfo,
            AnalysisStartHandler startHandler,
            AnalysisFinishedHandler finishHandler,
            AnalysisExceptionHandler exceptionHandler)
        {
            CleanPipelinedAnalysisResults(dinfo);

            Thread worker = new Thread(new ThreadStart(delegate()
                {
                    if (startHandler != null)
                    {
                        startHandler();
                    }

                    try
                    {
                        var dirs = dinfo.GetDirectories("p*", SearchOption.TopDirectoryOnly);

                        dirs.AsParallel()
                            .Select(new UnzipFilter().Compute)
                            .Select(new FixClosingFilter().Compute)
                            .Select(new MergeFilter().Compute)
                            .Select(new RemoveTyposFilter().Compute)
                            .Select(new DetectMovesFilter().Compute)
                            .Select(new DetectBacktrackingFilter().Compute)
                            .ToList();
                    }
                    catch (Exception ex)
                    {
                        exceptionHandler(ex);
                    }

                    if (finishHandler != null)
                    {
                        finishHandler();
                    }
                }));

            worker.Start();

            return worker;
        }

        public static void CleanPipelinedAnalysisResults(DirectoryInfo dinfo)
        {
            // Delete the xml, lck, txt, dtr files
            dinfo.DeleteAllFilesWithPattern("*.xml");
            dinfo.DeleteAllFilesWithPattern("*.lck");
            dinfo.DeleteAllFilesWithPattern("*.txt");
            dinfo.DeleteAllFilesWithPattern("*.dtr");
        }
    }
}
