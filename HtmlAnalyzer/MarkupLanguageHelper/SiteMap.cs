using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace HtmlAnalyzer.MarkupLanguageHelper
{
    public class SiteMap
    {        
        #region Static defintions

        private static int relativeDepthOf(Uri baseAddress, Uri uri)
        {
            if (!baseAddress.IsBaseOf(uri))
                return -1;

            return uri.Segments.Count() - baseAddress.Segments.Count();
        }
        #endregion

        public Encoding BaseEncoding { get; private set; }

        public Uri BaseAddress { get; private set; }

        public XElement Indexes { get; private set; }

        public int BottomLevel { get; set; }
        
        public SiteMap(Document doc)
        {
            if (doc.IsIndexDocument)
            {
                BaseEncoding = doc.BaseEncoding;
                BaseAddress = doc.Address;

                Dictionary<Uri, List<string>> tempDict = new Dictionary<Uri, List<string>>();

                BottomLevel = 10;

                Indexes = mapOf(doc, tempDict);

                System.Diagnostics.Debug.WriteLine(Indexes.ToString());
            }
        }

        //private XElement SortOf(XElement original)
        //{
        //    foreach (XElement child in original.)
        //    {
        //    }
        //}

        private XElement mapOf(Document doc, Dictionary<Uri, List<string>> tempDict)
        {
            List<XElement> children = new List<XElement>();
            string title;

            if (doc.DocumentType == DocTypes.HtmlFrameset || doc.DocumentType == DocTypes.XHtmlFrameset)
            {
                //Parse the Frameset & Frame

                foreach (KeyValuePair<Element, Uri> kvp in doc.SiblingUriDict)
                {
                    //Concern only the frame of index by checking filename or framename
                    if (Document.IsChapterIndexByFilename(kvp.Value) || Document.IsChapterIndexByFrameName(kvp.Key))
                    {
                        Document chapter = new Document(kvp.Value, 0, BaseEncoding);
                        return mapOf(chapter, tempDict);
                    }
                }
                throw new Exception();
            }
            else //Parse normal Html file
            {
                // Be sure to add current Uri
                if (!tempDict.ContainsKey(doc.Address))
                {
                    tempDict.Add(doc.Address, new List<string> { doc.Title });
                }

                Dictionary<Element, List<Element>> layout = new Dictionary<Element, List<Element>>();
                Element textOnly = null, current = null, root = doc.RootElement;

                if (doc.Address == BaseAddress)
                {
                    foreach (Scope scope in root.AllTextScopes)
                    {
                        current = root.OwnerOfText(scope);

                        if (!current.IsElementOf("body"))
                            continue;

                        if (!current.IsElementOf("a"))
                        {
                            textOnly = current;
                        }
                        else
                        {
                            current = current.ContainerOf("a");
                            if (textOnly == null)
                                layout.Add(current, new List<Element>());
                            else if (doc.SiblingUriDict.ContainsKey(current))
                            {
                                if (!layout.ContainsKey(textOnly))
                                    layout.Add(textOnly, new List<Element>());

                                layout[textOnly].Add(current);
                            }
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<Element, Uri> kvp in doc.SiblingUriDict)
                    {
                        layout.Add(kvp.Key, new List<Element>());
                    }
                }

                foreach (KeyValuePair<Element, List<Element>> kvp in layout)
                {
                    if (kvp.Key.IsElementOf("a"))
                    {
                        Element href = kvp.Key;

                        if (!doc.SiblingUriDict.ContainsKey(href))
                            throw new Exception();

                        Uri address = doc.SiblingUriDict[href];

                        if (href.AllText != "")
                        {
                            if (!tempDict.ContainsKey(address))
                            {
                                tempDict.Add(address, new List<string> { href.AllText });
                                children.Add(
                                    new XElement(levelOf(address),
                                        new XAttribute("title", href.AllText),
                                        new XAttribute("uri", relativeAddressOf(address))
                                        )
                                    );
                            }
                            else
                            {
                                tempDict[address].Add(href.AllText);
                            }
                        }
                    }
                    else if (kvp.Value.Count != 0)
                    {
                        List<XElement> grandChildren = new List<XElement>();

                        foreach (Element sibling in kvp.Value)
                        {
                            if (!doc.SiblingUriDict.ContainsKey(sibling))
                                throw new Exception();

                            Uri address = doc.SiblingUriDict[sibling];

                            if (!tempDict.ContainsKey(address) && sibling.AllText != "")
                            {
                                tempDict.Add(address, new List<string> { sibling.AllText });
                                grandChildren.Add(
                                    new XElement(levelOf(address),
                                        new XAttribute("title", sibling.AllText),
                                        new XAttribute("uri", relativeAddressOf(address))
                                        )
                                    );
                            }
                            else
                            {
                                tempDict[address].Add(sibling.AllText);
                            }
                        }

                        if (grandChildren.Count != 0)
                        {
                            children.Add(
                                new XElement(
                                    "folder",
                                    new XAttribute("title", kvp.Key.AllText),
                                    grandChildren
                                    )
                                );
                        }
                    }
                }

                //foreach (KeyValuePair<Element, Uri> kvp in doc.SiblingUriDict)
                //{
                //    if (!tempDict.ContainsKey(kvp.Value))
                //    {
                //        tempDict.Add(kvp.Value, new List<string> { kvp.Key.AllText });
                //        children.Add(
                //            new XElement(levelOf(kvp.Value),
                //                new XAttribute("title", kvp.Key.AllText),
                //                new XAttribute("uri", kvp.Value)
                //                )
                //            );
                //    }
                //    else
                //    {
                //        tempDict[kvp.Value].Add(kvp.Key.AllText);
                //    }
                //}

                if (children.Count == 0)
                {
                    title = tempDict[doc.Address].Find(s => s != "");

                    return new XElement(levelOf(doc.Address),
                                new XAttribute("title", title),
                                new XAttribute("uri", relativeAddressOf(doc.Address))
                        );
                }

                int currentLevel = relativeDepthOf(BaseAddress, doc.Address);
                if (BottomLevel > currentLevel)
                {
                    int bottomCount = 0;
                    for (int i = 0; i < children.Count; i++)
                    {
                        XElement child = children[i];

                        if (child.Name == "folder" && child.HasElements)
                        {
                            List<XElement> grandChildren = child.Elements().ToList();
                            bool needModify = false;

                            for (int j = 0; j < grandChildren.Count; j ++)
                            {
                                XElement grandChild = grandChildren[j];
                                Uri address = new Uri(BaseAddress, grandChild.Attribute("uri").Value);
                                Document childDoc = new Document(address, 0, BaseEncoding);
                                XElement expanded = mapOf(childDoc, tempDict);
                                if (expanded.Nodes().Count() != 0)
                                {
                                    needModify = true;
                                    grandChildren.RemoveAt(j);
                                    grandChildren.Insert(j, expanded);
                                }
                                else
                                {
                                    bottomCount++;
                                    if (bottomCount > 3)
                                    {
                                        BottomLevel = currentLevel;
                                        break;
                                    }
                                }
                            }

                            if (needModify)
                            {
                                children.RemoveAt(i);
                                XElement newChild = new XElement("folder", child.Attributes(), grandChildren);
                                children.Insert(i, newChild);
                            }
                        }
                        else
                        {
                            Uri address = new Uri(BaseAddress, child.Attribute("uri").Value);
                            Document childDoc = new Document(address, 0, BaseEncoding);
                            XElement expanded = mapOf(childDoc, tempDict);
                            if (expanded.Nodes().Count() > 2)
                            {
                                children.RemoveAt(i);
                                children.Insert(i, expanded);
                            }
                            else
                            {
                                bottomCount++;
                                if (bottomCount > 3)
                                {
                                    BottomLevel = currentLevel;
                                    break;
                                }
                            }
                        }
                    }
                }

                title = tempDict[doc.Address].Find(s => s != "");
                return new XElement(levelOf(doc.Address),
                        new XAttribute("title", title),
                        new XAttribute("uri", relativeAddressOf(doc.Address)),
                        children
                        );
            }


        }

        private string relativeAddressOf(Uri uri)
        {
            if (uri == BaseAddress)
                return uri.ToString();
            else
            {
                int last = BaseAddress.AbsolutePath.LastIndexOf('/');

                if (last < 0)
                    throw new Exception();

                string path = BaseAddress.AbsolutePath.Substring(0, last + 1);
                return uri.AbsolutePath.Replace(path, "").Replace('/', '\\');
            }
        }

        private string levelOf(Uri uri)
        {
            return "Level" + relativeDepthOf(BaseAddress, uri).ToString();
        }

        #region Obsoleted codes
        /*/
        private XElement indexOf(Document index)
        {
            List<Uri> existingUri = new List<Uri>();
            System.Diagnostics.Debug.WriteLine("Try parseHtml " + index.ToString());

            int bottom = -1;
            List<XElement> siblingNodes = siblingNodesOf(index, 0, ref bottom, ref existingUri);            

            return siblingNodes == null || siblingNodes.Count == 0 ?
                new XElement(levelOf(index.Address),
                    new XElement("title", index.Title),
                    new XElement("uri", index.Address)
                    ) :
                new XElement(levelOf(index.Address),
                    new XElement("title", index.Title),
                    new XElement("uri", index.Address),
                    siblingNodes.ToArray()
                    );
        }

        private XElement indexOf(Uri uri, int expectedLevel, ref int bottom, ref List<Uri> existingUri)
        {
            Document doc = new Document(uri, 0, BaseEncoding);

            int currentLevel = relativeDepthOf(BaseAddress, uri);

            List<Uri> siblingUri = (from kvp in doc.SiblingUriDict
                                    where relativeDepthOf(BaseAddress, kvp.Value) > expectedLevel
                                    select kvp.Value).ToList();

            if (currentLevel < expectedLevel && siblingUri.Count == 0)
                return new XElement(levelOf(doc.Address),
                    new XElement("title", doc.Title),
                    new XElement("uri", doc.Address)
                    );

            List<XElement> siblingNodes = siblingNodesOf(doc, currentLevel, ref bottom, ref existingUri);

            if (siblingNodes.Count == 0)
                return  new XElement(levelOf(doc.Address),
                    new XElement("title", doc.Title),
                    new XElement("uri", doc.Address)
                    );
            else
                return 
                new XElement(levelOf(doc.Address),
                    new XElement("title", doc.Title),
                    new XElement("uri", doc.Address),
                    siblingNodes
                    );
        }

        private List<XElement> siblingNodesOf(Document doc, int level, ref int bottom, ref List<Uri> existingUri)
        {
            List<XElement> result = new List<XElement>();
            int currentLevel = -1;

            foreach (KeyValuePair<Element, Uri> kvp in doc.SiblingUriDict)
            {
                currentLevel = relativeDepthOf(BaseAddress, kvp.Value);

                if (currentLevel < level)
                    continue;

                if (bottom == -1 || currentLevel > bottom)
                {
                    if (!existingUri.Contains(kvp.Value))
                        existingUri.Add(kvp.Value);
                    result.Add(indexOf(kvp.Value, currentLevel>level ? currentLevel : currentLevel+1, ref bottom, ref existingUri));
                }
                else if (currentLevel == bottom)
                {
                    result.Add( 
                        new XElement(levelOf(kvp.Value),
                                    new XElement("title", kvp.Key.OwnText),
                                    new XElement("uri", kvp.Value)
                                    )
                        );
                }

            }

            return result;
        }



        private XElement parseHtml(Document index, Dictionary<Uri, List<string>> tempDict)
        {
            List<Uri> newSiblings = new List<Uri>();

            string level = levelOf(index.Address);

            System.Diagnostics.Debug.WriteLine("Try parseHtml " + index.ToString());
            if (!tempDict.ContainsKey(index.Address))
                tempDict.Add(index.Address, new List<string> { index.Title });
            else // if (tempDict[doc.Address].Count < 3)
                tempDict[index.Address].Add(index.Title);

            List<XElement> siblingNodes = siblingNodesOf(index, tempDict, ref newSiblings);

            if (newSiblings.Count != 0)
            {
                int oldCount = tempDict.Count;

                int i = 0;
                for (; i < newSiblings.Count; i++)
                {
                    Uri address = newSiblings[i];
                    Document sibling = new Document(address, 0, BaseEncoding);

                    XElement node = parseHtml(sibling, tempDict);

                    if (node != null)
                        siblingNodes.Add(node);

                    if (sibling.SiblingUriDict.Count > 2 && oldCount == tempDict.Count)
                    {
                        return null;
                        //i = Math.Max(existingUri.Count - 2, i);
                    }
                }
                return new XElement(level,
                    new XElement("title", index.Title),
                    new XElement("uri", index.Address),
                    siblingNodes.ToArray()
                    );
            }
            else
            {
                return new XElement(level,
                    new XElement("title", index.Title),
                    new XElement("uri", index.Address)
                    );
            }
        }

        private List<XElement> siblingNodesOf(Document doc, Dictionary<Uri, List<string>> tempDict, ref List<Uri> newSiblings)
        {
            List<XElement> result = new List<XElement>();
            foreach (KeyValuePair<Element, Uri> kvp in doc.SiblingUriDict)
            {
                if (!tempDict.ContainsKey(kvp.Value))
                {
                    tempDict.Add(kvp.Value, new List<string> { kvp.Key.OwnText });
                    newSiblings.Add(kvp.Value);

                    result.Add(
                        new XElement(levelOf(kvp.Value),
                            new XElement("title", kvp.Key.OwnText),
                            new XElement("uri", kvp.Value)
                        ));
                }
                else
                    tempDict[kvp.Value].Add(kvp.Key.OwnText);
            }

            return result;

        }

        private bool isBottom(Document doc, Dictionary<Uri, List<string>> tempDict, ref List<Uri> newSiblings)
        {
            foreach (KeyValuePair<Element, Uri> kvp in doc.SiblingUriDict)
            {
                if (!tempDict.ContainsKey(kvp.Value))
                {
                    tempDict.Add(kvp.Value, new List<string> { kvp.Key.OwnText });
                    newSiblings.Add(kvp.Value);
                }
                else
                    tempDict[kvp.Value].Add(kvp.Key.OwnText);
            }

            return newSiblings.Count == 0;

        }

        //private void parseHtml(Uri rootUri, Dictionary<Uri, List<string>> tempDict)
        //{
        //    int count = 0;

        //    while(hasMoreChild(rootUri, tempDict))
        //    {
        //        Document rootIndex = new Document(rootUri, 0, BaseEncoding);
        //        int oldSiblingCount = tempDict.Count;
        //        foreach (KeyValuePair<Element, Uri> kvp in rootIndex.SiblingUriDict)
        //        {
        //            parseHtml(kvp.Value, tempDict);
        //            if (oldSiblingCount == tempDict.Count)
        //            {
        //                count++;

        //                if (count > 4)
        //                    break;
        //            }
        //        }
        //    }

        //    //int count = 0;
        //    //if (hasMoreChild(rootUri, tempDict))
        //    //{
        //    //    foreach (KeyValuePair<Element, Uri> kvp in rootIndex.SiblingUriDict)
        //    //    {
        //    //        Document siblingDoc = new Document(kvp.Value, 0, BaseEncoding);

        //    //        parseHtml(siblingDoc, tempDict);

        //    //        //if (!hasMoreChild(siblingDoc, tempDict))
        //    //        //{
        //    //        //    count++;
        //    //        //    if (count > 4)
        //    //        //        break;
        //    //        //}
        //    //    }
        //    //}
        //}

        //private bool hasMoreChild(Uri rootUri, Dictionary<Uri, List<string>> tempDict)
        //{
        //    Document rootIndex = new Document(rootUri, 0, BaseEncoding);
        //    int existingUri = 0;

        //    foreach (KeyValuePair<Element, Uri> kvp in rootIndex.SiblingUriDict)
        //    {
        //        Document siblingDoc = new Document(kvp.Value, 0, BaseEncoding);

        //        string title = siblingDoc.Title;

        //        if (tempDict.ContainsKey(kvp.Value))
        //            tempDict[kvp.Value].Add(title);
        //        else
        //        {
        //            existingUri++;
        //            tempDict.Add(kvp.Value, new List<string> { title });
        //        }

        //        //foreach (KeyValuePair<Element, Uri> child in siblingDoc.SiblingUriDict)
        //        //{
        //        //    string childPath = title + "#" + child.Key.OwnText;

        //        //    if (!tempDict.ContainsKey(child.Value))
        //        //        tempDict.Add(child.Value, new List<string>());

        //        //    tempDict[child.Value].Add(childPath);

        //        //    if (tempDict[child.Value].Count >= 3)
        //        //    {
        //        //        break;
        //        //    }

        //        //    //Document grandChild = new Document(child.Value, 0, siblingDoc.BaseEncoding);

        //        //    //foreach(KeyValuePair<Element, Uri> grandLink in grandChild.SiblingUriDict)
        //        //    //{
        //        //    //    childPath = title + "#" + child.Key.OwnText;

        //        //    //    if (!tempDict.ContainsKey(child.Value))
        //        //    //        tempDict.Add(child.Value, new List<string>());

        //        //    //    tempDict[child.Value].Add(childPath);

        //        //    //    if (tempDict[child.Value].Count >= 3)
        //        //    //    {
        //        //    //        break;
        //        //    //    }

        //        //    //}
        //        //}

        //    }

        //    return existingUri != 0;
        //}

        //private void parseSiblingIndex(Document doc, Dictionary<int, Dictionary<string, Document>> SubIndexes)
        //{
        //        //string siblingPath = kvp.Value.AbsolutePath;

        //        //if (Document.IsMeaningfulFilename(kvp.Value.LocalPath))
        //        //{
        //        //    parseSiblingIndex(siblingDoc, SubIndexes);
        //        //}
        //        //else if (siblingDoc.SiblingUriDict.Count != 0 && relativeDepthOf(kvp.Value, siblingDoc.SiblingUriDict.First().Value) > 0)
        //        //{
        //        //    parseSiblingIndex(siblingDoc, SubIndexes);
        //        //}
        //        //else
        //        //{
        //        //    int sequenceId = Document.NumberFromUri(siblingDoc.Address);

        //        //    if (sequenceId == 1)
        //        //    {

        //        //    }

        //        //    foreach (KeyValuePair<Element, Uri> siblingUri in siblingDoc.SiblingUriDict)
        //        //    {
        //        //    }
        //        //}
        //    //int depth = relativeDepthOf(BaseAddress, kvp.Value);

        //    //if (SubIndexes.Contains(depth))
        //    //    SubIndexes.Add(depth, new Dictionary<Uri, Document>());

        //    //SubIndexes[depth].Add(doc.Title, doc);
        //}

        //*/
        #endregion
    }
}
