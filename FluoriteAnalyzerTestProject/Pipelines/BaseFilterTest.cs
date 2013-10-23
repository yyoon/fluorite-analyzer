using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FluoriteAnalyzer.Pipelines
{
    public abstract class BaseFilterTest
    {
        protected string GetDataPath()
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();

            string className = methodBase.DeclaringType.Name;
            string methodName = methodBase.Name;

            return string.Format(@"Data\{0}\{1}", className, methodName);
        }
    }
}
