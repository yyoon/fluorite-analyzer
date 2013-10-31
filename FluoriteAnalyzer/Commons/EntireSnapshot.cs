namespace FluoriteAnalyzer.Commons
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents all snapshots of a given point in time.
    /// </summary>
    public class EntireSnapshot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntireSnapshot"/> class.
        /// </summary>
        public EntireSnapshot()
        {
            this.FilePaths = new List<string>();
            this.FileSnapshots = new Dictionary<string, FileSnapshot>();
        }

        /// <summary>
        /// Gets the file paths.
        /// </summary>
        /// <value>
        /// The file paths.
        /// </value>
        public List<string> FilePaths { get; private set; }

        /// <summary>
        /// Gets the file snapshots.
        /// </summary>
        /// <value>
        /// The file snapshots.
        /// </value>
        public Dictionary<string, FileSnapshot> FileSnapshots { get; private set; }

        /// <summary>
        /// Gets the current (the most recently opened / modified) file.
        /// </summary>
        /// <value>
        /// The current (the most recently opened / modified) file. null if there is no file.
        /// </value>
        public string CurrentFile
        {
            get
            {
                if (this.FilePaths != null && this.FilePaths.Count > 0)
                {
                    return this.FilePaths[0];
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
