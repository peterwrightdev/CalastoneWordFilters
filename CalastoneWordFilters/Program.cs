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
            Client client = new Client(new FilterFactory(), new CustomFileReader(), new StandardConsole());

            StreamReader streamReader = null;
            List<IFilter> filtersToApply = null;
            try
            {
                streamReader = client.RequestFileFromUser();

                filtersToApply = client.GetFiltersToApply();
            }
            catch
            {
                Main(args);
                Environment.Exit(0);
            }

            client.PrintFilteredWordsFromStream(streamReader, filtersToApply);
        }
    }
}
