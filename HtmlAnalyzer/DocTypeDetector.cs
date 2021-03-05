using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAnalyzer.MarkupLanguageHelper;
using HtmlAnalyzer.GenericPatternIndexer;

namespace HtmlAnalyzer
{
    public enum DocTypes
    {
        Unknown,
        HtmlLoose,
        HtmlStrict,
        HtmlFrameset,
        XHtmlLoose,
        XHtmlStrict,
        XHtmlFrameset,
        XML,
        SGML,
        Plain
    }

    public static class DocTypeDetector
    {
        public const int MaxHeaderPosition = 2048;

        public const byte Space = 32;           //  ' '
        public const byte Exclamation = 33;     //  '!'
        public const byte DoubleQuote = 34;     //  '"'
        public const byte SingleQuote = 39;     //  '''
        public const byte ForwardSlash = 47;    //  '/'
        public const byte LessThan = 60;        //  '<'
        public const byte GreaterThan = 62;     //  '>'
        public const byte QuestionMark = 63;    //  '?'

        public static byte[] CharsetBytes = null;
        public static byte[] EncodingBytes = null;
        public static byte[] DocTypeBytes = null;
        public static byte[] XmlBytes = null;

        static DocTypeDetector()
        {
            CharsetBytes = Encoding.UTF8.GetBytes("charset=");
            DocTypeBytes = Encoding.UTF8.GetBytes("<!DOCTYPE");
            XmlBytes = Encoding.UTF8.GetBytes("<?xml version=\"");
            EncodingBytes = Encoding.UTF8.GetBytes("encoding=\"");
        }

