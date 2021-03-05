using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utitilties.NumberParser;
using System.Diagnostics;

namespace AloudBible.Bible
{
    public enum LineItemType
    {
        None,
        BookIndicator,
        BookName,
        Leading,
        Preposition,
        Number,
        Indicator,
        Text
    }

    public class LineTemplate
    {
        private static char[] spaces =new char[] {' ', ':', '-', '\t'};

        public BibleCode Code { get; private set; }

        public CulturedConfig Culture { get; private set; }

        public NumberWordConverter NumberParser { get { return Culture.NumberParser; } }

        public Dictionary<LineItemType, string> LineItems = new Dictionary<LineItemType, string>();

        public string Splitter { get; private set; }

        public List<LineItemType> Sequences { get; private set; }

        public LineItemType this[int index]
        {
            get { return (index >= 0 && index < Sequences.Count) ? Sequences[index] : LineItemType.None; }
        }

        public int LeadingLength { get; private set; }

        public int NumberPos { get; private set; }

        private LineTemplate(BibleCode code, CulturedConfig culture)
        {
            Code = code;
            Culture = culture;
            Sequences = new List<LineItemType>();
        }

        public LineTemplate(BibleCode code, CulturedConfig culture, string line, string numberString, string preposition, string indicator)
            : this(code, culture)
        {

            int prepositionIndex = -1, indicatorIndex = -1, numberIndex = -1;
            List<int> indexes = new List<int>();

            if (numberString != null && numberString != "")
            {
                LineItems.Add(LineItemType.Number, numberString);
                numberIndex = line.IndexOf(numberString);
                indexes.Add(numberIndex);
            }

            if (preposition != null && preposition != "")
            {
                LineItems.Add(LineItemType.Preposition, preposition);
                prepositionIndex = line.IndexOf(preposition);
                indexes.Add(prepositionIndex);
            }

            if (indicator != null && indicator != "")
            {
                LineItems.Add(LineItemType.Indicator, indicator);
                indicatorIndex = line.IndexOf(indicator);
                indexes.Add(indicatorIndex);
            }

            indexes.Sort();

            if (indexes.Count != 0 && indexes[0] != 0)
            {
                LineItems.Add(LineItemType.Leading, line.Substring(0, indexes[0]));
                Sequences.Add(LineItemType.Leading);
                LeadingLength = indexes[0];
            }
            else
                LeadingLength = 0;

            for (int i = 0; i < indexes.Count; i ++)
            {
                if (indexes[i] == numberIndex)
                    Sequences.Add(LineItemType.Number);
                else if (indexes[i] == indicatorIndex)
                    Sequences.Add(LineItemType.Indicator);
                else if (indexes[i] == prepositionIndex)
                    Sequences.Add(LineItemType.Preposition);
            }

            if (Sequences.Count != 0)
            {
                string lastSection = LineItems[Sequences.Last()];
                int end = line.IndexOf(lastSection) + lastSection.Length;
                string remained = line.Substring(end, line.Length - end).Trim();

                if (remained.Length != 0 && remained != "")
                {
                    Sequences.Add(LineItemType.Text);
                }

                NumberPos = Sequences.LastIndexOf(LineItemType.Number);
            }
        }

