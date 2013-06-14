using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluoriteAnalyzer.Pipelines
{
    // T1: Input Type
    // T2: Output Type
    public abstract class BasePipelineFilter<T1, T2> : IPipelineFilter<T1, T2>
    {
        public Type InputType
        {
            get
            {
                return typeof(T1);
            }
        }

        public Type OutputType
        {
            get
            {
                return typeof(T2);
            }
        }

        public abstract object FilterSettings { get; }

        public abstract T2 Compute(T1 input);
    }
}
