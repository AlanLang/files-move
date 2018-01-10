using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesMove
{
    public class FileConfi
    {
        public string version;
        public string person;
        public string basepath;
        public List<files> files;
    }

    public class files
    {
        public string name;
        public string path;
    }
}
