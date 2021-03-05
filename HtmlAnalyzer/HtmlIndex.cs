using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Winista.Text.HtmlParser.Nodes;
//using Winista.Text.HtmlParser;
//using Winista.Text.HtmlParser.Lex;
//using Winista.Text.HtmlParser.Util;
//using Winista.Text.HtmlParser.Tags;
//using Winista.Text.HtmlParser.Filters;
//using Winista.Text.HtmlParser.Visitors;

namespace HtmlAnalyzer
{
    //public class HtmlIndex
    //{
    //    #region Static definitions
    //    public static NodeClassFilter TitleTagFilter = new NodeClassFilter(typeof(TitleTag));
    //    public static NodeClassFilter FrameSetTagFilter = new NodeClassFilter(typeof(FrameSetTag));
    //    public static NodeClassFilter FrameTagFilter = new NodeClassFilter(typeof(FrameTag));
    //    public static NodeClassFilter ATagFilter = new NodeClassFilter(typeof(ATag));
    //    public static NodeClassFilter BodyTagFilter = new NodeClassFilter(typeof(BodyTag));

    //    public static string ContentIndicator = "content";

    //    public static Uri Address = null;

    //    public static Dictionary<int, HtmlConfig> LayeredConfig = new Dictionary<int, HtmlConfig>();

    //    public static void Reset()
    //    {
    //        Address = null;
    //        LayeredConfig.Clear();
    //    }

    //    public static int RelativeDepthOf(Uri uri)
    //    {
    //        if (!Address.IsBaseOf(uri))
    //            return -1;

    //        return uri.Segments.Count() - Address.Segments.Count();
    //    }
    //    #endregion

    //    public Uri Address { get; set; }

    //    public string Context { get; set; }

    //    public Encoding TheEncoding { get; set; }

    //    public string Title { get; set; }

    //    public Dictionary<Uri, HtmlIndex> Children { get; set; }

    //    public List<TextNode> Texts { get; set; }

    //    public HtmlIndex(Uri address) : this(address, null){}

    //    public HtmlIndex(Uri address, Encoding encoding)
    //    {
    //        if (Address == null)
    //            Address = address;

    //        Children = new Dictionary<Uri, HtmlIndex>();
    //        Texts = new List<TextNode>();

    //        load(address, encoding);
    //    }

    //    private void load(Uri address, Encoding encoding)
    //    {
    //        Address = address;
    //        if (address.IsFile && File.Exists(address.LocalPath))
    //        {
    //            FileInfo fi = new FileInfo(address.LocalPath);
    //            FileStream fs = new FileStream(address.LocalPath, FileMode.Open, FileAccess.Read);

    //            int length = (int)(fi.Length);
    //            byte[] buffer = new byte[length];

    //            fs.Read(buffer, 0, length);
    //            fs.Close();

    //            Encoding theEncoding = encoding;
    //            string content = null;

    //            if (Utility.EncodingAdapter.TryParse(buffer, ref theEncoding, ref content))
    //            {
    //                TheEncoding = theEncoding;
    //                Context = content == null ? TheEncoding.GetString(buffer) : content;
    //                Parse();
    //            }
    //        }
    //        else
    //            throw new NotImplementedException();
    //    }

    //    public void Parse()
    //    {
    //        Lexer lexer = new Lexer(Context);
    //        Parser parser = new Parser(lexer);
    //        parser.Encoding = TheEncoding.HeaderName;

    //        NodeList allNodes = parser.Parse(null);

    //        NodeList nodes = allNodes.ExtractAllNodesThatMatch(FrameSetTagFilter, true);

    //        if (nodes.Count != 0)
    //        {
    //            ParseFrameSet(nodes, this);
    //        }
    //        else
    //        {
    //            parseHtmlBodyNodes(allNodes, this);
    //        }

    //        int depth = RelativeDepthOf(Address);
    //        if (depth >= 0 && !LayeredConfig.ContainsKey(depth))
    //            LayeredConfig.Add(depth, new HtmlConfig(TheEncoding.HeaderName, nodes.Count!= 0, Children.Count != 0));

    //    }

    //    private void parseHtmlBodyNodes(NodeList allNodes, HtmlIndex doc)
    //    {
    //        NodeList nodes = allNodes.ExtractAllNodesThatMatch(TitleTagFilter, true);

    //        if (nodes.Count != 0)
    //        {
    //            TitleTag title = nodes[0] as TitleTag;
    //            doc.Title = title.Title;
    //        }
    //        else
    //            doc.Title = Path.GetFileName(Address.LocalPath);

    //        //nodes = allNodes.ExtractAllNodesThatMatch(BodyTagFilter, true);
    //        //foreach (INode nd in nodes[0].Children)
    //        //{
    //        //    if (nd is TextNode)
    //        //        doc.Texts.Add(nd as TextNode);
    //        //}

    //        NodeList aTags = allNodes.ExtractAllNodesThatMatch(ATagFilter, true);

    //        for (int i = 0; i < aTags.Count; i ++ )
    //        {
    //            if (i > 3)
    //                break;

    //            ATag a = aTags[i] as ATag;
    //            if (a == null)
    //                continue;

    //            Uri tagUri = new Uri(Address, a.Link);

    //            if (Address.IsBaseOf(tagUri) && !Children.ContainsKey(tagUri))
    //            {
    //                doc.Children.Add(tagUri, new HtmlIndex(tagUri, TheEncoding));
    //            }
    //        }

    //    }

    //    private void ParseFrameSet(NodeList nodes, HtmlIndex doc)
    //    {
    //        List<FrameTag> frames = new List<FrameTag>();

    //        NodeList frameSets = nodes.ExtractAllNodesThatMatch(FrameSetTagFilter);

    //        foreach(INode node in frameSets)
    //        {
    //            FrameSetTag frameSet = node as FrameSetTag;
    //            if (frameSet == null)
    //                continue;

    //            foreach (INode child in frameSet.Children)
    //            {
    //                if (child != null && child is FrameTag && !frames.Contains(child as FrameTag))
    //                {
    //                    frames.Add(child as FrameTag);
    //                }
    //            }
    //        }

    //        //Try to filter out the content frame by guessing through frame name or src filename
    //        FrameTag contentFrame = null;

    //        foreach(FrameTag frame in frames)
    //        {
    //            if (frame.FrameName.Equals(ContentIndicator, StringComparison.CurrentCultureIgnoreCase) || frame.FrameLocation.Contains(ContentIndicator))
    //            {
    //                contentFrame = frame;
    //                break;
    //            }
    //        }

    //        if (contentFrame != null)
    //            frames.Remove(contentFrame);

    //        if (frames.Count == 1)
    //        {
    //            Uri redirectUri = new   Uri(doc.Address, frames[0].FrameLocation);
    //            doc.Redirect(redirectUri, doc.TheEncoding);
    //        }
    //    }

    //    private void Redirect(Uri redirectUri, Encoding encoding)
    //    {
    //        load(redirectUri, encoding);
    //    }
    //}
}
