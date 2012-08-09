using FluoriteAnalyzer.Events;

namespace FluoriteAnalyzer.Commons
{
    class FileSnapshot
    {
        public string Content { get; set; }
        public DocumentChange LastChange { get; set; }
    }
}
