using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.BusinessLogic.Interfaces
{
    public  interface IFileService
    {
        void CreateFile(string filePath);

        void CreateDirectory(string directory);

        void DeleteDirectory(string directory);

        void RenameDirectory(string source, string destination);

        bool Exists(string path);
    }
}
