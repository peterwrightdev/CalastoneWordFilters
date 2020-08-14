using CalastoneWordFilterer.Consoles;
using CalastoneWordFilterer.Factories;
using CalastoneWordFilterer.FileReaders;
using CalastoneWordFilterer.Filters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CalastoneWordFilterer
{
    public class Client
    {
        // injected services for use in unit testing and extension

        // used to read .txt file content
        private IFileReader _fileReader;

        // used to generate instances of Filters to apply to the contents of file read
        private IFilterFactory _filterFactory;

        // used to control messaging to and from the user
        private IConsole _console;

        public Client(IFilterFactory filterFactory, IFileReader fileReader, IConsole console)
        {
            this._fileReader = fileReader;
            this._filterFactory = filterFactory;
            this._console = console;
        }

        public StreamReader RequestFileFromUser()
        {
            this._console.WriteLine("Please supply the filename to filter.");
            string filename = this._console.ReadLine();

            try
            {
                return this._fileReader.GetStreamReader(filename);
            }
            catch
            {
                this._console.WriteLine(string.Format("Failed to find file {0}. Please ensure path is correct.", filename));
                this._console.WriteLine(string.Empty);
                throw new FileNotFoundException();
            }
        }

        public List<IFilter> GetFiltersToApply()
        {
            // Read in what filters the user wants to apply - For extendability, assuming that list of filters can grow, no reason to assume user would always want to apply all filters.
            this._console.WriteLine("Choose filters to apply from the following;");

            List<Type> filters = this._filterFactory.GetFilterTypes();

            for (int key = 0; key < filters.Count; key++)
            {
                this._console.WriteLine(string.Format("{0}: {1}", key, filters[key].Name));
            }

            this._console.WriteLine(string.Empty);
            this._console.WriteLine("For example, type '0 1' to apply the first two filters only.");
            string[] keysOfFiltersToApply = this._console.ReadLine().Split(" ").Where(s => !string.IsNullOrEmpty(s)).ToArray();

            List<IFilter> filtersToApply = new List<IFilter>();
            try
            {
                foreach (string key in keysOfFiltersToApply)
                {
                    // This step is more of an extension step to the problem provided. Makes sense to allow users to apply inputs at runtime for variable filters.
                    // For example, while question only raises the case of filtering out words of 3 characters or less, not unreasonable to assume similar filters on 2 or 4 characters may be desired.
                    // By allowing these to be set by users, we don't have to create unique filters for each case.
                    ParameterInfo[] constructorParams = this._filterFactory.GetParametersForFilter(filters[int.Parse(key)]);

                    if (constructorParams.Length > 0)
                    {
                        object[] inputParams = new object[constructorParams.Length];

                        for (int i = 0; i < constructorParams.Count(); i++)
                        {
                            this._console.WriteLine(string.Empty);
                            this._console.WriteLine(string.Format("Filter {0} has {1} parameter {2}. Please enter a {3}{4}",
                                filters[int.Parse(key)].Name,
                                constructorParams[i].IsOptional ? "optional" : "required",
                                constructorParams[i].Name,
                                constructorParams[i].ParameterType.Name,
                                constructorParams[i].IsOptional ? string.Format(", default value if none supplied is {0}", constructorParams[i].DefaultValue.ToString()) : string.Empty));

                            string input = this._console.ReadLine();
                            inputParams[i] = string.IsNullOrEmpty(input) ? constructorParams[i].DefaultValue : Convert.ChangeType(input, constructorParams[i].ParameterType);
                        }

                        filtersToApply.Add(this._filterFactory.CreateFilter(filters[int.Parse(key)], inputParams));

                    }
                    else
                    {
                        filtersToApply.Add(this._filterFactory.CreateFilter(filters[int.Parse(key)]));
                    }

                }
            }
            catch
            {
                this._console.WriteLine("Failed to apply filters.");
                this._console.WriteLine(string.Empty);
                throw new ApplicationException();
            }

            return filtersToApply;
        }

        public void PrintFilteredWordsFromStream(StreamReader streamReader, List<IFilter> filtersToApply)
        {
            List<string> filteredWords = new List<string>();
            while (!streamReader.EndOfStream)
            {
                Regex regex = new Regex("[^a-zA-Z]");
                var line = streamReader.ReadLine();

                // Noticed that in text supplied, seems that file uses period as a separator as well as space. This is non-standard, so in reality would query this with stakeholders to determine how this is expected to be handled.
                var lineWords = line.Split(new char[] { ' ', '.' }).Select(w => regex.Replace(w, string.Empty)).Where(w => !string.IsNullOrEmpty(w));

                foreach (string word in lineWords)
                {
                    // question asks for all words, non-distinct.
                    // If the word has already been added, then the filters must have already been succesfully applied for this word, so no need to run them again.
                    if (filteredWords.Contains(word))
                    {
                        filteredWords.Add(word);
                    }
                    else
                    {
                        bool exclude = false;

                        foreach (IFilter filter in filtersToApply)
                        {
                            if (filter.ExcludeWord(word))
                            {
                                exclude = true;
                                break;
                            }
                        }

                        if (!exclude)
                        {
                            filteredWords.Add(word);
                        }
                    }
                }
            }

            this._console.WriteLine(string.Empty);
            this._console.WriteLine("Result is as follows;");
            this._console.WriteLine(string.Empty);

            this._console.WriteLine(string.Join(" ", filteredWords));
            this._console.ReadLine();
        }
    }
}
