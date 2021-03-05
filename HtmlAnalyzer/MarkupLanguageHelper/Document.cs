using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HtmlAnalyzer.GenericPatternIndexer;

namespace HtmlAnalyzer.MarkupLanguageHelper
{

    public class Document
    {
        #region Static Functions
        private static List<string> meaningfulNames = new List<string>() { "doc", "search", "about" };

        public static bool IsMeaningfulFilename(string fileName)
        {
            return meaningfulNames.Contains(fileName.ToLower());
        }

        public static bool IsIndexByFilename(Document doc)
        {
            return doc.Address.LocalPath.ToLower().Contains("doc");
        }

        public static bool IsIndexBySiblings(Document doc)
        {
            return doc.SiblingUriDict != null && doc.SiblingUriDict.Count != 0 && doc.RootElement.AllTextScopes.Count < doc.SiblingUriDict.Count * 2;
        }

        public static bool IsChapterIndexByFilename(Uri uri)
        {
            return uri.LocalPath.ToLower().Contains("chapter");
        }

        public static bool IsChapterIndexByFrameName(Element element)
        {
            if (element.Name != "frame")
                return false;

            Dictionary<string, string> attributes = element.Attributes;

            if (!attributes.ContainsKey("name"))
                return false;

            return attributes["name"].ToLower().Contains("chapter");
        }

        private static char[] digitChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        public static int NumberFromUri(Uri uri)
        {
            string fileName = uri.LocalPath;

            int last = fileName.LastIndexOfAny(digitChars);

            if (last == -1)
                return -1;

            int first = fileName.IndexOfAny(digitChars);

            if (int.TryParse(fileName.Substring(first, last-first+1), out last))
                return last;

            return -1;
        }

        private static char[] unexpectedTagChars = new char[] { ' ', '<', '>' };

        public static List<Element> ElementsFrom(List<Element> collections, string tagName)
        {
            List<Element> result = new List<Element>();

            foreach (Element el in collections)
            {
                if (el.Name == tagName && !result.Contains(el))
                    result.Add(el);

                if (el.Children != null && el.Children.Count != 0)
                    result.AddRange(ElementsFrom(el.Children, tagName));
            }

            return result;
        }

        #endregion

        #region Property
        public Uri Address { get; private set; }

        public DocTypes DocumentType { get; private set; }

        public Encoding BaseEncoding { get; set; }

        public Element RootElement { get; private set; }

        public string Title { get; set; }

        private char[] CharArray { get; set; }

        public Dictionary<string, List<Element>> PathDict { get; set; }

        public int SiblingDepth { get; set; }

        public Dictionary<Element, Uri> SiblingUriDict { get; set; }
        
        public List<Element> this[string tagName]
        {
            get
            {
                List<Element> result = new List<Element>();

                if (RootElement.Name == tagName)
                    result.Add(RootElement);

                result.AddRange(ElementsFrom(RootElement.Children, tagName));

                return result;

                //string theTagName = tagName.Trim(unexpectedTagChars).ToLower();
                //string keyName = "<" + theTagName + ">";

                //foreach (string key in PathDict.Keys)
                //{
                //    if (key.Contains(keyName))
                //    {
                //        foreach(Element element in PathDict[key])
                //        {
                //            Element el = element;
                //            while (el.Name != theTagName)
                //            {
                //                el = el.Parent;
                //            }
                //            result.Add(el);
                //        }
                //    }
                //}

            }
        }

        public bool IsIndexDocument
        {
            get
            {
                if (DocumentType == DocTypes.XHtmlFrameset || DocumentType == DocTypes.HtmlFrameset)
                    return true;
                else
                    return IsIndexBySiblings(this);
            }
        }
        #endregion

        #region constructors
        public Document(Uri uri) : this(uri, 1, Encoding.UTF8){}

