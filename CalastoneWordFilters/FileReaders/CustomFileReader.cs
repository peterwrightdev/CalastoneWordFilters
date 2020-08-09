using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace CalastoneWordFilterer.FileReaders
{
    class CustomFileReader : IFileReader
    {
        public StreamReader GetStreamReader(string path)
        {
            return new StreamReader(path);
        }
    }
}
