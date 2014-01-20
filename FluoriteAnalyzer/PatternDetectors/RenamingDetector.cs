using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluoriteAnalyzer.Events;

namespace FluoriteAnalyzer.PatternDetectors
{
    class RenamingDetector : AbstractPatternDetector
    {
        private static RenamingDetector _instance = null;
        internal static RenamingDetector GetInstance()
        {
            return _instance ?? (_instance = new RenamingDetector());
        }

        private static readonly int LookaheadLimit = 1000;
        private static readonly string RenameCommand = "org.eclipse.jdt.ui.edit.text.java.rename.element";

        public override IEnumerable<PatternInstance> DetectAsPatternInstances(Commons.ILogProvider logProvider)
        {
            List<Event> list = logProvider.LoggedEvents
                .Where(x => x is DocumentChange || IsRenameCommand(x)).ToList();
            List<EclipseCommand> renameList = logProvider.LoggedEvents
                .Where(x => IsRenameCommand(x)).Cast<EclipseCommand>().ToList();

            List<PatternInstance> detectedPatterns = new List<PatternInstance>();

            foreach (int i in Enumerable.Range(0, renameList.Count))
            {
                EclipseCommand renameCommand = renameList[i];

                int renameIndex = list.IndexOf(renameCommand);
                int length = 0;
                for (int j = 1; j < LookaheadLimit; ++j)
                {
                    List<Event> sublist = list.GetRange(renameIndex + 1, j * 2);
                    if (sublist.Any(x => x is EclipseCommand))
                    {
                        break;
                    }

                    if (IsSymmetric(sublist.Cast<DocumentChange>().ToList()))
                    {
                        length = j * 2;
                        break;
                    }
                }

                detectedPatterns.Add(new PatternInstance(renameCommand, length, "Rename Element of Length " + length));
            }

            return detectedPatterns;
        }

        private bool IsSymmetric(List<DocumentChange> sublist)
        {
            if (sublist.Count % 2 == 1)
            {
                return false;
            }

            for (int i = 0; i < sublist.Count / 2; ++i)
            {
                if (!IsSymmetric(sublist[i], sublist[sublist.Count - i - 1]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsSymmetric(DocumentChange dc1, DocumentChange dc2)
        {
            if (dc1.Offset != dc2.Offset)
            {
                return false;
            }

            if (dc1 is Insert)
            {
                return dc2 is Delete && ((Insert)dc1).Text == ((Delete)dc2).Text;
            }
            else if (dc1 is Delete)
            {
                return dc2 is Insert && ((Delete)dc1).Text == ((Insert)dc2).Text;
            }
            else if (dc1 is Replace)
            {
                return dc2 is Replace && ((Replace)dc1).DeletedText == ((Replace)dc2).InsertedText
                    && ((Replace)dc1).InsertedText == ((Replace)dc2).DeletedText;
            }
            else
            {
                return false;
            }
        }

        public static bool IsRenameCommand(Event anEvent)
        {
            EclipseCommand eclipseCommand = anEvent as EclipseCommand;
            return eclipseCommand != null && eclipseCommand.CommandID == RenameCommand;
        }
    }
}
