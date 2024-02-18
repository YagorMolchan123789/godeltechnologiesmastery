using System.Text.Json.Nodes;

namespace GTE.Mastery.Documents.Api.BusinessLogic
{
    public class FileService : IFileService
    {
        public void CreateFile(string filePath)
        {
            var directory = filePath.Remove(filePath.LastIndexOf("\\"));
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var arr = new JsonArray();
            File.WriteAllText(filePath, arr.ToJsonString());
        }
    }
}
