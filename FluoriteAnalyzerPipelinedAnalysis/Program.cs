using FluoriteAnalyzer.Pipelines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluoriteAnalyzerPipelinedAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo dinfo = new DirectoryInfo(args[0]);

            PipelinedAnalysis.CleanPipelinedAnalysisResults(dinfo);
            Thread worker = PipelinedAnalysis.PerformAnalysis(dinfo,
                delegate() { Console.WriteLine("Starting Analysis on \"" + dinfo.FullName + "\""); },
                delegate() { Console.WriteLine("Finished Analysis"); },
                delegate(Exception ex) { throw ex; });

            worker.Join();
        }
    }
}
