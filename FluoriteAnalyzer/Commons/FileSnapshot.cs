using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FluoriteAnalyzer.Events;
using System;

namespace FluoriteAnalyzer.Commons
{
    public class FileSnapshot
    {
        public string FilePath { get; set; }
        public string Content { get; set; }
        public DocumentChange LastChange { get; set; }

        public void DisplayInRichTextBox(RichTextBox richText, Font strikeoutFont)
        {
            // Clear the text.
            richText.Text = "";

            if (string.IsNullOrEmpty(Content)) { return; }

            // If no change was made, simply display the whole content and leave.
            if (LastChange == null)
            {
                richText.Text = Content;
                return;
            }

            // Get the deletion offset & text / insertion offset & length.
            int deletionOffset = -1;
            string deletedText = "";
            int insertionOffset = -1;
            int insertionLength = -1;

            GetOffsetAndLength(
                ref deletionOffset, ref deletedText,
                ref insertionOffset, ref insertionLength);

            // insert the deleted text in the desired location.
            string content = Content;
            if (deletionOffset != -1)
            {
                content = content.Insert(deletionOffset, deletedText);
            }

            // Set the text.
            richText.Text = content;

            // Stupid workaround due to the weird behavior of RichTextBox (it translates \r\n into \n automatically).
            if (deletionOffset != -1)
            {
                deletionOffset -= content.Substring(0, deletionOffset).Count(x => x == '\r');

                richText.Select(deletionOffset, deletedText.Length - deletedText.Count(x => x == '\r'));
                richText.SelectionFont = strikeoutFont;
                richText.SelectionBackColor = Color.LightGray;
            }

            if (insertionOffset != -1)
            {
                insertionOffset -= content.Substring(0, insertionOffset).Count(x => x == '\r');

                richText.Select(insertionOffset, insertionLength);
                richText.SelectionBackColor = Color.LightGreen;
            }

            if (insertionOffset == -1 && deletionOffset == -1)
            {
                richText.Select(0, 0);
                richText.ScrollToCaret();
            }
            else if (insertionOffset == -1)
            {
                MoveScrollToOffset(richText, deletionOffset);
            }
            else
            {
                MoveScrollToOffset(richText, insertionOffset);
            }
        }

        private static readonly int PRECEDING_CONTEXT_SIZE = 5;
        private static void MoveScrollToOffset(RichTextBox richText, int desiredOffset)
        {
            // Find desired offset
            for (int i = 0; i < PRECEDING_CONTEXT_SIZE; ++i)
            {
                int lastLineEndingIndex = richText.Text.Substring(0, desiredOffset)
                    .LastIndexOf('\n');
                if (lastLineEndingIndex != -1)
                {
                    desiredOffset = Math.Max(0, lastLineEndingIndex - 1);
                }
            }

            richText.Select(desiredOffset, 0);
            richText.ScrollToCaret();
        }

        private void GetOffsetAndLength(ref int deletionOffset, ref string deletedText,
            ref int insertionOffset, ref int insertionLength)
        {
            deletionOffset = -1;
            deletedText = "";
            insertionOffset = -1;
            insertionLength = -1;

            if (LastChange is Insert)
            {
                Insert insert = (Insert)LastChange;
                insertionOffset = insert.Offset;
                insertionLength = insert.Length;
            }
            else if (LastChange is Delete)
            {
                Delete delete = (Delete)LastChange;
                deletionOffset = delete.Offset;
                deletedText = delete.Text;
            }
            if (LastChange is Replace)
            {
                Replace replace = (Replace)LastChange;
                deletionOffset = replace.Offset;
                deletedText = replace.DeletedText;
                insertionOffset = replace.Offset;
                insertionLength = replace.InsertionLength;
            }
            else if (LastChange is Move)
            {
                Move move = (Move)LastChange;
                if (move.DeletedFrom == FilePath)
                {
                    deletionOffset = move.EffectiveDeletionOffset;
                    deletedText = move.DeletedText;
                }
                if (move.InsertedTo == FilePath)
                {
                    insertionOffset = move.InsertionOffset;
                    insertionLength = move.InsertionLength;
                }
            }

            // Adjust the insertion offset, only if the deletion precedes the insertion.
            if (deletionOffset != -1 && deletionOffset <= insertionOffset)
            {
                insertionOffset += deletedText.Length;
            }
        }
    }
}