        public static DocTypes DocTypesOf(byte[] content, ref Encoding encoding)
        {
            DocTypes result = DocTypes.Unknown;
            int max = Math.Min(content.Length, MaxHeaderPosition);

            string text = encoding.GetString(content, 0, max);

            List<string> matched = Finder.InsideOfPattern(text, "encoding=\"", "\"");

            if (matched.Count != 0)
            {
                if (matched.Count != 0 && encoding.HeaderName != matched[0])
                {
                    encoding = Encoding.GetEncoding(matched[0]);
                    text = encoding.GetString(content, 0, max);
                }

                if (text.IndexOf("<?xml version=\"") != -1)
                    return DocTypes.XML;
            }
            else
            {
                matched = Finder.InsideOfPattern(text, "charset=", "\"");

                if (matched.Count != 0 && encoding.HeaderName != matched[0])
                {
                    encoding = Encoding.GetEncoding(matched[0]);
                    text = encoding.GetString(content, 0, max);
                }

                matched = Finder.InsideOfPattern(text, "<!DOCTYPE", ">");

                if (matched.Count != 0)
                {
                    text = matched[0];
                    if (text.IndexOf("xhtml", StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        if (text.IndexOf("strict", 0, StringComparison.OrdinalIgnoreCase) != -1)
                            result = DocTypes.XHtmlStrict;
                        else if (text.IndexOf("frameset", StringComparison.OrdinalIgnoreCase) != -1)
                            result = DocTypes.XHtmlFrameset;
                        else
                            result = DocTypes.XHtmlLoose;

                    }
                    else if (matched[0].IndexOf("html", StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        if (text.IndexOf("strict", 0, StringComparison.OrdinalIgnoreCase) != -1)
                            result = DocTypes.HtmlStrict;
                        else if (text.IndexOf("frameset", StringComparison.OrdinalIgnoreCase) != -1)
                            result = DocTypes.HtmlFrameset;
                        else
                            result = DocTypes.HtmlLoose;
                    }
                }
                else if (text.Contains("frame"))
                {
                    result = DocTypes.HtmlFrameset;
                }
                else
                    result = DocTypes.HtmlLoose;
            }
            return result;

            #region Deleted codes
            //List<string> tags = Finder.StringsOfPettern(textOnly, "<", ">");

            //        if (word.IndexOf("strict", docStart, StringComparison.OrdinalIgnoreCase) != -1)
            //            result = DocTypes.XHtmlStrict;
            //        else if (word.IndexOf("frameset", docStart, StringComparison.OrdinalIgnoreCase) != -1)
            //            result = DocTypes.XHtmlFrameset;
            //        else
            //            result = DocTypes.XHtmlFrameset;
            
            //start = word.IndexOf("<?xml version=\"");
            //if (start != -1)
            //{
            //    result = DocTypes.XML;

            //    start = word.IndexOf("encoding=\"", start);

            //    if (start != -1)
            //    {
            //        start += "encoding=\"".Length;
            //        end = word.IndexOf('"', start);

            //        if (end != -1)
            //        {
            //            string encodingStr = word.Substring(start, end - start);
            //            encoding = Encoding.GetEncoding(encodingStr);
            //        }
            //    }

            //    return result;
            //}

            //docStart = word.IndexOf("<!DOCTYPE");
            //encoding = Encoding.Default;
            //if (docStart != -1)
            //{
            //    result = DocTypes.HtmlLoose;

            //    start = word.IndexOf("xhtml", docStart, StringComparison.OrdinalIgnoreCase);
            //    if (start != -1)
            //    {
            //        if (word.IndexOf("strict", docStart, StringComparison.OrdinalIgnoreCase) != -1)
            //            result = DocTypes.XHtmlStrict;
            //        else if (word.IndexOf("frameset", docStart, StringComparison.OrdinalIgnoreCase) != -1)
            //            result = DocTypes.XHtmlFrameset;
            //        else
            //            result = DocTypes.XHtmlFrameset;
            //    }
            //    else
            //    {
            //        if (word.IndexOf("strict", docStart, StringComparison.OrdinalIgnoreCase) != -1)
            //            result = DocTypes.HtmlStrict;
            //        else if (word.IndexOf("frameset", docStart, StringComparison.OrdinalIgnoreCase) != -1)
            //            result = DocTypes.HtmlFrameset;
            //    }

            //    start = word.IndexOf("charset=\"", start);

            //    if (start != -1)
            //    {
            //        start += "encoding=\"".Length;
            //        end = word.IndexOf('"', start);

            //        if (end != -1)
            //        {
            //            string encodingStr = word.Substring(start, end - start);
            //            encoding = Encoding.GetEncoding(encodingStr);
            //        }
            //    }
            //}
            //return result;

            //start = BoyerMooreBinary.IndexOf(XmlBytes, content, 256);

            //if (start != -1) //XML expected
            //{
            //    //start = BoyerMooreBinary.IndexOf(EncodingBytes, content, 256+start);
            //    result = DocTypes.XML;
            //}
            //else if((start = BoyerMooreBinary.IndexOf(DocTypeBytes, content, MaxHeaderPosition)) != -1)
            //{
            //    //TODO:
            //    result = DocTypes.HtmlLoose;
            //}

            //start = BoyerMooreBinary.IndexOf(result == DocTypes.XML ? EncodingBytes : CharsetBytes, content, MaxHeaderPosition);
            //end = Array.IndexOf(content, DoubleQuote, start);
            //if (start != -1 && end != -1)
            //{
            //    byte[] charset = new byte[end - start];
            //    Array.Copy(buffer, start, charset, 0, charset.Length);
            //    string charsetString = Encoding.Default.GetString(charset);
            //    encoding = Encoding.GetEncoding(charsetString);
            //}
            //else
            //    encoding = Encoding.Default;
            #endregion

            
        }

        public static TagParser HelperOf(DocTypes docType)
        {
            TagParser helper = null;
            switch (docType)
            {
                case DocTypes.HtmlStrict:
                    helper = TagParser.HtmlHelper;
                    break;
                case DocTypes.XHtmlStrict:
                    helper = TagParser.XhtmlHelper;
                    break;
                case DocTypes.HtmlLoose:
                    helper = TagParser.LooseHtmlHelper;
                    break;
                case DocTypes.XHtmlLoose:
                    helper = TagParser.LooseXhtmlHelper;
                    break;
                case DocTypes.HtmlFrameset:
                    helper = TagParser.HtmlFramesetHelper;
                    break;
                case DocTypes.XHtmlFrameset:
                    helper = TagParser.XhtmlFramesetHelper;
                    break;
                case DocTypes.XML:
                case DocTypes.SGML:
                case DocTypes.Plain:
                case DocTypes.Unknown:
                default:
                    {
                        helper = null;
                        break;
                    }
            }
            return helper;
        }

        public static bool TryGetTagHelperOf(byte[] content, out TagParser helper, ref Encoding encoding)
        {
            DocTypes docType = DocTypesOf(content, ref encoding);

            switch (docType)
            {
                case DocTypes.HtmlStrict:
                    helper = TagParser.HtmlHelper;
                    return true;
                case DocTypes.XHtmlStrict:
                    helper = TagParser.XhtmlHelper;
                    return true;
                case DocTypes.HtmlLoose:
                    helper = TagParser.LooseHtmlHelper;
                    return true;
                case DocTypes.XHtmlLoose:
                    helper = TagParser.LooseXhtmlHelper;
                    return true;
                case DocTypes.HtmlFrameset:
                    helper = TagParser.HtmlFramesetHelper;
                    return true;
                case DocTypes.XHtmlFrameset:
                    helper = TagParser.XhtmlFramesetHelper;
                    return true;
                case DocTypes.XML:
                case DocTypes.SGML:
                case DocTypes.Plain:
                case DocTypes.Unknown:
                default:
                    {
                        helper = null;
                        return false;
                    }
            }
        }
    }
}
