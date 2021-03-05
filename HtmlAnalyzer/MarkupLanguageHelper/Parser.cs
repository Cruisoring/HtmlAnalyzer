using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HtmlAnalyzer.GenericPatternIndexer;

namespace HtmlAnalyzer.MarkupLanguageHelper
{
    public class Parser
    {
        public const char LessThan = '<';
        public const char GreaterThan = '>';
        public const char Slash = '/';
        public const char Exclamation = '!';

        //private List<int> lessThanPositions = new List<int>();
        //private List<int> greaterThanPositions = new List<int>();
        //private Queue<int> lessThanPositions = new Queue<int>();
        //private Queue<int> greaterThanPositions = new Queue<int>();
        //private List<int> slashPositions = new List<int>();
        //private List<int> endingTagPositions = new List<int>();

        //Dictionary<string, List<Tag>> parsedSectors = new Dictionary<string, List<Tag>>();

        //public Dictionary<int, TagLabel> endingTagDict = new Dictionary<int, TagLabel>();

        //public TagsMap Map { get; set; }

        public Uri Address { get; private set; }

        public char[] CharArray { get; private set; }

        //string content = null;

        TagParser helper = null;

        Encoding theEncoding = null;

        public Parser(Uri address)
            : this(address, Encoding.UTF8)
        { }

        public Parser(Uri address, Encoding assumedEncoding)
        {
            Address = address;
            byte[] buffer = null;

            if (address.IsFile && File.Exists(address.LocalPath))
            {
                FileInfo fi = new FileInfo(address.LocalPath);
                FileStream fs = new FileStream(address.LocalPath, FileMode.Open, FileAccess.Read);

                int length = (int)(fi.Length);
                buffer = new byte[length];

                fs.Read(buffer, 0, length);
                fs.Close();
            }
            else
                throw new NotImplementedException();

            theEncoding = (assumedEncoding == null) ? Encoding.UTF8 : assumedEncoding;
            if (DocTypeDetector.TryGetTagHelperOf(buffer, out helper, ref theEncoding))
            {
                CharArray = theEncoding.GetChars(buffer);
            }
        }

        public Document Root
        {
            get
            {
                if (helper == null)
                    throw new Exception("TagParser is not determined yet!");

                List<Scope> splitRanges = Finder.ScopesOfPattern(CharArray, "<", ">");

                List<Element> elements = Element.HtmlElementsOf(CharArray, splitRanges, true);

                //foreach (Element el in elements[0].Children)
                //{
                //    //showOutText(el);
                //    Console.WriteLine(el.AllText);
                //}

                Document doc = null;

                //if (elements.Count == 1)
                //{
                //    doc = new Document(Address, elements[0], theEncoding);
                //}
                //else
                //    doc = new Document(Address, new Element(new TextScope(CharArray, 0, CharArray.Length), elements), theEncoding);
                return doc;
            }
        }

        private void showOutText(Element el)
        {
            if (el.OwnTextScopes != null)
                Console.WriteLine(el.Name + ": " + el.OwnText);

            if (el.Children != null)
            {
                foreach(Element child in el.Children)
                {
                    Console.Write("\t");
                    showOutText(child);
                }
            }
        }

        #region Codes Obsoleted

        //public Parser(byte[] buffer)
        //{
        //    Map = new TagsMap();

        //    Encoding theEncoding = Encoding.Default;
        //    if (Utility.EncodingAdapter.TryParse(buffer, ref theEncoding, ref content))
        //    {
        //        Map.TheEncoding = theEncoding;
        //    }
        //    else
        //        Map.TheEncoding = Encoding.Default;

        //    if (content == null)
        //        content = Map.TheEncoding.GetString(buffer);

        //    CharArray = content.ToCharArray();
        //}


            //int lastEnding, leading, ending, nextLeading, nextEndingIndex = 0;
            //for (int i = 0, j = 0; i < openingIndexes.Count; i++)
            //{
            //    nextLeading = openingIndexes[i];
            //    lastEnding = closingIndexes[j];

