using Mastery.KeeFi.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Business.Services
{
    public class FileService : IFileService
    {
        public void CreateFile(string filePath)
        {
            var directory = filePath.Remove(filePath.LastIndexOf("/"));

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(filePath))
            {
                var arr = new JsonArray();
                File.WriteAllText(filePath, arr.ToJsonString());
            }
        }

        public void CreateDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void DeleteDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public void RenameDirectory(string source, string destination)
        {
            Directory.Move(source, destination);
        }
    }
}
