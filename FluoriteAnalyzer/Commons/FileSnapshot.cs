namespace FluoriteAnalyzer.Commons
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using FluoriteAnalyzer.Events;

    /// <summary>
    /// A class representing the snapshot of a certain file.
    /// </summary>
    public class FileSnapshot
    {
        /// <summary>
        /// The preceding context size which will be displayed right before the last change.
        /// </summary>
        private static readonly int PrecedingContextSize = 5;

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the content of the file.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the last change made in this file.
        /// </summary>
        /// <value>
        /// The last change.
        /// </value>
        public DocumentChange LastChange { get; set; }

        /// <summary>
        /// Displays the current content in rich text box and highlight the last change.
        /// </summary>
        /// <param name="richText">The rich text.</param>
        /// <param name="strikeoutFont">The strikeout font.</param>
        public void DisplayInRichTextBox(RichTextBox richText, Font strikeoutFont)
        {
            // Clear the text.
            richText.Text = string.Empty;

            if (string.IsNullOrEmpty(this.Content))
            {
                return;
            }

            // If no change was made, simply display the whole content and leave.
            if (this.LastChange == null)
            {
                richText.Text = this.Content;
                return;
            }

            // Get the deletion offset & text / insertion offset & length.
            int deletionOffset = -1;
            string deletedText = string.Empty;
            int insertionOffset = -1;
            int insertionLength = -1;

            this.GetOffsetAndLength(
                ref deletionOffset,
                ref deletedText,
                ref insertionOffset,
                ref insertionLength);

            // insert the deleted text in the desired location.
            string content = this.Content;
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

        /// <summary>
        /// Moves the scroll to the given offset.
        /// </summary>
        /// <param name="richText">The rich text.</param>
        /// <param name="desiredOffset">The desired offset.</param>
        private static void MoveScrollToOffset(RichTextBox richText, int desiredOffset)
        {
            // Find desired offset
            for (int i = 0; i < PrecedingContextSize; ++i)
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

        /// <summary>
        /// Gets the offset and the length.
        /// </summary>
        /// <param name="deletionOffset">The deletion offset.</param>
        /// <param name="deletedText">The deleted text.</param>
        /// <param name="insertionOffset">The insertion offset.</param>
        /// <param name="insertionLength">Length of the insertion.</param>
        private void GetOffsetAndLength(
            ref int deletionOffset,
            ref string deletedText,
            ref int insertionOffset,
            ref int insertionLength)
        {
            deletionOffset = -1;
            deletedText = string.Empty;
            insertionOffset = -1;
            insertionLength = -1;

            if (this.LastChange is Insert)
            {
                Insert insert = (Insert)this.LastChange;
                insertionOffset = insert.Offset;
                insertionLength = insert.Length;
            }
            else if (this.LastChange is Delete)
            {
                Delete delete = (Delete)this.LastChange;
                deletionOffset = delete.Offset;
                deletedText = delete.Text;
            }

            if (this.LastChange is Replace)
            {
                Replace replace = (Replace)this.LastChange;
                deletionOffset = replace.Offset;
                deletedText = replace.DeletedText;
                insertionOffset = replace.Offset;
                insertionLength = replace.InsertionLength;
            }
            else if (this.LastChange is Move)
            {
                Move move = (Move)this.LastChange;
                if (move.DeletedFrom == this.FilePath)
                {
                    deletionOffset = move.EffectiveDeletionOffset;
                    deletedText = move.DeletedText;
                }

                if (move.InsertedTo == this.FilePath)
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
