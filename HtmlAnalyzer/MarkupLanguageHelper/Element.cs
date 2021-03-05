using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlAnalyzer.MarkupLanguageHelper
{
    [Flags]
    public enum ElementType
    {
        Simple =        0x00000000,
        Compound =      0x10000000,
        Declaration=    0x01000000,
        WithChildren =  0x00000001,
        WithOwnText =   0x00000100,
        WithInnerText = 0x00000200
    }

    public class Element : TextScope, IComparable<Element>
    {
        #region Definitions
        public const char LessThan = '<';
        public const char GreaterThan = '>';
        public const char Slash = '/';
        public const char Exclamation = '!';
        public const char QuestionMark = '?';

        public static char[] WhiteSpaces = {' ', '\t', '\r', '\n'};

        public static char TagLeading = LessThan;
        public static char TagEnding = GreaterThan;

        private static char[] unexpectedTagChars = new char[] { ' ', '<', '>' };

        public static Dictionary<string, string> ReservedHtmlEntities = new Dictionary<string, string>()
        {
            {"&nbsp;", " "},         //non-breaking space
            {"&#160;", " "},       //non-breaking space    
            {"&lt;", "<"},       //less than    
            {"&#60;", "<"},       //less than    
            {"&gt;", ""},       //greater than   
            {"&#62;", ">"},       //greater than
            {"&amp;", "&"},       //ampersand
            {"&#38;", "&"},       //ampersand
            {"&cent;", ""},       //cent
            {"&#162;", "¢"},       //cent
            {"&pound;", "£"},       //pound
            {"&#163;", "£"},       //pound
            {"&yen;", "¥"},       //yen
            {"&#165;", "¥"},       //yen
            {"&euro;", "€"},       //euro
            {"&#8364;", "€"},       //euro
            {"&sect;", "§"},       //section
            {"&#167;", "§"},       //section
            {"&copy;", "©"},       //copyright
            {"&#169;", "©"},       //copyright
            {"&reg;", "®"},       //registered trademark
            {"&#174;", "®"},       //registered trademark
            {"&trade;", "™"},       //trademark
            {"&#8482;", "™"}       //trademark
        };


        #region Transitional Html Specific Definitions
        private static readonly Dictionary<string, List<string>> TagContainer = new Dictionary<string, List<string>>{
            //{ "!--", new List<string>(){} },
            //{ "!DOCTYPE", new List<string>(){} },
            { "a", new List<string>{"", "div", "tr", "form", "body", "html"} },
            { "abbr", new List<string>(){"", "body", "html"} },               //Phrase element
            { "acronym", new List<string>(){"", "body", "html"} },            //Phrase element
            //{ "address", new List<string>(){} },
            { "applet", new List<string>(){"", "body", "html"} },
            //{ "area", new List<string>(){} },
            { "b", new List<string>(){"", "body", "html"} },                  //font style elements
            { "base", new List<string>(){"", "head", "html"} },
            //{ "basefont", new List<string>(){} },
            //{ "bdo", new List<string>(){} },
            { "big", new List<string>(){"", "body", "html"} },                //font style elements
            //{ "blockquote", new List<string>(){} },
            { "body", new List<string>(){"html"} },
            //{ "br", new List<string>(){} },
            { "button", new List<string>(){} },
            { "caption", new List<string>(){"", "table", "body", "html"} },
            { "center", new List<string>(){"", "body", "html"} },
            { "cite", new List<string>(){"", "body", "html"} },               //Phrase element
            { "code", new List<string>(){"", "body", "html"} },               //Phrase element
            { "col", new List<string>(){"", "colgroup", "table", "body", "html"} },
            { "colgroup", new List<string>(){"", "table", "body", "html"} },
            { "dd", new List<string>(){"", "dl", "body", "html"} },
            { "del", new List<string>(){"", "body", "html"} },                //Markup of deleted word 
            { "dfn", new List<string>(){"", "body", "html"} },                //Phrase element
            { "dir", new List<string>(){"", "body", "html"} },
            { "div", new List<string>(){"", "body", "html"} },
            { "dl", new List<string>(){"", "body", "html"} },
            { "dt", new List<string>(){"", "dl", "body", "html"} },
            { "em", new List<string>(){"", "body", "html"} },                 //Phrase element
            //{ "fieldset", new List<string>(){} },
            //{ "font", new List<string>(){} },
            { "form", new List<string>(){"", "table", "body", "html"} },
            { "frame", new List<string>(){"frameset", "frame"} },
            { "frameset", new List<string>(){"frameset"} },
            { "head", new List<string>(){"html" } },
            { "h1", new List<string>(){"", "body", "html"} },
            { "h2", new List<string>(){"", "body", "html"} },
            { "h3", new List<string>(){"", "body", "html"} },
            { "h4", new List<string>(){"", "body", "html"} },
            { "h5", new List<string>(){"", "body", "html"} },
            { "h6", new List<string>(){"", "body", "html"} },
            //{ "hr", new List<string>(){} },
            { "html", new List<string>(){"html"} },
            { "i", new List<string>(){"", "body", "html"} },                //font style elements
            //{ "iframe", new List<string>(){} },
            //{ "img", new List<string>(){} },
            //{ "input", new List<string>(){} },
            { "ins", new List<string>(){"", "body", "html"} },                //Markup of deleted word 
            { "kbd", new List<string>(){"", "body", "html"} },                //Phrase element
            //{ "scope", new List<string>(){} },
            { "legend", new List<string>(){"fieldset", "body", "html"} },
            { "li", new List<string>(){"ul", "ol ", "dir", "body", "html"} },
            { "link", new List<string>(){"head", "html"} },
            //{ "map", new List<string>(){} },
            //{ "menu", new List<string>(){} },
            //{ "meta", new List<string>(){} },
            //{ "noframes", new List<string>(){} },
            //{ "noscript", new List<string>(){} },
            { "object", new List<string>(){"body", "html"} },
            { "ol", new List<string>(){"", "body", "html"} },
            { "optgroup", new List<string>(){"select"} },
            { "option", new List<string>(){"select", "form", "body", "html"} },
            { "p", new List<string>(){"body", "html"} },
            { "param", new List<string>(){"object"} },
            { "pre", new List<string>(){"", "body", "html"} },                    //indicating pre-formatted word.
            { "q", new List<string>(){"", "body", "html"} },
            //{ "s", new List<string>(){} },
            { "samp", new List<string>(){"", "body", "html"} },                   //Phrase element
            { "script", new List<string>(){"body", "html"} },
            { "select", new List<string>(){"form", "body", "html"} },
            { "small", new List<string>(){"", "body", "html"} },                  //font style elements
            //{ "span", new List<string>(){} },
            { "strike", new List<string>(){"", "body", "html"} },                 //Markup of striked word 
            { "strong", new List<string>(){"", "body", "html"} },                 //Phrase element
            { "style", new List<string>(){"body", "html"} },
            { "sub", new List<string>(){"", "body", "html"} },                    //defining subscript word.
            { "sup", new List<string>(){"", "body", "html"} },                    //defining superscript word.
            { "table", new List<string>(){"", "body", "html"} },
            { "tbody", new List<string>(){"", "table", "body", "html"} },
            { "td", new List<string>(){"", "tr", "table", "body", "html"} },
            { "textarea", new List<string>(){"", "form", "body", "html"} },
            { "tfoot", new List<string>(){"", "table", "body", "html"} },
            { "th", new List<string>(){"", "table", "body", "html"} },
            { "thead", new List<string>(){"", "table", "body", "html"} },
            { "title", new List<string>(){"", "head", "html"} },
            { "tr", new List<string>(){"", "table", "body", "html"} },
            { "tt", new List<string>(){"", "body", "html"} },                     //font style elements
            { "u", new List<string>(){"", "body", "html"} },                      //underline word
            { "ul", new List<string>(){"", "body", "html"} },
            //{ "var", new List<string>(){} },
            { "wbr", new List<string>(){"", "body", "html"} },                    //Phrase element
            //{ "xm", new List<string>(){} } 
        };
        #endregion

        #region Tags whose word shall be neglected

        List<string> NeglectTextTags = new List<string>
        {
            "applet",
             "botton", //???????
             "dir",
             "frameset",
             "frame",
             "head",
             "link",
             "param",
             "ol",
             "optgroup",
             "script",
             "select",
             "style",
             "table",
             "tbody",
             "tfoot",
             "thead",
             "tr",
             "ul"
        };

        #endregion

        #endregion

        #region Static functions
        public static List<Element> HtmlElementsOf(Char[] CharArray, List<Scope> segments, bool checkByExpectation)
        {
            List<TagLabel> tags = new List<TagLabel>();
            //bool isMalFormated = false;

            //List<TextScope> texts = new List<TextScope>();
            #region Get the Tag candidates
            //Get all possible Tags
            TagLabel label = null;
            foreach (Scope segment in segments)
            {
                if (TagParser.LooseHtmlHelper.IsValidTag(CharArray, segment, ref label) && label != null)
                {
                    tags.Add(label);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            #endregion

            #region Screen out unexpected tags
            if (checkByExpectation)
            {
                List<TagLabel> undertermined = new List<TagLabel>();
                List<TagLabel> unexpectedTags = new List<TagLabel>();
                List<TagLabel> stack = new List<TagLabel>();
                bool isUnexpectedFound = false;
                int start = -1;
                for (int i = 0; i < tags.Count; i++)
                {
                    label = tags[i];
                    if (undertermined.Count != 0 && TagContainer.ContainsKey(label.TagName) && !TagContainer[label.TagName].Contains(""))
                    {
                        if (!isUnexpectedFound)
                        {
                            if (label.Kind == TagKind.Closing || label.Kind == TagKind.UserDefinedClosing)
                            {       //The Tag can be placed at everywhere
                                int lastMatchIndex = undertermined.FindLastIndex(x => x.TagName == label.TagName);

                                if (lastMatchIndex != -1)
                                {
                                    undertermined.RemoveRange(lastMatchIndex, undertermined.Count - lastMatchIndex);
                                }
                                else
                                    unexpectedTags.Add(label);
                            }
                            else if (TagContainer[label.TagName].Count == 0)    //The Tag can be placed at everywhere
                            {
                                undertermined.Add(label);
                            }
                            else if ((label.Kind != TagKind.Opening && label.Kind != TagKind.UserDefinedOpening) &&
                                TagContainer.ContainsKey(undertermined.Last().TagName) && TagContainer[undertermined.Last().TagName].Contains(label.TagName)) //The Tag's relative position shall be checked
                            {
                                isUnexpectedFound = true;
                                start = label.First;
                                //undertermined.Add(label);
                                stack.Add(label);
                                unexpectedTags.Add(label);
                            }
                            else
                                undertermined.Add(label);
                        }
                        else
                        {
                            //Find out the last tag to close the unexpected part
                            if (label.Kind == TagKind.Closing || label.Kind == TagKind.UserDefinedClosing)
                            {
                                // Still can't avoid cases when the embedded content has unpaired tags, and one closing tag word happen to share the same name with the undertermined[undertermined.Count-2]
                                int lastUnexpected = stack.FindLastIndex(x => x.TagName == label.TagName);

                                if (lastUnexpected != -1)
                                {
                                    unexpectedTags.Add(label);
                                    stack.RemoveRange(lastUnexpected, stack.Count - lastUnexpected);

                                    if (lastUnexpected == 0)
                                    {
                                        isUnexpectedFound = false;
                                        Scope unexpectedScope = new Scope(start, label.Last - start + 1);

                                        for (int j = undertermined.Count - 1; j >= 0; j--)
                                        {
                                            if (unexpectedScope.Covers(undertermined[j]) && !unexpectedTags.Contains(undertermined[j]))
                                                unexpectedTags.Add(undertermined[j]);
                                        }

                                        start = -1;
                                    }
                                }
                                else if (label.TagName == undertermined.Last().TagName)
                                {
                                    undertermined.RemoveAt(undertermined.Count - 1);
                                    isUnexpectedFound = false;
                                    stack.Clear();
                                    Scope unexpectedScope = new Scope(start, label.Last - start + 1);

                                    for (int j = undertermined.Count - 1; j >= 0; j--)
                                    {
                                        if (unexpectedScope.Covers(undertermined[j]) && !unexpectedTags.Contains(undertermined[j]))
                                            unexpectedTags.Add(undertermined[j]);
                                    }

                                    start = -1;
                                }
                                else
                                {
                                    unexpectedTags.Add(label);
                                }
                            }
                            else
                            {
                                unexpectedTags.Add(label);
                                stack.Add(label);
                            }

                        }

                    }
                    else if (label.Kind == TagKind.Closing || label.Kind == TagKind.UserDefinedClosing)
                    {       //The Tag can be placed at everywhere
                        int lastMatchIndex = undertermined.FindLastIndex(x => x.TagName == label.TagName);

                        if (lastMatchIndex != -1)
                        {
                            undertermined.RemoveRange(lastMatchIndex, undertermined.Count - lastMatchIndex);
                        }
                        else
                            unexpectedTags.Add(label);
                    }
                    else
                        undertermined.Add(label);

                }

                if (unexpectedTags.Count != 0)
                {
                    foreach (TagLabel tag in unexpectedTags)
                    {
                        tags.Remove(tag);
                    }
                }
            }

            #endregion

            TagLabel currentTag, opening;
            List<TagLabel> unpairedTags = new List<TagLabel>();
            List<Element> parsedElements = new List<Element>();
            Element higher = null;

            for (int i = 0; i < tags.Count; i++)
            {
                currentTag = tags[i];
                if (currentTag.Kind >= TagKind.Sound)
                {
                    parsedElements.Add(new Element(currentTag));
                    continue;
                }
                else if (unpairedTags.Count == 0)
                {
                    unpairedTags.Add(currentTag);
                    continue;
                }

                if (currentTag.Kind == TagKind.Closing || currentTag.Kind == TagKind.UserDefinedClosing)
                {
                    int matchedIndex = unpairedTags.FindLastIndex(tag => tag.TagName == currentTag.TagName);

                    if (matchedIndex == -1)
                    {
                        //isMalFormated = true;
                        string best = null;

                        for(int x =unpairedTags.Count-1; x >= 0; x--)
                        {
                            best = GenericPatternIndexer.Finder.BestMatchOf(unpairedTags[x].TagName, currentTag.TagName);

                            if (best != null && (double)(best.Length)/unpairedTags[x].TagName.Length > 0.6)
                            {
                                matchedIndex = x;
                                currentTag.TagName = unpairedTags[matchedIndex].TagName;
                                break;
                            }
                        }
                    }

                    List<Element> children = new List<Element>();

                    opening = unpairedTags[matchedIndex];

                    Scope currentScope = new Scope(opening.First, currentTag.Last - opening.First + 1);

                    for (int j = matchedIndex + 1; j < unpairedTags.Count; j++)
                    {
                        if (unpairedTags[j].Kind != TagKind.Closing && unpairedTags[j].Kind != TagKind.UserDefinedClosing)
                            children.Add(new Element(unpairedTags[j]));
                        else
                            throw new Exception();
                    }

                    int siblingIndex = parsedElements.FindIndex(x => currentScope.Covers(x));

                    if (siblingIndex != -1)
                    {
                        for (int k = siblingIndex; k < parsedElements.Count; k++)
                        {
                            children.Add(parsedElements[k]);
                        }
                        children.Sort();
                        parsedElements.RemoveRange(siblingIndex, parsedElements.Count - siblingIndex);
                    }


                    if (children.Count == 0)
                    {
                        higher = new Element(unpairedTags[matchedIndex], currentTag);
                    }
                    else
                    {
                        higher = new Element(unpairedTags[matchedIndex], currentTag, children);
                    }

                    parsedElements.Add(higher);

                    unpairedTags.RemoveRange(matchedIndex, unpairedTags.Count - matchedIndex);
                }
                else
                {
                    unpairedTags.Add(currentTag);
                    #region 
                    //if (unpairedTags.Count != 1)
                    //{
                    //    parsedElements.Add(new Element(opening, currentTag));
                    //    unpairedTags.RemoveAt(unpairedTags.Count - 1);
                    //}
                    //else
                    //{
                    //    Scope intersection = opening.IntersectionWith(currentTag);

                    //    if (intersection.Covers(parsedElements.Last()))
                    //    {
                    //        Element higher = new Element(opening, currentTag, parsedElements);
                    //        parsedElements = new List<Element>();
                    //        parsedElements.Add(higher);
                    //    }
                    //    else
                    //    {
                    //        parsedElements.Add(new Element(opening, currentTag));
                    //    }

                    //    unpairedTags.RemoveAt(0);
                    //}
                    #endregion
                }
            }

            //if (unpairedTags.Count == 0)
            //    return parsedElements;

            foreach (TagLabel tag in unpairedTags)
            {
                parsedElements.Add(new Element(tag));
            }
            return parsedElements;
        }

        public static string StringFromScopes(Char[] CharArray, List<Scope> scopes)
        {
            if (CharArray == null || scopes == null || scopes.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();

            foreach (Scope fragment in scopes)
            {
                string temp = new string(CharArray, fragment.First, fragment.Length).Trim(WhiteSpaces);
                int start = 0, end = -1;
                string key = null;

                if (temp.Contains('&') && temp.Contains(';'))
                {
                    start = temp.IndexOf('&', start);

                    do 
                    {
                        end = temp.IndexOf(';', start);

                        if (end == -1)
                            break;
                        
                        key = temp.Substring(start, end-start + 1);

                        if (ReservedHtmlEntities.ContainsKey(key))
                            temp = temp.Replace(key, ReservedHtmlEntities[key]);

                        start = temp.IndexOf('&', start);
                    } while (start != -1);
                }
                    
                //sb.Append(CharArray, fragment.First, fragment.Length);
                sb.Append(temp);
                if (!temp.EndsWith("\r\n"))
                    sb.AppendLine();
            }

            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        #endregion

        #region Property

        public ElementType Kind { get; private set; }

        public TagLabel Opening { get; private set; }

        public TagLabel Closing { get; private set; }

        public string Name { get; private set; }

        public Element Parent { get; private set; }

        public string Path
        {
            get
            {
                string ownPath = string.Format("{0}{1}{2}", TagLeading, Name, TagEnding);

                if (Parent == null)
                    return ownPath;
                else
                    return string.Format("{0}{1}", Parent.Path, ownPath);
            }
        }

        public Dictionary<string, string> Attributes
        {
            get { return Opening.Attributes; }
        }

        public List<Element> Children { get; private set; }

        public List<Scope> OwnTextScopes { get; set; }

        public List<Scope> AllTextScopes { get; set; }

        public string OwnText
        {
            get
            {
                return (OwnTextScopes == null) ? "" : StringFromScopes(Page, OwnTextScopes);
            }
        }

        public string AllText
        {
            get
            {
                return (AllTextScopes == null) ? "" : StringFromScopes(Page, AllTextScopes);
            }
        }
        #endregion

        #region Constructors
        public Element(TagLabel opening) : base(opening)
        {
            if (opening.Page == null)
                throw new Exception();

            Opening = opening;
            Closing = null;
            Parent = null;
            Name = opening.TagName;

            switch(opening.Kind)
            {
                case TagKind.Declaration:
                case TagKind.DocType:
                case TagKind.Remark:
                    Kind = ElementType.Declaration;
                    break;
                case TagKind.Sound:
                case TagKind.UserDefinedSound:
                default:
                    Kind = ElementType.Simple;
                    break;
                //case TagKind.Opening:
                //    if (!IsWithoutEnding(opening))
                //        throw new Exception();
                //    Kind = ElementType.Declaration;
                //    break;
                    //Kind = ElementType.UserDefined;
                    //break;
            }
        }

        public Element(TagLabel opening, TagLabel closing) : base(opening.Page, opening.First, closing.Last - opening.First + 1)
        {
            if (opening.Page != closing.Page || opening.TagName != closing.TagName)
                throw new Exception();
            
            Opening = opening;
            Name = Opening.TagName;
            Closing = closing;
            Parent = null;
            Kind = ElementType.Compound;

            getText();

            //switch (opening.Kind)
            //{
            //    case TagKind.Opening:
            //        if (closing.Kind != TagKind.Closing)
            //            throw new Exception();
            //        Kind = (closing.First - opening.Last == 1) ? ElementType.Empty : ElementType.Compound;
            //        break;
            //    case TagKind.UserDefinedOpening:
            //        if (closing.Kind != TagKind.UserDefinedClosing)
            //            throw new Exception();
            //        Kind = (closing.First - opening.Last == 1) ? ElementType.Empty : ElementType.Compound;
            //        break;
            //    default:
            //        Kind = ElementType.Invalid;
            //        break;
            //}

        }

        public Element(TagLabel opening, TagLabel closing, List<Element> children)
            : this(opening, closing)
        {
            Children = children;
            Name = Opening.TagName;
            Kind = ElementType.Compound | ElementType.WithChildren;

            foreach (Element child in children)
            {
                child.Parent = this;
            }

            getText();
        }

        public Element(TextScope whole, List<Element> children) : base(whole)
        {
            if (whole == null || children == null || children.Count == 0 || whole.Page != children[0].Page || First > children[0].First || Last < children.Last().Last)
                throw new Exception();

            Children = children;
            Name = "root";
            Kind = ElementType.Compound | ElementType.WithChildren;
            Opening = children[0].Opening;
            Closing = children.Last().Closing;
            if (Closing == null)
                Closing = children.Last().Opening;

            AllTextScopes = new List<Scope>();
            OwnTextScopes = new List<Scope>();

            if (First < Opening.First)
            {
                Scope newScope = new Scope(First, Opening.First - First);
                if (isText(newScope))
                    OwnTextScopes.Add(newScope);
            }

            if (Last > Closing.Last)
            {
                Scope newScope = new Scope(Closing.Last + 1, Last - Closing.Last - 1);
                if (isText(newScope))
                    OwnTextScopes.Add(newScope);
            }

            if (OwnTextScopes.Count == 0)
            {
                OwnTextScopes = null;
            }
            //else
            //    AllTextScopes.AddRange(OwnTextScopes);

            foreach (Element child in Children)
            {
                child.Parent = this;
                if (child.AllTextScopes != null)
                    AllTextScopes.AddRange(child.AllTextScopes);
            }

            if (AllTextScopes.Count != 0)
            {
                AllTextScopes.Sort();
                Kind |= ElementType.WithInnerText;
            }
        }

        #endregion

        #region Functions
        private bool isText(Scope fragment)
        {
            string text = TextOf(Page, fragment);

            return text.Trim() != "";
            //if (word == "")
            //    return false;
            //else if (NeglectWhiteSpaces && word == "\r\n")
            //    return false;

            //return true;
        }

        private void getText()
        {
            if (Kind == ElementType.Simple)
                return;

            OwnTextScopes = new List<Scope>();
            AllTextScopes = new List<Scope>();

            int start;
            if (Children == null)
            {
                if (!NeglectTextTags.Contains(Name) && Opening.Last < Closing.First - 1)
                    OwnTextScopes.Add(new Scope(Opening.Last + 1, Closing.First - Opening.Last - 1));
            }
            else
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    if (Children[i].AllTextScopes != null)
                        AllTextScopes.AddRange(Children[i].AllTextScopes);

                    if (!NeglectTextTags.Contains(Name))
                    {
                        start = (i == 0) ? Opening.Last + 1 : Children[i - 1].Last + 1;
                        if (start < Children[i].First)
                        {
                            Scope chip = new Scope(start, Children[i].First - start);

                            OwnTextScopes.Add(chip);
                        }
                    }

                }
                if (!NeglectTextTags.Contains(Name) && Children.Last().Last + 1 < Closing.First)
                {
                    Scope chip = new Scope(Children.Last().Last + 1, Closing.First - Children.Last().Last - 1);

                    OwnTextScopes.Add(chip);
                }
            }

            for (int j = OwnTextScopes.Count - 1; j >= 0; j--)
            {
                if (!isText(OwnTextScopes[j]))
                    OwnTextScopes.RemoveAt(j);
            }


            if (OwnTextScopes.Count == 0)
            {
                OwnTextScopes = null;
            }
            else
            {
                Kind |= ElementType.WithOwnText;
                AllTextScopes.AddRange(OwnTextScopes);
                AllTextScopes.Sort();
            }

            if (AllTextScopes.Count == 0)
                AllTextScopes = null;
            else
                Kind |= ElementType.WithInnerText;
        }

        public Element OwnerOfText(Scope scope)
        {
            Scope theTextScope = null;
            int index = -1;

            if (!AllTextScopes.Contains(scope))
            {
                index = AllTextScopes.FindIndex(x => x.Covers(scope));

                if (index == -1 || AllTextScopes[index].Last != scope.Last || AllTextScopes[index].First != scope.First)
                    return null;

                theTextScope = AllTextScopes[index];
            }
            else
                theTextScope = scope;

            if(OwnTextScopes != null && OwnTextScopes.Contains(theTextScope))
                return this;

            index = Children.FindIndex(x => x.Covers(theTextScope));

            if (index == -1)
                return null;
            else
                return Children[index].OwnerOfText(theTextScope);
        }

        public Element ContainerOf(String tagName)
        {
            string name = tagName.Trim(unexpectedTagChars);

            if (this.Name == name)
                return this;
            else if (this.Path.Contains("<" + name + ">"))
            {
                Element result = null;
                do 
                {
                    result = this.Parent;
                } while (result != null && result.Name != name);
                return result;
            }
            else
                return null;
        }

        public bool IsElementOf(string tagName)
        {
            string name = tagName.Trim(unexpectedTagChars);
            return Name == name || Path.Contains("<" + name + ">");
        }

        public override string ToString()
        {
            return string.Format("<{0}>: {1}", Name, OwnText);
        }

        #region IComparable<Element> 

        public int CompareTo(Element other)
        {
            return First.CompareTo(other.First);
        }

        #endregion
        #endregion

        #region Deleted codes
        /*/
        public void GetTextScope()
        {
            if (Kind == ElementType.Simple)
                return;

            OwnTextScopes = new List<Scope>();
            AllTextScopes = new List<Scope>();

            int start;
            if (Children == null)
            {
                if (!NeglectTextTags.Contains(Name) && Opening.Last < Closing.First - 1)
                    OwnTextScopes.Add(new Scope(Opening.Last + 1, Closing.First - Opening.Last - 1));
            }
            else
            {
                for(int i=0; i< Children.Count; i ++)
                {
                    Children[i].GetTextScope();

                    if (Children[i].AllTextScopes != null)
                        AllTextScopes.AddRange(Children[i].AllTextScopes);

                    if(!NeglectTextTags.Contains(Name))
                    {
                        start = (i == 0) ? Opening.Last + 1 : Children[i - 1].Last + 1;
                        if (start < Children[i].First)
                        {
                            Scope chip = new Scope(start, Children[i].First - start);

                            OwnTextScopes.Add(chip);
                        }
                    }
           
                }
                if (!NeglectTextTags.Contains(Name) && Children.Last().Last + 1 < Closing.First)
                {
                    Scope chip = new Scope(Children.Last().Last + 1, Closing.First - Children.Last().Last - 1);

                    OwnTextScopes.Add(chip);
                }
            }

            for (int j = OwnTextScopes.Count - 1; j >= 0; j -- )
            {
                if (!isText(OwnTextScopes[j]))
                    OwnTextScopes.RemoveAt(j);
            }


            if (OwnTextScopes.Count == 0)
            {
                OwnTextScopes = null;
            }
            else
            {
                Kind |= ElementType.WithOwnText;
                AllTextScopes.AddRange(OwnTextScopes);
                AllTextScopes.Sort();
            }

            if (AllTextScopes.Count == 0)
                AllTextScopes = null;
            else
                Kind |= ElementType.WithInnerText;
        }
        //*/
        #endregion
    }

}
