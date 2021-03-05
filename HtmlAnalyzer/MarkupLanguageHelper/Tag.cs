using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAnalyzer.GenericPatternIndexer;

namespace HtmlAnalyzer.MarkupLanguageHelper
{
    public enum TagKind
    {
        Unknown,
        Opening,
        Closing,
        UserDefinedOpening,
        UserDefinedClosing,
        Invalid,
        Sound,
        UserDefinedSound,
        Remark,
        DocType,
        Declaration
    }

    public class TagLabel : TextScope
    {
        public static char[] AttributeValueLeading = new char[] { '=', '"' };
        public static char[] AttributeValueEnding = new char[] { '"' };
        public static char[] AttributeNameLeading = new char[] { ' ' };

        public string TagName { get; set; }

        public TagKind Kind { get; set; }

        public Dictionary<string, string> Attributes
        {
            get 
            {
                Dictionary<string, string> attributes = new Dictionary<string, string>();

                List<Scope> valueScopes = Finder.ScopesOfPattern(Page, AttributeValueLeading, AttributeValueEnding, this);

                int attrNameIndex = -1;
                if (valueScopes.Count != 0)
                {
                    foreach(Scope scope in valueScopes)
                    {
                        attrNameIndex = Finder.LastIndexOf(Page, AttributeNameLeading, scope.First);
                        if (attrNameIndex == -1)
                            throw new Exception();

                        string name = new string(Page, attrNameIndex, scope.First - attrNameIndex).Trim().ToLower();
                        string attriValue = new string(Page, scope.First + 2, scope.Length - 3);
                        attributes.Add(name, attriValue);
                    }
                }

                return attributes; 
            }
        }

        public TagLabel(TextScope scope, string name, TagKind kind)
            :base(scope)
        {
            TagName = name;
            Kind = kind;
        }

        public TagLabel(char[] context, IScope range, string name, TagKind kind)
            : base (context, range)
        {
            TagName = name;
            Kind = kind;
        }

        public override string ToString()
        {
            return String.Format("{0} of {1}: {2}", Kind, TagName, RawText);
        }
    }

    //public class Tag
    //{
    //    #region Definitions
    //    private static List<string> HtmlTagNames = new List<string>(){
    //        "<!-->",
    //        "<!DOCTYPE>",
    //        "<a>",
    //        "<abbr>",
    //        "<acronym>",
    //        "<address>",
    //        "<applet>",
    //        "<area>",
    //        "<b>",
    //        "<base>",
    //        "<basefont>",
    //        "<bdo>",
    //        "<big>",
    //        "<blockquote>",
    //        "<body>",
    //        "<br>",
    //        "<button>",
    //        "<caption>",
    //        "<center>",
    //        "<cite>",
    //        "<code>",
    //        "<col>",
    //        "<colgroup>",
    //        "<dd>",
    //        "<del>",
    //        "<dfn>",
    //        "<dir>",
    //        "<div>",
    //        "<dl>",
    //        "<dt>",
    //        "<em>",
    //        "<fieldset>",
    //        "<font>",
    //        "<form>",
    //        "<frame>",
    //        "<frameset>",
    //        "<head>",
    //        "<h1>", "<h2>","<h3>", "<h4>","<h5>", "<h6>",
    //        "<hr>",
    //        "<html>",
    //        "<i>",
    //        "<iframe>",
    //        "<img>",
    //        "<input>",
    //        "<ins>",
    //        "<kbd>",
    //        "<scope>",
    //        "<legend>",
    //        "<li>",
    //        "<link>",
    //        "<map>",
    //        "<menu>",
    //        "<meta>",
    //        "<noframes>",
    //        "<noscript>",
    //        "<object>",
    //        "<ol>",
    //        "<optgroup>",
    //        "<option>",
    //        "<p>",
    //        "<param>",
    //        "<pre>",
    //        "<q>",
    //        "<s>",
    //        "<samp>",
    //        "<script>",
    //        "<select>",
    //        "<small>",
    //        "<span>",
    //        "<strike>",
    //        "<strong>",
    //        "<style>",
    //        "<sub>",
    //        "<sup>",
    //        "<table>",
    //        "<tbody>",
    //        "<td>",
    //        "<textarea>",
    //        "<tfoot>",
    //        "<th>",
    //        "<thead>",
    //        "<title>",
    //        "<tr>",
    //        "<tt>",
    //        "<u>",
    //        "<ul>",
    //        "<var>"
    //    };

    //    public static List<string> LeadingIndicators = new List<string>();
    //    public static List<string> TagNames = new List<string>();

    //    public static string SplitClosingIndicator = "</";
    //    public static string SoundClosingIndicator = "/>";
    //    public static char SPACE = ' ';
    //    public static char[] Endings = new char[] { ' ', '>' };

    //    static Tag()
    //    {
    //        TagNames.Add("!--");
    //        TagNames.Add("!DOCTYPE");
    //        foreach(string tag in HtmlTagNames)
    //        {
    //            LeadingIndicators.Add(tag.Substring(0, tag.Length - 1));

    //            if (tag[1] == '!')
    //                continue;

    //            TagNames.Add(tag.Substring(1, tag.Length - 2));
    //        }
    //    }

    //    #endregion

    //    #region Static functions

    //    public static bool IsValidTag(string tagString, out string tagName)
    //    {
    //        if (tagString.StartsWith("</"))
    //        {
    //            int opening = tagString.IndexOfAny(Endings);
    //            tagName = tagString.Substring(2, opening - 2).ToLower();
    //            return (TagNames.Covers(tagName));
    //        }
    //        else if (tagString.EndsWith("/>"))
    //        {
    //            int previous = tagString.LastIndexOfAny(Endings);
    //            tagName = tagString.Substring(previous + 1, tagString.Length - previous -3).ToLower();
    //            //tagName = BoyerMooreString.FirstTextBetween(tagString, '<', SPACE);
    //            return (TagNames.Covers(tagName));
    //        }
    //        else if (tagString[1] == '!')
    //        {
    //            if (tagString.StartsWith("<!--"))
    //            {
    //                tagName = "!--";
    //                return tagString.EndsWith("-->");
    //            }
    //            else if (tagString.StartsWith("<!DOCTYPE"))
    //            {
    //                tagName = "!DOCTYPE";
    //                return tagString.EndsWith(">");
    //            }
    //        }

    //        tagName = "";
    //        return false;
    //    }

    //    public static Tag TagOf(string word)
    //    {
    //        return new Tag(word);
    //    }

    //    public static Tag TagOf(string word, Func<string, Tag> func)
    //    {
    //        return func == null ? TagOf(word) : func(word); 
    //    }
    //    #endregion

    //    #region Property

    //    public Tag Parent { get; set; }

    //    public string Children { get; set; }

    //    public List<Tag> Siblings { get; private set; }

    //    #endregion

    //    #region Constructors

    //    protected Tag(string content)
    //    {
    //        Children = content;
    //        Siblings = new List<Tag>();
    //    }
    //    #endregion

    //}

}
