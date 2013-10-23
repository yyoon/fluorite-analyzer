using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FluoriteAnalyzer.Pipelines
{
    // T1: Input Type
    // T2: Output Type
    public abstract class BasePipelineFilter<T1, T2> : IPipelineFilter<T1, T2>
    {
        public Type InputType { get { return typeof(T1); } }
        public Type OutputType { get { return typeof(T2); } }

        public abstract object FilterSettings { get; }
        public abstract T2 Compute(T1 input);

        protected void AppendResult(string outputDir, string inputName, string message)
        {
            // TODO: Make this efficient.
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();

            string className = methodBase.DeclaringType.Name;
            string fileName = string.Format("Results_{0}_{1}.txt", className, inputName);

            using (StreamWriter writer = new StreamWriter(Path.Combine(outputDir, fileName), true, Encoding.Default))
            {
                writer.WriteLine(message);
            }
        }

        protected string GetSaveFileName(string outputDir, string inputName)
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();

            string className = methodBase.DeclaringType.Name;
            string fileName = string.Format("{0}_{1}.dtr", Path.GetFileNameWithoutExtension(inputName), className);

            return Path.Combine(outputDir, fileName);
        }
    }
}