        public LineTemplate(BibleCode code, CulturedConfig culture, string line, string numberString)
            : this(code, culture)
        {
            if (code != BibleCode.Verses)
                throw new Exception();

            #region Codes obsoleted on Dec 07, 2011
            //string[] sections = trimed.Split(CulturedConfig.DefaultVerseSplitter, StringSplitOptions.RemoveEmptyEntries);

            //int num, numPos = -1;
            //for (int ch = 0; ch < sections.Count(); ch ++)
            //{
            //    string section = sections[ch];
            //    if (int.TryParse(section, out num))
            //    {
            //        Sequences.Add(LineItemType.Number);
            //    }
            //    else if (ch == 0)
            //    {
            //        numPos = trimed.IndexOf(numberString);
            //        Sequences.Add(LineItemType.Leading);
            //        LineItems.Add(LineItemType.Leading, numberString);
            //        break;
            //    }
            //    else
            //        break;
            //}

            //int whitePos = trimed.IndexOfAny(spaces, numPos + numberString.ContentLength);
            //int endPos = whitePos+1;
            //while(spaces.Contains(trimed[endPos])) { endPos ++; }
            //Splitter = trimed.Substring(whitePos, endPos - whitePos);
            #endregion

            int indexPos = line.LastIndexOf(numberString);

            if (indexPos == -1)
                throw new Exception();

            string label = line.Substring(0, indexPos + numberString.Length);
            string[] sections = label.Split(CulturedConfig.DefaultVerseSplitter, StringSplitOptions.RemoveEmptyEntries);
            int sectionNum = sections.Count();

            if (sectionNum != 1)
            {
                int num;
                for (int i = 0; i < sectionNum - 1; i ++ )
                {
                    if (int.TryParse(sections[i], out num))
                        Sequences.Add(LineItemType.Number);
                    else
                    {
                        foreach (string key in culture.ReverseNames.Keys)
                        {
                            if(sections[i].StartsWith(key) && int.TryParse(sections[i].Substring(key.Length, sections[i].Length-key.Length), out num))
                            {
                                Sequences.Add(LineItemType.BookIndicator);
                                //Sequences.Add(LineItemType.Number);
                                break;
                            }
                        }

                        if (Sequences.Count == 0)
                            Sequences.Add(LineItemType.Leading);

                        int last = line.IndexOf(sections[sectionNum - 2]) + sections[sectionNum - 2].Length;
                        int len = line.LastIndexOf(numberString) - last;
                        Splitter = line.Substring(last, len);
                    }
                }
            }
            Sequences.Add(LineItemType.Number);

            int textPos = indexPos + numberString.Length;
            LeadingLength = 0;
            while (CulturedConfig.DefaultVerseSplitter.Contains(line[textPos]))
            {
                textPos++;
                LeadingLength++; 
            }

            Sequences.Add(LineItemType.Text);
            NumberPos = Sequences.LastIndexOf(LineItemType.Number);

        }

        public LineTemplate(BibleCode code, CulturedConfig culture, string line)
            : this(code, culture)
        {
            if (code != BibleCode.BookIndicator || Culture[BibleCode.Verses] == null)
                throw new Exception();

            string bookname = null;
            int booknameIndex = -1;
            foreach (string key in culture.ReverseNames.Keys)
            {
                if(line.ToLower().Contains(key))
                {
                    bookname = key;
                    booknameIndex = line.ToLower().IndexOf(key);
                    break;
                }
            }
            
            if (booknameIndex != 0)
            {
                Sequences.Add(LineItemType.Leading);
                LeadingLength = booknameIndex;
                LineItems.Add(LineItemType.Leading, line.Substring(0, booknameIndex));
            }

            Sequences.Add(LineItemType.BookName);

            string remained = line.Substring(booknameIndex + bookname.Length, line.Length - booknameIndex - bookname.Length);

            if (int.TryParse(remained, out booknameIndex))
            {
                Sequences.Add(LineItemType.Number);
                Splitter = CulturedConfig.DefaultVerseSplitter.Contains(remained[0]) ? remained[0].ToString() : "";
            }
            else
                throw new Exception();

        }

        public LineTemplate(BibleCode code, CulturedConfig culture, string line, BibleCode likely)
            : this(code, culture)
        {
            if (code != BibleCode.Books)
                throw new Exception();

            string lowerLine = culture.CharAsString ? line : line.ToLower();
            string name = null;

            foreach(string bookName in Culture.BookNames[likely])
            {
                if (lowerLine.Contains(bookName))
                {
                    Sequences.Add(LineItemType.BookIndicator);
                    name = bookName;
                    break;
                }
            }

            int first = lowerLine.IndexOf(name);

            if (first != 0)
            {
                LeadingLength = first;
                Sequences.Insert(0, LineItemType.Leading);
                LineItems.Add(LineItemType.Leading, line.Substring(0, first));
            }

            if (first + name.Length < line.Length)
            {
                Sequences.Add(LineItemType.Text);
                LineItems.Add(LineItemType.Text, line.Substring(first + name.Length, line.Length - first - name.Length));
            }
            NumberPos = Sequences.LastIndexOf(LineItemType.Number);
        }

