using System;
using System.Collections.Generic;
using System.Text;

namespace CalastoneWordFilterer.Filters
{
    public interface IFilter
    {
        // Method which returns bool to indicate whether this supplied word should be excluded.
        public bool ExcludeWord(string word);
    }
}
