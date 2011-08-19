using System;
using System.Xml;

namespace FluoriteAnalyzer.Events
{
    internal class AssistCommand : Command
    {
        #region AssistTypeEnum enum

        public enum AssistTypeEnum
        {
            QUICK_ASSIST,
            CONTENT_ASSIST,
        }

        #endregion

        #region StartEndEnum enum

        public enum StartEndEnum
        {
            START,
            END,
        }

        #endregion

        public AssistCommand(XmlElement element)
            : base(element)
        {
            AssistType = (AssistTypeEnum) (Enum.Parse(typeof (AssistTypeEnum), GetPropertyValueFromDict("assist_type")));
            StartEnd = (StartEndEnum) (Enum.Parse(typeof (StartEndEnum), GetPropertyValueFromDict("start_end")));
        }

        public AssistTypeEnum AssistType { get; set; }

        public StartEndEnum StartEnd { get; set; }
    }
}