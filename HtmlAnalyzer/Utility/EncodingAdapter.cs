using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlAnalyzer.Utility
{
    public static class EncodingAdapter
    {
        public static byte[] CharsetIndicator = null;
        public static BoyerMooreString CharsetPattern = null;

        static EncodingAdapter()
        {
            CharsetIndicator = Encoding.UTF8.GetBytes("charset=");
            CharsetPattern = new BoyerMooreString("charset=");
        }

        //static int search(byte[] chars, byte[] indicator)
        //{
        //    for (int i = 0; i <= chars.Length - indicator.Length; i++)
        //    {
        //        if (match(chars, indicator, i))
        //        {
        //            return i;
        //        }
        //    }
        //    return -1;
        //}

        //static bool match(byte[] chars, byte[] indicator, int start)
        //{
        //    if (indicator.Length + start > chars.Length)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        for (int i = 0; i < indicator.Length; i++)
        //        {
        //            if (indicator[i] != chars[i + start])
        //            {
        //                return false;
        //            }
        //        }
        //        return true;
        //    }
        //}

        public static bool TryParse(byte[] buffer, ref Encoding encoding, ref string content)
        {
            try
            {
                if (encoding == null)
                {
                    encoding = Encoding.Default;

                    //int start = search(buffer, CharsetBytes);
                    int start = BoyerMooreBinary.IndexOf(CharsetIndicator, buffer, 4096);

                    if (start != -1)
                    {
                        start += CharsetIndicator.Length;
                        byte endIndicator = (byte)'"';
                        int end = Array.IndexOf(buffer, endIndicator, start);
                        if (end != -1)
                        {
                            byte[] charset = new byte[end - start];
                            Array.Copy(buffer, start, charset, 0, charset.Length);
                            string charsetString = Encoding.Default.GetString(charset);
                            encoding = Encoding.GetEncoding(charsetString);
                        }
                    }
                }
                else
                {
                    content = encoding.GetString(buffer);

                    //int start = content.IndexOf("charset=");
                    //int start = CharsetPattern.Search(content);

                    //if(start == -1)
                    //{
                    //    return true;
                    //}

                    //start += CharsetBytes.Length;
                    //int end = content.IndexOf('"', start);

                    //string charsetString = content.Substring(start, end - start);
                    string charsetString = BoyerMooreString.FirstTextBetween(content, CharsetPattern, '"');
                    if (charsetString != null && charsetString != encoding.HeaderName)
                        encoding = Encoding.GetEncoding(charsetString);

                    return true;
                }

                return true;

            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }

        }
    }
}