        public Document(Uri uri, int siblingDepth, Encoding encoding)
        {
            BaseEncoding = encoding;
            Address = uri;
            PathDict = new Dictionary<string, List<Element>>();
            SiblingUriDict = new Dictionary<Element, Uri>();
            SiblingDepth = siblingDepth;

            byte[] buffer = null;

            if (Address.IsFile && File.Exists(Address.LocalPath))
            {
                FileInfo fi = new FileInfo(Address.LocalPath);
                FileStream fs = new FileStream(Address.LocalPath, FileMode.Open, FileAccess.Read);

                int length = (int)(fi.Length);
                buffer = new byte[length];

                fs.Read(buffer, 0, length);
                fs.Close();
            }
            else
                throw new NotImplementedException();

            Encoding theEncoding = BaseEncoding;
            DocumentType = DocTypeDetector.DocTypesOf(buffer, ref theEncoding);

            CharArray = BaseEncoding.GetChars(buffer);
            BaseEncoding = theEncoding;

            //TagParser helper = DocTypeDetector.HelperOf(DocumentType);

            //if (DocTypeDetector.TryGetTagHelperOf(buffer, out helper, ref theEncoding))
            //{
            //    CharArray = BaseEncoding.GetChars(buffer);
            //    BaseEncoding = theEncoding;
            //}

            if (DocumentType == DocTypes.HtmlFrameset || DocumentType == DocTypes.XHtmlFrameset)
                parseFrame();
            else
                parseHtml();
        }

        #endregion

        public bool Contains(string tagName)
        {
            string keyName = tagName.Trim().ToLower();
            foreach(string key in PathDict.Keys)
            {
                if (key.Contains(keyName))
                    return true;
            }
            return false;
        }

        private int relativeDepthOf(Uri uri)
        {
            if (!Address.IsBaseOf(uri))
                return -1;

            return uri.Segments.Count() - Address.Segments.Count();
        }

        private void parseFrame()
        {
            TagParser helper= DocTypeDetector.HelperOf(DocumentType);

            List<Scope> splitRanges = Finder.ScopesOfPattern(CharArray, "<", ">");

            List<Element> elements = Element.HtmlElementsOf(CharArray, splitRanges, true);

            if (elements.Count == 1)
                RootElement = elements[0];
            else
                RootElement = elements.Find(x => x.Name == "frameset");

            List<Element> frames = ElementsFrom(elements, "frame");

            foreach (Element frame in frames)
            {
                string path = frame.Path;

                if (!PathDict.ContainsKey(path))
                    PathDict.Add(path, new List<Element>());
                PathDict[path].Add(frame);
            }

            foreach (Element frame in frames)
            {
                Dictionary<string, string> attributes = frame.Attributes;

                if (attributes.ContainsKey("src"))
                {
                    Uri frameUri = new Uri(Address, attributes["src"]);
                    SiblingUriDict.Add(frame, frameUri);
                }

                if (IsChapterIndexByFilename(Address) || IsChapterIndexByFrameName(frame))
                    Title = attributes["name"];
            }
        }

        private void parseHtml()
        {
            TagParser helper = DocTypeDetector.HelperOf(DocumentType);

            List<Scope> splitRanges = Finder.ScopesOfPattern(CharArray, "<", ">");

            List<Element> elements = Element.HtmlElementsOf(CharArray, splitRanges, true);

            RootElement = (elements.Count == 1) ? elements[0] : new Element(new TextScope(CharArray, 0, CharArray.Length), elements);

            foreach (Scope scope in RootElement.AllTextScopes)
            {
                Element owner = RootElement.OwnerOfText(scope);
                string path = owner.Path;
                if (!PathDict.ContainsKey(path))
                    PathDict.Add(path, new List<Element>());
                PathDict[path].Add(owner);
            }

            List<Element> aElements = this["a"];

            foreach (Element a in aElements)
            {
                Dictionary<string, string> attributes = a.Opening.Attributes;

                if (attributes.ContainsKey("href") && !attributes["href"].Contains('#'))
                {
                    Uri aUri = new Uri(Address, attributes["href"]);

                    int depth = relativeDepthOf(aUri);
                    if (depth >= Math.Max(0, SiblingDepth))
                        SiblingUriDict.Add(a, aUri);
                }
            }

            List<Element> titles = this["title"];

            if (titles.Count != 0)
            {
                Title = titles[0].OwnText;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} existingUri @ {2}", Title, SiblingUriDict.Count, Address);
        }
    }

}
