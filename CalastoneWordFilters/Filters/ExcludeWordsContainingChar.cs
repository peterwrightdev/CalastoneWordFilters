using System;
using System.Collections.Generic;
using System.Text;

namespace CalastoneWordFilterer.Filters
{
    public class ExcludeWordsContainingChar :IFilter
    {
        private char _charToExclude;

        public ExcludeWordsContainingChar(char charToExclude = 't')
        {
            this._charToExclude = charToExclude;
        }

        public bool ExcludeWord(string word)
        {
            return word.IndexOf(this._charToExclude, StringComparison.CurrentCultureIgnoreCase) != -1;
        }
    }
}
