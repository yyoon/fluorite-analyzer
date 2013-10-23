using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluoriteAnalyzer.PatternDetectors
{
    interface IPreviewablePatternInstance
    {
        int FirstID { get; }
        int SecondID { get; }
    }
}
