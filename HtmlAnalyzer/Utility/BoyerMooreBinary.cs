using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlAnalyzer.Utility
{
    /// <summary>
    /// Byte array search utility.
    /// </summary>
    /// <see cref="http://blog.csdn.net/sealyao/article/details/4568167"/>
    public static class BoyerMooreBinary
    {
        private static int ALPHABET_SIZE = 256;

        private static byte[] Text;
        private static byte[] Pattern;

        private static int[] last;
        private static int[] match;
        private static int[] suffix;

        /**
        * Searches the Pattern in the Text.
        * returns the position of the lastTag occurrence, if found and -1 otherwise.
        */
        public static int IndexOf(byte[] pattern, byte[] text, int length) 
        {
            Text = new byte[length];
            Array.Copy(text, Text, length);
            Pattern = pattern;
            last = new int[ALPHABET_SIZE];
            match = new int[Pattern.Length];
            suffix = new int[Pattern.Length];

            // Preprocessing
            ComputeLast();
            ComputeMatch(); 

            // Searching
            int i = pattern.Length - 1;
            int j = pattern.Length - 1;    
            while (i < text.Length) 
            {
                if (pattern[j] == text[i]) 
                {
                    if (j == 0) 
                    { 
                        return i;
                    }
                    j--;
                    i--;
                } 
                else 
                {
                  i += pattern.Length - j - 1 + Math.Max(j - last[text[i]], match[j]);
                  j = pattern.Length - 1;
              }
            }
            return -1;    
          }  

        public static int IndexOf(byte[] pattern, byte[] text)
        {
            return IndexOf(pattern, text, text.Length);
        }


        /**
        * Computes the function lastTag and stores its values in the array lastTag.
        * lastTag(Char ch) = the index of the right-most occurrence of the character ch
        *                                                           in the Pattern; 
        *                 -1 if ch does not occur in the Pattern.
        */
        private static void ComputeLast() {
            for (int k = 0; k < last.Length; k++) { 
                last[k] = -1;
            }
            for (int j = Pattern.Length-1; j >= 0; j--) {
                if (last[Pattern[j]] < 0) {
                    last[Pattern[j]] = j;
                }
            }
        }


        /**
        * Computes the function match and stores its values in the array match.
        * match(j) = min{ s | 0 < s <= j && p[j-s]!=p[j]
        *                            && p[j-s+1]..p[m-s-1] is suffix of p[j+1]..p[m-1] }, 
        *                                                         if such s exists, else
        *            min{ s | j+1 <= s <= m 
        *                            && p[0]..p[m-s-1] is suffix of p[j+1]..p[m-1] }, 
        *                                                         if such s exists,
        *            m, otherwise,
        * where p is the Pattern and m is its length.
        */
        private static void ComputeMatch() {
            /* Phase 1 */
            for (int j = 0; j < match.Length; j++) { 
                match[j] = match.Length;
            } //O(m) 

            ComputeSuffix(); //O(m)

            /* Phase 2 */
            //Uses an auxiliary array, backwards version of the KMP failure function.
            //suffix[i] = the smallest j > i s.t. p[j..m-1] is a prefix of p[i..m-1],
            //if there is no such j, suffix[i] = m

            //Compute the smallest shift s, such that 0 < s <= j and
            //p[j-s]!=p[j] and p[j-s+1..m-s-1] is suffix of p[j+1..m-1] or j == m-1}, 
            //                                                         if such s exists,
            for (int i = 0; i < match.Length - 1; i++) {
                int j = suffix[i + 1] - 1; // suffix[i+1] <= suffix[i] + 1
                if (suffix[i] > j) { // therefore Pattern[i] != Pattern[j]
                    match[j] = j - i;
                } 
                else {// j == suffix[i]
                    match[j] = Math.Min(j - i + match[i], match[j]);
                }
            }

            /* Phase 3 */
            //Uses the suffix array to compute each shift s such that
            //p[0..m-s-1] is a suffix of p[j+1..m-1] with j < s < m
            //and stores the minimum of this shift and the previously computed one.
            if (suffix[0] < Pattern.Length) {
                for (int j = suffix[0] - 1; j >= 0; j--) {
                    if (suffix[0] < match[j]) { match[j] = suffix[0]; }
                }
                {
                    int j = suffix[0];
                    for (int k = suffix[j]; k < Pattern.Length; k = suffix[k]) {
                        while (j < k) {
                            if (match[j] > k) {
                                match[j] = k;
                            }
                            j++;
                        }
                    }
                }
            }
        }


        /**
        * Computes the values of suffix, which is an auxiliary array, 
        * backwards version of the KMP failure function.
        * 
        * suffix[i] = the smallest j > i s.t. p[j..m-1] is a prefix of p[i..m-1],
        * if there is no such j, suffix[i] = m, i.e. 

        * p[suffix[i]..m-1] is the longest prefix of p[i..m-1], if suffix[i] < m.
        */
        private static void ComputeSuffix() 
        {        
            suffix[suffix.Length-1] = suffix.Length;            
            int j = suffix.Length - 1;
            for (int i = suffix.Length - 2; i >= 0; i--) {  
                while (j < suffix.Length - 1 && !Pattern[j].Equals(Pattern[i])) {
                    j = suffix[j + 1] - 1;
                }
                if (Pattern[j] == Pattern[i]) { 
                    j--; 
                }
                suffix[i] = j + 1;
            }
        }


    }

    public static class BoyerMooreChar
    {
        private static int ALPHABET_SIZE = 256;

        private static char[] Text;
        private static char[] Pattern;

        private static int[] last;
        private static int[] match;
        private static int[] suffix;

        /**
        * Searches the Pattern in the Text.
        * returns the position of the lastTag occurrence, if found and -1 otherwise.
        */
        public static int IndexOf(char[] pattern, char[] text, int length) 
        {
            Text = new char[length];
            Array.Copy(text, Text, length);
            Pattern = pattern;
            last = new int[ALPHABET_SIZE];
            match = new int[Pattern.Length];
            suffix = new int[Pattern.Length];

            // Preprocessing
            ComputeLast();
            ComputeMatch(); 

            // Searching
            int i = pattern.Length - 1;
            int j = pattern.Length - 1;    
            while (i < text.Length) 
            {
                if (pattern[j] == text[i]) 
                {
                    if (j == 0) 
                    { 
                        return i;
                    }
                    j--;
                    i--;
                } 
                else 
                {
                  i += pattern.Length - j - 1 + Math.Max(j - last[text[i]], match[j]);
                  j = pattern.Length - 1;
              }
            }
            return -1;    
          }  

        public static int IndexOf(char[] pattern, char[] text)
        {
            return IndexOf(pattern, text, text.Length);
        }


        /**
        * Computes the function lastTag and stores its values in the array lastTag.
        * lastTag(Char ch) = the index of the right-most occurrence of the character ch
        *                                                           in the Pattern; 
        *                 -1 if ch does not occur in the Pattern.
        */
        private static void ComputeLast() {
            for (int k = 0; k < last.Length; k++) { 
                last[k] = -1;
            }
            for (int j = Pattern.Length-1; j >= 0; j--) {
                if (last[Pattern[j]] < 0) {
                    last[Pattern[j]] = j;
                }
            }
        }


        /**
        * Computes the function match and stores its values in the array match.
        * match(j) = min{ s | 0 < s <= j && p[j-s]!=p[j]
        *                            && p[j-s+1]..p[m-s-1] is suffix of p[j+1]..p[m-1] }, 
        *                                                         if such s exists, else
        *            min{ s | j+1 <= s <= m 
        *                            && p[0]..p[m-s-1] is suffix of p[j+1]..p[m-1] }, 
        *                                                         if such s exists,
        *            m, otherwise,
        * where p is the Pattern and m is its length.
        */
        private static void ComputeMatch() {
            /* Phase 1 */
            for (int j = 0; j < match.Length; j++) { 
                match[j] = match.Length;
            } //O(m) 

            ComputeSuffix(); //O(m)

            /* Phase 2 */
            //Uses an auxiliary array, backwards version of the KMP failure function.
            //suffix[i] = the smallest j > i s.t. p[j..m-1] is a prefix of p[i..m-1],
            //if there is no such j, suffix[i] = m

            //Compute the smallest shift s, such that 0 < s <= j and
            //p[j-s]!=p[j] and p[j-s+1..m-s-1] is suffix of p[j+1..m-1] or j == m-1}, 
            //                                                         if such s exists,
            for (int i = 0; i < match.Length - 1; i++) {
                int j = suffix[i + 1] - 1; // suffix[i+1] <= suffix[i] + 1
                if (suffix[i] > j) { // therefore Pattern[i] != Pattern[j]
                    match[j] = j - i;
                } 
                else {// j == suffix[i]
                    match[j] = Math.Min(j - i + match[i], match[j]);
                }
            }

            /* Phase 3 */
            //Uses the suffix array to compute each shift s such that
            //p[0..m-s-1] is a suffix of p[j+1..m-1] with j < s < m
            //and stores the minimum of this shift and the previously computed one.
            if (suffix[0] < Pattern.Length) {
                for (int j = suffix[0] - 1; j >= 0; j--) {
                    if (suffix[0] < match[j]) { match[j] = suffix[0]; }
                }
                {
                    int j = suffix[0];
                    for (int k = suffix[j]; k < Pattern.Length; k = suffix[k]) {
                        while (j < k) {
                            if (match[j] > k) {
                                match[j] = k;
                            }
                            j++;
                        }
                    }
                }
            }
        }


        /**
        * Computes the values of suffix, which is an auxiliary array, 
        * backwards version of the KMP failure function.
        * 
        * suffix[i] = the smallest j > i s.t. p[j..m-1] is a prefix of p[i..m-1],
        * if there is no such j, suffix[i] = m, i.e. 

        * p[suffix[i]..m-1] is the longest prefix of p[i..m-1], if suffix[i] < m.
        */
        private static void ComputeSuffix() 
        {        
            suffix[suffix.Length-1] = suffix.Length;            
            int j = suffix.Length - 1;
            for (int i = suffix.Length - 2; i >= 0; i--) {  
                while (j < suffix.Length - 1 && !Pattern[j].Equals(Pattern[i])) {
                    j = suffix[j + 1] - 1;
                }
                if (Pattern[j] == Pattern[i]) { 
                    j--; 
                }
                suffix[i] = j + 1;
            }
        }


    }

    //public class BoyerMooreBinary<T> where T : IEquatable
    //{
    //    private static int ALPHABET_SIZE = 256;

    //    private static T[] Text;
    //    private static T[] Pattern;

    //    private static int[] lastTag;
    //    private static int[] match;
    //    private static int[] suffix;

    //    /**
    //    * Searches the Pattern in the Text.
    //    * returns the position of the lastTag occurrence, if found and -1 otherwise.
    //    */
    //    public static int IndexOf(T[] pattern, T[] text, int length)
    //    {
    //        Text = new T[length];
    //        Array.Copy(text, Text, length);
    //        Pattern = pattern;
    //        lastTag = new int[ALPHABET_SIZE];
    //        match = new int[Pattern.Length];
    //        suffix = new int[Pattern.Length];

    //        // Preprocessing
    //        ComputeLast();
    //        ComputeMatch();

    //        // Searching
    //        int i = pattern.Length - 1;
    //        int j = pattern.Length - 1;
    //        while (i < text.Length)
    //        {
    //            if (pattern[j] == text[i])
    //            {
    //                if (j == 0)
    //                {
    //                    return i;
    //                }
    //                j--;
    //                i--;
    //            }
    //            else
    //            {
    //                i += pattern.Length - j - 1 + Math.Max(j - lastTag[text[i]], match[j]);
    //                j = pattern.Length - 1;
    //            }
    //        }
    //        return -1;
    //    }

    //    public static int IndexOf(T[] pattern, T[] text)
    //    {
    //        return IndexOf(pattern, text, text.Length);
    //    }


    //    /**
    //    * Computes the function lastTag and stores its values in the array lastTag.
    //    * lastTag(Char ch) = the index of the right-most occurrence of the character ch
    //    *                                                           in the Pattern; 
    //    *                 -1 if ch does not occur in the Pattern.
    //    */
    //    private static void ComputeLast()
    //    {
    //        for (int k = 0; k < lastTag.Length; k++)
    //        {
    //            lastTag[k] = -1;
    //        }
    //        for (int j = Pattern.Length - 1; j >= 0; j--)
    //        {
    //            if (lastTag[Pattern[j]] < 0)
    //            {
    //                lastTag[Pattern[j]] = j;
    //            }
    //        }
    //    }


    //    /**
    //    * Computes the function match and stores its values in the array match.
    //    * match(j) = min{ s | 0 < s <= j && p[j-s]!=p[j]
    //    *                            && p[j-s+1]..p[m-s-1] is suffix of p[j+1]..p[m-1] }, 
    //    *                                                         if such s exists, else
    //    *            min{ s | j+1 <= s <= m 
    //    *                            && p[0]..p[m-s-1] is suffix of p[j+1]..p[m-1] }, 
    //    *                                                         if such s exists,
    //    *            m, otherwise,
    //    * where p is the Pattern and m is its length.
    //    */
    //    private static void ComputeMatch()
    //    {
    //        /* Phase 1 */
    //        for (int j = 0; j < match.Length; j++)
    //        {
    //            match[j] = match.Length;
    //        } //O(m) 

    //        ComputeSuffix(); //O(m)

    //        /* Phase 2 */
    //        //Uses an auxiliary array, backwards version of the KMP failure function.
    //        //suffix[i] = the smallest j > i s.t. p[j..m-1] is a prefix of p[i..m-1],
    //        //if there is no such j, suffix[i] = m

    //        //Compute the smallest shift s, such that 0 < s <= j and
    //        //p[j-s]!=p[j] and p[j-s+1..m-s-1] is suffix of p[j+1..m-1] or j == m-1}, 
    //        //                                                         if such s exists,
    //        for (int i = 0; i < match.Length - 1; i++)
    //        {
    //            int j = suffix[i + 1] - 1; // suffix[i+1] <= suffix[i] + 1
    //            if (suffix[i] > j)
    //            { // therefore Pattern[i] != Pattern[j]
    //                match[j] = j - i;
    //            }
    //            else
    //            {// j == suffix[i]
    //                match[j] = Math.Min(j - i + match[i], match[j]);
    //            }
    //        }

    //        /* Phase 3 */
    //        //Uses the suffix array to compute each shift s such that
    //        //p[0..m-s-1] is a suffix of p[j+1..m-1] with j < s < m
    //        //and stores the minimum of this shift and the previously computed one.
    //        if (suffix[0] < Pattern.Length)
    //        {
    //            for (int j = suffix[0] - 1; j >= 0; j--)
    //            {
    //                if (suffix[0] < match[j]) { match[j] = suffix[0]; }
    //            }
    //            {
    //                int j = suffix[0];
    //                for (int k = suffix[j]; k < Pattern.Length; k = suffix[k])
    //                {
    //                    while (j < k)
    //                    {
    //                        if (match[j] > k)
    //                        {
    //                            match[j] = k;
    //                        }
    //                        j++;
    //                    }
    //                }
    //            }
    //        }
    //    }


    //    /**
    //    * Computes the values of suffix, which is an auxiliary array, 
    //    * backwards version of the KMP failure function.
    //    * 
    //    * suffix[i] = the smallest j > i s.t. p[j..m-1] is a prefix of p[i..m-1],
    //    * if there is no such j, suffix[i] = m, i.e. 

    //    * p[suffix[i]..m-1] is the longest prefix of p[i..m-1], if suffix[i] < m.
    //    */
    //    private static void ComputeSuffix()
    //    {
    //        suffix[suffix.Length - 1] = suffix.Length;
    //        int j = suffix.Length - 1;
    //        for (int i = suffix.Length - 2; i >= 0; i--)
    //        {
    //            while (j < suffix.Length - 1 && !Pattern[j].Equals(Pattern[i]))
    //            {
    //                j = suffix[j + 1] - 1;
    //            }
    //            if (Pattern[j] == Pattern[i])
    //            {
    //                j--;
    //            }
    //            suffix[i] = j + 1;
    //        }
    //    }


    //}



        //private static void preBmBc(byte[] x, int m, int bmBc[]) 
        //{  
  
        //   int i;  
          
        //   for (i = 0; i < ASIZE; ++i)  
          
        //      bmBc[i] = m;  
          
        //   for (i = 0; i < m - 1; ++i)  
          
        //      bmBc[x[i]] = m - i - 1;  
          
        //}  
  
        //private static void suffixes(byte[] x, int m, int *suff) {  
          
        //   int f, g, i;  
          
        //  f = 0；  
          
        //   suff[m - 1] = m;  
          
        //   g = m - 1;  
          
        //   for (i = m - 2; i >= 0; --i) {  
          
        //      if (i > g && suff[i + m - 1 - f] < i - g)  
          
        //         suff[i] = suff[i + m - 1 - f];  
          
        //      else {  
          
        //         if (i < g)  
          
        //            g = i;  
          
        //         f = i;  
          
        //         while (g >= 0 && x[g] == x[g + m - 1 - f])  
          
        //            --g;  
          
        //         suff[i] = f - g;  
          
        //      }  
          
        //   }  
          
        //}  
          
        //private static void preBmGs(byte[] x, int m, int bmGs[]) {  
          
        //   int i, j, suff[XSIZE];  
          
        //   suffixes(x, m, suff);  
          
        //   for (i = 0; i < m; ++i)  
          
        //      bmGs[i] = m;  
          
        //   j = 0;  
          
        //   for (i = m - 1; i >= 0; --i)  
          
        //      if (suff[i] == i + 1)  
          
        //         for (; j < m - 1 - i; ++j)  
          
        //            if (bmGs[j] == m)  
          
        //               bmGs[j] = m - 1 - i;  
          
        //   for (i = 0; i <= m - 2; ++i)  
          
        //      bmGs[m - 1 - suff[i]] = m - 1 - i;  
          
        //}  
          
        //public static void BM(byte[] x, int m, byte[] y, int n) 
        //{  
          
        //   int i, j, bmGs[XSIZE], bmBc[ASIZE];  
          
        //   /* Preprocessing */  
          
        //   preBmGs(x, m, bmGs);  
          
        //   preBmBc(x, m, bmBc);  
          
        //   /* Searching */  
          
        //   j = 0;  
          
        //   while (j <= n - m) {  
          
        //      for (i = m - 1; i >= 0 && x[i] == y[i + j]; --i);  
          
        //      if (i < 0) {  
          
        //         OUTPUT(j);  
          
        //         j += bmGs[0];  
          
        //      }  
          
        //      else  
          
        //         j += MAX(bmGs[i], bmBc[y[i + j]] - m + 1 + i);  
          
        //   }  
          
        //}  
    //}
}
