using CalastoneWordFilterer.Consoles;
using CalastoneWordFilterer.Factories;
using CalastoneWordFilterer.FileReaders;
using CalastoneWordFilterer.Filters;
using System.IO;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System;

namespace CalastoneWordFilterer
{
    class Program
    {
        static void Main(string[] args)
        {
            Core coreApp = new Core(new FilterFactory(), new CustomFileReader(), new StandardConsole());

            StreamReader streamReader = null;
            List<IFilter> filtersToApply = null;
            try
            {
                streamReader = coreApp.RequestFileFromUser();

                filtersToApply = coreApp.GetFiltersToApply();
            }
            catch
            {
                Main(args);
            }

            coreApp.PrintFilteredWordsFromStream(streamReader, filtersToApply);
        }
    }
}
