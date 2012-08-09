using FluoriteAnalyzer.Events;

namespace FluoriteAnalyzer.Common
{
    class FileSnapshot
    {
        public string Content { get; set; }
        public DocumentChange LastChange { get; set; }
    }
}
