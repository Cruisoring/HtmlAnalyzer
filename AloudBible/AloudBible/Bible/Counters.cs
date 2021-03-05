using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AloudBible.Bible
{
    public class Counters
    {
        public int ByteOffset { get; set; }
        public int CharOffset { get; set; }
        public int BooksOffset { get; set; }
        public int ChaptersOffset { get; set; }
        public int SectionsOffset { get; set; }
        public int VersesOffset { get; set; }

        public Counters()
        {
            ByteOffset = 0;
            CharOffset = 0;
            BooksOffset = 0;
            ChaptersOffset = 0;
            SectionsOffset = 0;
            VersesOffset = 0;
        }

        public Counters(int byteOffset, int charOffset, int bookId, int chapterId, int sectionId, int verseId)
        {
            ByteOffset = byteOffset;
            CharOffset = charOffset;
            BooksOffset = bookId;
            ChaptersOffset = chapterId;
            SectionsOffset = sectionId;
            VersesOffset = verseId;
        }
    }
}
