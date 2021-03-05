using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlAnalyzer.MarkupLanguageHelper
{
    public delegate bool IsValidTagDelegate(TagParser helper, string text, ref string tagName, ref TagKind Kind);

    public class TagParser
    {
        #region Strict Html Specific Definitions
        private static readonly List<string> StrictHtmlTags = new List<string>(){
            "<!-->",
            "<!DOCTYPE>",
            "<a>",
            "<abbr>",
            "<acronym>",
            "<address>",
            //"<applet>",
            "<area>",
            "<b>",
            "<base>",
            //"<basefont>",
            "<bdo>",
            "<big>",
            "<blockquote>",
            "<body>",
            "<br>",
            "<button>",
            "<caption>",
            //"<center>",
            "<cite>",
            "<code>",
            "<col>",
            "<colgroup>",
            "<dd>",
            "<del>",
            "<dfn>",
            //"<dir>",
            "<div>",
            "<dl>",
            "<dt>",
            "<em>",
            "<fieldset>",
            //"<font>",
            "<form>",
            //"<frame>",
            //"<frameset>",
            "<head>",
            "<h1>", "<h2>","<h3>", "<h4>","<h5>", "<h6>",
            "<hr>",
            "<html>",
            "<i>",
            "<iframe>",
            "<img>",
            "<input>",
            "<ins>",
            "<kbd>",
            "<scope>",
            "<legend>",
            "<li>",
            "<link>",
            "<map>",
            //"<menu>",
            "<meta>",
            "<noframes>",
            "<noscript>",
            "<object>",
            "<ol>",
            "<optgroup>",
            "<option>",
            "<p>",
            "<param>",
            "<pre>",
            "<q>",
            //"<s>",
            "<samp>",
            "<script>",
            "<select>",
            "<small>",
            "<span>",
            //"<strike>",
            "<strong>",
            "<style>",
            "<sub>",
            "<sup>",
            "<table>",
            "<tbody>",
            "<td>",
            "<textarea>",
            "<tfoot>",
            "<th>",
            "<thead>",
            "<title>",
            "<tr>",
            "<tt>",
            //"<u>",
            "<ul>",
            "<var>",
            "<wbr>"
        };
        #endregion

        #region Transitional Html Specific Definitions
        private static readonly List<string> TransitionalHtmlTags = new List<string>(){
            "<!-->",
            "<!DOCTYPE>",
            "<a>",
            "<abbr>",
            "<acronym>",
            "<address>",
            "<applet>",
            "<area>",
            "<b>",
            "<base>",
            "<basefont>",
            "<bdo>",
            "<big>",
            "<blockquote>",
            "<body>",
            "<br>",
            "<button>",
            "<caption>",
            "<center>",
            "<cite>",
            "<code>",
            "<col>",
            "<colgroup>",
            "<dd>",
            "<del>",
            "<dfn>",
            "<dir>",
            "<div>",
            "<dl>",
            "<dt>",
            "<em>",
            "<fieldset>",
            "<font>",
            "<form>",
            "<frame>",
            "<frameset>",
            "<head>",
            "<h1>", "<h2>","<h3>", "<h4>","<h5>", "<h6>",
            "<hr>",
            "<html>",
            "<i>",
            "<iframe>",
            "<img>",
            "<input>",
            "<ins>",
            "<kbd>",
            "<scope>",
            "<legend>",
            "<li>",
            "<link>",
            "<map>",
            "<menu>",
            "<meta>",
            "<noframes>",
            "<noscript>",
            "<object>",
            "<ol>",
            "<optgroup>",
            "<option>",
            "<p>",
            "<param>",
            "<pre>",
            "<q>",
            "<s>",
            "<samp>",
            "<script>",
            "<select>",
            "<small>",
            "<span>",
            "<strike>",
            "<strong>",
            "<style>",
            "<sub>",
            "<sup>",
            "<table>",
            "<tbody>",
            "<td>",
            "<textarea>",
            "<tfoot>",
            "<th>",
            "<thead>",
            "<title>",
            "<tr>",
            "<tt>",
            "<u>",
            "<ul>",
            "<var>",
            "<wbr>",
            "<xmp>"
        };
        #endregion

        #region Frameset Specific Definitions
        private static readonly List<string> FramesetTags = new List<string>(){
            "<!-->",
            "<!DOCTYPE>",
            "<a>",
            "<abbr>",
            "<acronym>",
            "<address>",
            "<applet>",
            "<area>",
            "<b>",
            "<base>",
            "<basefont>",
            "<bdo>",
            "<big>",
            "<blockquote>",
            "<frameset>",
            "<br>",
            "<button>",
            "<caption>",
            "<center>",
            "<cite>",
            "<code>",
            "<col>",
            "<colgroup>",
            "<dd>",
            "<del>",
            "<dfn>",
            "<dir>",
            "<div>",
            "<dl>",
            "<dt>",
            "<em>",
            "<fieldset>",
            "<font>",
            "<form>",
            "<frame>",
            "<frameset>",
            "<head>",
            "<h1>", "<h2>","<h3>", "<h4>","<h5>", "<h6>",
            "<hr>",
            "<html>",
            "<i>",
            "<iframe>",
            "<img>",
            "<input>",
            "<ins>",
            "<kbd>",
            "<scope>",
            "<legend>",
            "<li>",
            "<link>",
            "<map>",
            "<menu>",
            "<meta>",
            "<noframes>",
            "<noscript>",
            "<object>",
            "<ol>",
            "<optgroup>",
            "<option>",
            "<p>",
            "<param>",
            "<pre>",
            "<q>",
            "<s>",
            "<samp>",
            "<script>",
            "<select>",
            "<small>",
            "<span>",
            "<strike>",
            "<strong>",
            "<style>",
            "<sub>",
            "<sup>",
            "<table>",
            "<tbody>",
            "<td>",
            "<textarea>",
            "<tfoot>",
            "<th>",
            "<thead>",
            "<title>",
            "<tr>",
            "<tt>",
            "<u>",
            "<ul>",
            "<wbr>",
            "<var>"
        };
        #endregion

        #region Transitional Defintions
        private static readonly List<string> TransitionalTags = new List<string>(){
            "<applet>",
            "<basefont>",
            "<center>",
            "<dir>",
            "<font>",
            "<isindex>",
            "<menu>",
            "<s>",
            "<strike>",
            "<u>",
            "<xmp>"
        };
        #endregion

        public static TagParser HtmlHelper = new TagParser(StrictIsValidTag, StrictHtmlTags, false);
        public static TagParser XhtmlHelper = new TagParser(StrictIsValidTag, StrictHtmlTags, true, false);
        public static TagParser LooseHtmlHelper = new TagParser(HtmlIsValidTag, TransitionalHtmlTags, false);
        public static TagParser LooseXhtmlHelper = new TagParser(HtmlIsValidTag, TransitionalHtmlTags, true, false);
        public static TagParser HtmlFramesetHelper = new TagParser(HtmlIsValidTag, FramesetTags, false);
        public static TagParser XhtmlFramesetHelper = new TagParser(HtmlIsValidTag, FramesetTags, true, false);
        public static TagParser XMLHelper = new TagParser(GenericIsValidTag, StrictHtmlTags, true, true);

        #region Definitions

        public static char SPACE = ' ';
        public const char LessThan = '<';
        public const char GreaterThan = '>';
        public const char Slash = '/';
        public const char Exclamation = '!';
        public const char QuestionMark = '?';

        public static char TagFirstChar = LessThan;
        public static char TagLastChar = GreaterThan;

        public static string RemarkTagIdentity = "<!-->";
        public static string DocTypeTagIdentity = "<!DOCTYPE>";

        public static char[] TagNameEndings = new char[] { ' ', '>' };
        public static char[] IllegalCharsWithinTag = new char[] { '<', '>' };

        //public static string SplitClosingIndicator = "</";
        //public static string SoundClosingIndicator = "/>";

        #endregion

        #region Static functions

        public static bool GenericIsValidTag(TagParser helper, string tagString, ref string tagName, ref TagKind kind)
        {
            kind = TagKind.Invalid;
            if (tagString[0] != TagFirstChar || tagString[tagString.Length - 1] != TagLastChar)
            {
                return false;
            }
            else if (tagString.IndexOfAny(IllegalCharsWithinTag, 1, tagString.Length-2) != -1)
            {
                return false;
            }

            int last = 0;

            switch(tagString[1])
            {
                case QuestionMark:      //Case when the Tag starts with "<?"
                    last = tagString.IndexOfAny(TagNameEndings);
                    tagName = tagString.Substring(2, last - 2);

                    if (tagName == "xml")
                        kind = TagKind.Declaration;
                    else
                        kind = TagKind.UserDefinedSound;

                    return true;

                case Slash:             //Case when the Tag starts with "</"
                    last = tagString.IndexOfAny(TagNameEndings);
                    tagName = tagString.Substring(2, last - 2);
                    kind = (TransitionalHtmlTags.Contains(tagName)) ? TagKind.Closing : TagKind.UserDefinedClosing;
                    return true;

                case Exclamation:       //Case when the Tag starts with "<!"
                    if (tagString.StartsWith("<!--"))   //Case when the Tag starts with "<!--"
                    {
                        tagName = "!--";
                        if (tagString.EndsWith("-->"))
                        {
                            kind = TagKind.Remark;
                            return true;
                        }
                        else
                        {
                            kind = TagKind.Unknown;
                            return false;
                        }
                    }
                    else if (tagString.StartsWith("<!DOCTYPE"))     //Case when the Tag starts with "<!DOCTYPE"
                    {
                        tagName = "!DOCTYPE";
                        kind = TagKind.DocType;
                        return true;
                    }
                    return false;
                default:
                    last = tagString.IndexOfAny(TagNameEndings);

                    if (last < 1)
                    {
                        kind = TagKind.Invalid;
                        return false;
                    }

                    tagName = tagString.Substring(1, last - 1).ToLower();

                    if (tagString[tagString.Length - 2] == Slash) //Case when the Tag ends with "/>"
                        kind = TagKind.Sound;
                    else
                        kind = TagKind.Opening;
                    return true;

                    //if (tagString[tagString.Length - 2] == Slash) //Case when the Tag ends with "/>"
                    //{
                    //    int previous = tagString.LastIndexOfAny(TagNameEndings);
                    //    tagName = tagString.Substring(previous + 1, tagString.Length - previous - 3).ToLower();

                    //    kind = TagKind.Sound;
                    //    return (helper.TagNames.Covers(tagName));
                    //}

                    //kind = TagKind.Invalid;

                    //return true;
            }
        }

        public static bool HtmlIsValidTag(TagParser helper, string tagString, ref string tagName, ref TagKind kind)
        {
            kind = TagKind.Invalid;
            if (tagString[0] != TagFirstChar || tagString[tagString.Length - 1] != TagLastChar)
            {
                return false;
            }
            else if (tagString.IndexOfAny(IllegalCharsWithinTag, 1, tagString.Length - 2) != -1)
            {
                return false;
            }

            int last;

            switch (tagString[1])
            {
                case Slash:             //Case when the Tag starts with "</"
                    last = tagString.IndexOfAny(TagNameEndings);

                    if (last < 2)
                    {
                        kind = TagKind.Invalid;
                        return false;
                    }
                    tagName = tagString.Substring(2, last - 2).ToLower();
                    if (helper.TagNames.Contains(tagName))
                    {
                        kind = TagKind.Closing;
                        return true;
                    }
                    return false;
                case Exclamation:       //Case when the Tag starts with "<!"
                    if (tagString.StartsWith("<!--"))   //Case when the Tag starts with "<!--"
                    {
                        tagName = "!--";
                        kind = TagKind.Remark;
                        return true;
                    }
                    else if (tagString.StartsWith("<!DOCTYPE"))     //Case when the Tag starts with "<!DOCTYPE"
                    {
                        tagName = "!DOCTYPE";
                        kind = TagKind.DocType;
                        return true;
                    }
                    return false;
                default:
                    last = tagString.IndexOfAny(TagNameEndings);

                    if (last < 1)
                    {
                        kind = TagKind.Invalid;
                        return false;
                    }
                    tagName = tagString.Substring(1, last - 1).ToLower();

                    if (helper.TagNames.Contains(tagName))
                    {
                        if (tagString[tagString.Length - 2] == Slash) //Case when the Tag ends with "/>"
                            kind = TagKind.Sound;
                        else
                            kind = TagKind.Opening;
                        return true;
                    }
                    else                        
                    {
                        if (tagString[tagString.Length - 2] == Slash) //Case when the Tag ends with "/>"
                            kind = TagKind.UserDefinedSound;
                        else
                            kind = TagKind.UserDefinedClosing;

                        return true;
                    }
            }
        }

        public static bool StrictIsValidTag(TagParser helper, string tagString, ref string tagName, ref TagKind kind)
        {
            kind = TagKind.Invalid;
            if (tagString[0] != TagFirstChar || tagString[tagString.Length - 1] != TagLastChar)
            {
                return false;
            }
            else if (tagString.IndexOfAny(IllegalCharsWithinTag, 1, tagString.Length - 2) != -1)
            {
                return false;
            }

            int last;

            switch (tagString[1])
            {
                case QuestionMark:      //Case when the Tag starts with "<?"
                    last = tagString.IndexOfAny(TagNameEndings);
                    tagName = tagString.Substring(2, last - 2);

                    if (tagName == "xml")
                    {
                        kind = TagKind.Declaration;
                        return true;
                    }
                    return false;
                case Slash:             //Case when the Tag starts with "</"
                    last = tagString.IndexOfAny(TagNameEndings);
                    tagName = tagString.Substring(2, last - 2);
                    if (helper.TagNames.Contains(tagName))
                    {
                        kind = TagKind.Closing;
                        return true;
                    }
                    return false;
                case Exclamation:       //Case when the Tag starts with "<!"
                    if (tagString.StartsWith("<!--"))   //Case when the Tag starts with "<!--"
                    {
                        tagName = "!--";
                        if (tagString.EndsWith("-->"))
                        {
                            kind = TagKind.Remark;
                            return true;
                        }
                        else
                        {
                            kind = TagKind.Invalid;
                            return false;
                        }
                    }
                    else if (tagString.StartsWith("<!DOCTYPE"))     //Case when the Tag starts with "<!DOCTYPE"
                    {
                        tagName = "!DOCTYPE";
                        kind = TagKind.DocType;
                        return true;
                    }
                    return false;
                default:
                    last = tagString.IndexOfAny(TagNameEndings);

                    if (last < 1)
                    {
                        kind = TagKind.Invalid;
                        return false;
                    }
                    tagName = tagString.Substring(1, last - 1);

                    if (helper.TagNames.Contains(tagName))
                    {
                        if (tagString[tagString.Length - 2] == Slash) //Case when the Tag ends with "/>"
                            kind = TagKind.Sound;
                        else
                            kind = TagKind.Opening;
                        return true;
                    }

                    kind = TagKind.Invalid;
                    return false;
            }
        }

        #endregion

        private IsValidTagDelegate isValidFunc = null;

        public bool IsCaseSensitive { get; private set; }

        public bool IsExpandable { get; private set; }

        public List<string> TagNames = new List<string>();

        private TagParser()
        {
            TagNames.Add("!--");
            TagNames.Add("!DOCTYPE");
        }

        private TagParser(IsValidTagDelegate isValid, List<string> tags, bool isCaseSensitive)
            : this(isValid, tags, isCaseSensitive, false)
        { }

        private TagParser(IsValidTagDelegate isValid, List<string> tags, bool isCaseSensitive, bool isExpandable)
            : this()
        {
            IsCaseSensitive = isCaseSensitive;
            IsExpandable = isExpandable;
            isValidFunc = isValid;

            string tagName = null;
            foreach (string tag in tags)
            {
                tagName = tag.Substring(1, tag.Length - 2).ToLower();

                if (tagName[0] != '!' && !TagNames.Contains(tagName))
                    TagNames.Add(tagName);
            }

        }

        public bool IsValidTag(string tagString, ref string tagName, ref TagKind Kind)
        {
            if (isValidFunc == null)
                return false;

            return isValidFunc(this, tagString, ref tagName, ref Kind);
        }

        public bool IsValidTag(char[] context, IScope range, ref TagLabel label)
        {
            string text = TextScope.TextOf(context, range);
            string tagName = null;
            TagKind kind = TagKind.Unknown;

            if (IsValidTag(text, ref tagName, ref kind))
            {
                label = new TagLabel(context, range, tagName, kind);
                return true;
            }
            else
                return false;
        }
    }


}
