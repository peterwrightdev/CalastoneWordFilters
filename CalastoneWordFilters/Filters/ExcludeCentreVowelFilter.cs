using System;
using System.Collections.Generic;
using System.Text;

namespace CalastoneWordFilterer.Filters
{
    public class ExcludeCentreVowelFilter : IFilter
    {
        // Method which returns bool to indicate whether this supplied word should be excluded.
        public bool ExcludeWord(string word)
        {
            // if even number of characters, we want to exclude the word if either of the "middle" characters are vowels
            if (word.Length % 2 == 0)
            {
                return "aeiouAEIOU".Contains(word[word.Length / 2]) || "aeiouAEIOU".Contains(word[word.Length / 2 - 1]);
            }
            else
            {
                // Check if 
                return "aeiouAEIOU".Contains(word[word.Length / 2]);
            }
        }
    }
}
