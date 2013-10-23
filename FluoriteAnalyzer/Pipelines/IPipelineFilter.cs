using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluoriteAnalyzer.Pipelines
{
    // T1: Input Type
    // T2: Output Type
    public interface IPipelineFilter<T1, T2>
    {
        Type InputType { get; }
        Type OutputType { get; }
        object FilterSettings { get; }

        T2 Compute(T1 input);
    }
}
