using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace CalastoneWordFilterer.FileReaders
{
    public interface IFileReader
    {
        StreamReader GetStreamReader(string path);
    }
}