            //    if (undertermined.Count == 0 || lastEnding > nextLeading)
            //    {
            //        undertermined.Push(nextLeading);
            //        continue;
            //    }

            //    //nextLeading shall always be greater than lastEnding
            //    if (lastEnding == nextLeading)
            //        throw new Exception();

            //    do
            //    {
            //        leading = undertermined.Pop();

            //        ending = closingIndexes[nextEndingIndex++];

            //        if (leading < ending)
            //        {
            //            Scope newMatch = new Scope(leading, ending - leading + endingPetternLength);
            //            result.Add(newMatch);
            //        }
            //        else
            //            throw new Exception();
            //    } while (undertermined.Count != 0 && nextEndingIndex <= j);

            //    j++;
            //    undertermined.Push(nextLeading);
            //}

            //if (undertermined.Count() == 0 || nextEndingIndex == closingIndexes.Count)
            //    return result;
            //else if (undertermined.Count() == 1 && nextEndingIndex == closingIndexes.Count - 1)
            //{
            //    leading = undertermined.Pop();

            //    ending = closingIndexes[nextEndingIndex++];

            //    if (leading < ending)
            //    {
            //        Scope newMatch = new Scope(leading, ending - leading + endingPetternLength);
            //        result.Add(newMatch);
            //        return result;
            //    }
            //    else
            //        throw new Exception();
            //}
            //else
            //    throw new Exception();
        /*/
        public void Parse2()
        {
            char theByte;
            for (int i = 0; i < CharArray.Length; i ++ )
            {
                theByte = CharArray[i];
                if (theByte == LessThan)
                {
                    lessThanPositions.Add(i);
                    if (i != CharArray.Length - 1 && (CharArray[i + 1] == Slash || CharArray[i+1] == Exclamation))
                        endingTagPositions.Add(i);
                }
                else if (theByte == GreaterThan)
                {
                    greaterThanPositions.Add(i);
                    if (i != 0 && CharArray[i - 1] == Slash)
                        endingTagPositions.Add(i);
                }
                //else if (theByte == Slash)
                //    slashPositions.Add(i);
            }

            //Queue<int> theQueue;
            string tagName = null;
            TagKind kind = TagKind.Unknown;

            TextScope scope = null;

            for (int i = 0; i < endingTagPositions.Count; i ++ )
            {
                int posA =endingTagPositions[i];
                char ch = CharArray[posA];
                bool foundEndingTag = false;

                TagLabel newLabel = null;

                if (ch == LessThan)
                {
                    int nextIndex = greaterThanPositions.FindIndex(x => x > posA);

                    do
                    {
                        int end = greaterThanPositions[nextIndex];
                        scope = new TextScope(CharArray, posA, end + 1 - posA);

                        string word = scope.RawText;

                        if (helper.IsValidTag(word, ref tagName, ref kind))
                        {
                            newLabel = new TagLabel(scope, tagName, kind);
                            endingTagDict.Add(scope.First, newLabel);
                            foundEndingTag = true;
                            greaterThanPositions.RemoveAt(nextIndex);
                            break;
                        } 
                        
                        nextIndex++;
                    } while (nextIndex < greaterThanPositions.Count);

                }
                else if (ch == GreaterThan)
                {
                    int prevIndex = lessThanPositions.FindLastIndex(x => x < posA);

                    do
                    {
                        int opening = lessThanPositions[prevIndex];
                        scope = new TextScope(CharArray, opening, posA + 1 - opening);

                        string word = scope.RawText;

                        if(helper.IsValidTag(word, ref tagName, ref kind))
                        {
                            newLabel = new TagLabel(scope, tagName, kind);
                            endingTagDict.Add(scope.First, newLabel);
                            foundEndingTag = true;
                            lessThanPositions.RemoveAt(prevIndex);
                            break;
                        }

                        prevIndex--;
                    } while (prevIndex >= 0);

                }
                else
                    throw new NotImplementedException();

                

                if (!foundEndingTag)
                    throw new NotImplementedException();
            }
        }
        //*/
        #endregion

    }
}
