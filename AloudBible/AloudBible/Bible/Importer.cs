using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace AloudBible.Bible
{
    public enum ImporterState
    {
        None,
        ContentAwaited,
        ExtraAwaited,
        IntroductionAwaited,
        AuthorAwaited,
        BriefAwaited
    }

    public class Importer
    {
        public static int LeastLineLength = 1;
        public static Boolean ExtraAwailable = true;

        public Work Books { get; private set; }
        public CulturedConfig Culture { get; private set; }
        bool isFirst = false;
        bool withBookTitle = false;
        //bool withBookTitleRepeated = false;
        bool withVerses = false;
        bool withVerseIndex = false;
        bool withChapters = false;
        bool withParagraph = false;

        private String introductionKey, authorKey, briefKey;
        bool withIntroduction = false;
        bool withAuthor = false;
        bool withBrief = false;

        public LineTemplate bookTitleTemplate { get; private set; }
        public LineTemplate chapterTemplate { get; private set; }
        public LineTemplate paragraghTemplate { get; private set; }
        public LineTemplate verseTemplate { get; private set; }
        public LineTemplate verseIndexTemplate { get; private set; }

        public Importer(Work books, CulturedConfig culture)
        {
            Books = books;
            Culture = culture;
            isFirst = true;

            if (culture.Reserved != null)
            {
                introductionKey = culture.Reserved[0];
                authorKey = culture.Reserved[1];
                briefKey = culture.Reserved[2];
            }
        }

        public Book Load(BibleCode code, string fileName)
        {
            Book theBook = Books.AddBook(code); //new Book(Books, code);
            string[] lines = null;
            string line = null;
            int number = -1;
            Chapter chapter = null;
            Section paragraph = null;
            //Verse verse = null;
            ImporterState state = ExtraAwailable ? ImporterState.ExtraAwaited : ImporterState.ContentAwaited;
            string introduction = null, author = null, brief = null;

            #region Get all lines from the file

            using (StreamReader sr = new StreamReader(fileName, Books.Culture.TheEncoding))
            {
                string context = sr.ReadToEnd();

                lines = context.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                lines = (from s in lines
                         let t=s.Trim()
                         where t.Length >= LeastLineLength
                         select t.Trim()).ToArray();
            }

            #endregion

            if (isFirst)
            {
                #region Parse first book of a collection of books share the same format
                if (Culture.IsBookName(lines[0], code, ref line))
                {
                    withBookTitle = true;
                    Debug.WriteLine("The book name is " + line);

                    //number = lines.Count() - 1;

                    lines = (from s in lines
                             let trim = s.TrimStart()
                             where !trim.StartsWith(line)
                             select trim).ToArray();

                    //withBookTitleRepeated = (number != lines.Count());
                }

                BibleCode kind = BibleCode.Unknown;
                int lineCount = lines.Count();
                string text = null;

                for (int i = 0; i < lineCount; i++)
                {
                    text = lines[i].Replace("　", "");
                    if (state == ImporterState.ExtraAwaited)
                    {
                        if (text.Equals(introductionKey, StringComparison.CurrentCultureIgnoreCase))
                        {
                            withIntroduction = true;
                            state = ImporterState.IntroductionAwaited;
                            continue;
                        }
                        else if (text.Equals(authorKey, StringComparison.CurrentCultureIgnoreCase))
                        {
                            withAuthor = true;
                            state = ImporterState.AuthorAwaited;
                            continue;
                        }
                        else if (text.Equals(briefKey, StringComparison.CurrentCultureIgnoreCase))
                        {
                            withBrief = true;
                            state = ImporterState.BriefAwaited;
                            continue;
                        }
                    }
                    else if (state == ImporterState.IntroductionAwaited)
                    {
                        if (introduction == null)
                        {
                            introduction = lines[i];
                            continue;
                        }
                        else if (text.Equals(authorKey, StringComparison.CurrentCultureIgnoreCase))
                        {
                            withAuthor = true;
                            state = ImporterState.AuthorAwaited;
                            continue;
                        }
                        else if (!Culture.IsContentIndicator(lines[i]))
                        {
                            introduction += lines[i];
                            continue;
                        }
                        else
                        {
                            state = ImporterState.ContentAwaited;
                        }
                    }
                    else if (state == ImporterState.AuthorAwaited)
                    {
                        if (author == null)
                        {
                            author = lines[i];
                            continue;
                        }
                        else if (text.Equals(briefKey, StringComparison.CurrentCultureIgnoreCase))
                        {
                            withBrief = true;
                            state = ImporterState.BriefAwaited;
                            continue;
                        }
                        else if (!Culture.IsContentIndicator(lines[i]))
                        {
                            author += lines[i];
                            continue;
                        }
                        else
                        {
                            state = ImporterState.ContentAwaited;
                        }
                    }
                    else if (state == ImporterState.BriefAwaited)
                    {
                        if (brief == null)
                        {
                            brief = lines[i];
                            continue;
                        }
                        else if (!Culture.IsContentIndicator(lines[i]))
                        {
                            brief += lines[i];
                            continue;
                        }
                        else
                        {
                            state = ImporterState.ContentAwaited;
                        }
                    }

                    if (Culture.TryParseBookContent(lines[i], ref kind, ref text, ref number))
                    {
                        switch(kind)
                        {
                            case BibleCode.Verses:
                                if (!withVerses)
                                {
                                    withVerses = true;
                                    verseTemplate = Culture[BibleCode.Verses];
                                    verseIndexTemplate = Culture[BibleCode.BookIndicator];
                                    withVerseIndex = verseIndexTemplate != null;
                                }

                                if (chapter == null || number <= chapter.Verses.Count)
                                {
                                    int chapterNum = -1;
                                    string indicatorStr = lines[i].Substring(0, lines[i].IndexOf(verseTemplate.Splitter));
                                    if (withVerseIndex && verseIndexTemplate.IsCompliant(indicatorStr, code, ref chapterNum))
                                    {
                                        chapter = theBook.AddChapter(chapterNum);
                                    }
                                    else
                                        throw new Exception();
                                }

                                chapter.AddVerse(number, text);
                                //verse = new Verse(chapter, number, buffer);
                                break;
                            case BibleCode.Paragraphs:
                                if (!withParagraph)
                                {
                                    withParagraph = true;
                                    paragraghTemplate = Culture[BibleCode.Paragraphs];
                                }

                                paragraph = chapter.AddSection(text);

                                //if (chapter != null && chapter.Parent == theBook)
                                //{
                                //    paragraph = new Paragraph(chapter, text);
                                //}
                                //else
                                //    throw new Exception(); 
                                break;
                            case BibleCode.Chapters: 
                                if (!withChapters)
                                {
                                    withChapters = true;
                                    chapterTemplate = Culture[BibleCode.Chapters];
                                }

                                //if (number != theBook.Chapters.Count + 1)
                                //    throw new Exception();

                                //if (chapter != null)
                                //    chapter.EndChapter();

                                chapter = theBook.AddChapter(number);
                                break;
                            default:
                                throw new Exception();
                        }
                    }
                    else
                        throw new Exception("Failed to parse: " + line);

                }
                isFirst = false;
                #endregion
            }
            else
            {
                #region Handling when template available to take different strategies
                if (withBookTitle && !Culture.IsBookName(lines[0], code, ref line))
                    throw new Exception("Failed to parse book name from: " + lines[0]);
                else
                {
                    lines = (from s in lines
                             let trim = s.TrimStart()
                             where !trim.StartsWith(line)
                             select trim).ToArray();
                }

                int lineCount = lines.Count();
                string text = null;

                if (withChapters)
                {
                    #region Handling when chapters as good divident
                    List<int> chapterIndexes = new List<int>();

                    for (int i = 0; i < lineCount; i ++)
                    {
                        if (chapterTemplate.IsCompliant(lines[i], out number))
                        {
                            chapterIndexes.Add(i);
                        }
                    }

                    if (chapterIndexes.Count == 0)
                    {
                        line = lines[0];
                        if (!Culture.NumberParser.TryParse(line, out number))
                            throw new Exception("Something must be wrong with the Chapter lines like " + line);

                        string preposition = Culture.Prepositions.Find(x => Culture.ContainsKey(line, x));
                        string indicator = Culture.BookNames[BibleCode.Chapters].First(x => Culture.ContainsKey(line, x));
                        string numberString = Culture.NumberParser.NumberStringOf(line);

                        LineTemplate tempChapterTemplate = new LineTemplate(BibleCode.Chapters, Culture, line, numberString, preposition, indicator);

                        for (int i = 0; i < lineCount; i++)
                        {
                            if (tempChapterTemplate.IsCompliant(lines[i], out number))
                            {
                                chapterIndexes.Add(i);
                            }
                        }
                    }
                    
                    if (chapterIndexes[0] == 0)
                        chapterIndexes.Add(lineCount);
                    else
                        throw new NotImplementedException("How to interpret " + lines[0]);

                    for (int ch = 0; ch < chapterIndexes.Count - 1; ch ++ )
                    {
                        if (ch != theBook.Chapters.Count)
                            throw new Exception("Miss or mismatch of chapter " + ch.ToString());

                        int first = chapterIndexes[ch] + 1;
                        int last = chapterIndexes[ch + 1] - 1;

                        //if (chapter != null)
                        //    chapter.EndChapter();

                        chapter = theBook.AddChapter(ch + 1);

                        for (int j = first; j <= last; j++)
                        {
                            if (verseTemplate.IsCompliant(lines[j], ref text, ref number))
                            {
                                chapter.AddVerse(number, text);
                            }
                            else if (withParagraph && paragraghTemplate.IsCompliant(lines[j], ref text))
                            {
                                //paragraph = new Paragraph(chapter, text);
                                paragraph = chapter.AddSection(text);
                            }
                            else
                                throw new Exception("Failed to parse " + lines[j]);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Handling when chapter index is leading each verses
                    for (int j = 0; j < lineCount; j++)
                    {
                        if (chapter == null || number <= chapter.Verses.Count)
                        {
                            if (verseTemplate.IsCompliant(lines[j], ref text, ref number))
                            {
                                if (number == 1)
                                {
                                    int chapterNum = -1;
                                    string indicatorStr = lines[j].Substring(0, lines[j].IndexOf(verseTemplate.Splitter));

                                    if (withVerseIndex && verseIndexTemplate.IsCompliant(indicatorStr, code, ref chapterNum))
                                    {
                                        //if (chapter != null)
                                        //    chapter.EndChapter();

                                        chapter = theBook.AddChapter(chapterNum);

                                    }
                                    else
                                        throw new Exception();
                                }
                                else if (number != chapter.Verses.Count + 1)
                                    throw new Exception();

                                //verse = new Verse(chapter, number, buffer);
                                chapter.AddVerse(number, text);
                            }
                            else if (withParagraph && paragraghTemplate.IsCompliant(lines[j], ref text))
                            {
                                //paragraph = new Paragraph(chapter, text);
                                paragraph = chapter.AddSection(text);
                            }
                            else
                                throw new Exception("Failed to parse: " + lines[j]);
                        }

                    }
                    #endregion
                }
                #endregion
            }

            theBook.EndLoading();

            return theBook;
        }
    }
}