        public bool IsCompliant(string line, BibleCode likely)
        {
            if (likely < BibleCode.Genesis || likely > BibleCode.Revelation)
                throw new Exception();
            
            if (Sequences.Contains(LineItemType.Leading))
            {
                if (!line.StartsWith(LineItems[LineItemType.Leading]))
                    return false;
            }

            if (Sequences.Contains(LineItemType.Text) && !line.EndsWith(LineItems[LineItemType.Text]))
                return false;

            string nameString = line.Substring(LeadingLength, line.Length - LeadingLength);

            if (!Culture.CharAsString)
                nameString = nameString.ToLower();

            foreach (string name in Culture.BookNames[likely])
            {
                if (nameString.Contains(name))
                    return true;
            }

            return false;

        }

        public bool IsCompliant(string line, out int number)
        {
            number = -1;

            string trimed = line.Trim();

            if (Code == BibleCode.Chapters)
            {
                if (Sequences.Contains(LineItemType.Indicator) && !trimed.Contains(LineItems[LineItemType.Indicator]))
                    return false;
                else if (Sequences.IndexOf(LineItemType.Indicator) == Sequences.Count-1 && !trimed.EndsWith(LineItems[LineItemType.Indicator]))
                    return false;

                if (Sequences.Contains(LineItemType.Preposition) && !trimed.Contains(LineItems[LineItemType.Preposition]))
                    return false;
                else if (Sequences.IndexOf(LineItemType.Preposition) == 0 && !trimed.StartsWith(LineItems[LineItemType.Preposition]))
                    return false;

                if (Sequences.Contains(LineItemType.Number) && !NumberParser.TryParse(trimed, out number))
                    return false;

                return true;
            }
            else
                return false;
        }

        public bool IsCompliant(string line, ref string text)
        {
            if (Code == BibleCode.Paragraphs)
            {
                if (Sequences.Contains(LineItemType.Preposition) && line.StartsWith(LineItems[LineItemType.Preposition]))
                {
                    text = line.Substring(LeadingLength, text.Length - LeadingLength);
                    return true;
                }
                else if (Sequences.Count == 0)
                {
                    text = line.Trim();
                    return true;
                }
            }

            return false;
        }

        public bool IsCompliant(string line, ref string text, ref int number)
        {
            int indexPos = -1;
            if (Splitter != null && Splitter != "" && (indexPos = line.IndexOf(Splitter)) == -1)
                return false;

            string[] sections = line.Split(CulturedConfig.DefaultVerseSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (NumberPos != -1 && int.TryParse(sections[NumberPos], out indexPos))
            {
                number = indexPos;
                indexPos = line.IndexOf(sections[NumberPos])+LeadingLength + sections[NumberPos].Length;
                text =(line.Length > indexPos) ? line.Substring(indexPos, line.Length - indexPos) : "";
                text = text.Trim();
                return true;
            }

            return false;
        }

        public bool IsCompliant(string line, BibleCode book, ref int number)
        {
            if (this.Code != BibleCode.BookIndicator || book < BibleCode.Genesis || book > BibleCode.Revelation)
                throw new Exception();

            if (Sequences.Contains(LineItemType.Leading))
            {
                line = line.Substring(LeadingLength, line.Length - LeadingLength);
            }

            if (Splitter != "")
            {
                string[] sections = line.Split(Splitter.ToCharArray());
                string bookname = sections[Sequences.IndexOf(LineItemType.BookName)].ToLower();
                string numString = sections[Sequences.IndexOf(LineItemType.Number)];
                if (Culture.ReverseNames.ContainsKey(bookname))
                {
                    if (int.TryParse(numString, out number))
                        return true;
                    else
                        Debug.WriteLine("Failed to parse " + numString);
                }
                else
                    return false;
            }

            foreach (string key in Culture.BookNames[book])
            {
                if (line.ToLower().Contains(key))
                {
                    string numString =line.Substring(key.Length, line.Length - key.Length);
                    if (int.TryParse(numString, out number))
                        return true;
                    else
                        Debug.WriteLine("Failed to parse " + numString);
                }
            }

            return false;
        }
    }
}
