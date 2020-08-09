using System;
using System.Collections.Generic;
using System.Text;

namespace CalastoneWordFilterer.Filters
{
    public class ExcludeShortWordsFilter : IFilter
    {
        private int _minLength;

        public ExcludeShortWordsFilter(int minLength = 3)
        {
            this._minLength = minLength;
        }

        public bool ExcludeWord(string word)
        {
            return word.Length < this._minLength;
        }
    }
}
