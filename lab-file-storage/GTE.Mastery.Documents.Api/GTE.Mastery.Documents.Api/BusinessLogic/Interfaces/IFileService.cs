namespace GTE.Mastery.Documents.Api.BusinessLogic.Interfaces
{
    public interface IFileService
    {
        void CreateFile(string filePath);
        void CreateDirectory(string directory);
        void DeleteDirectory(string directory);
        
        void RenameDirectory(string source, string destination);

        bool Exists(string path);
    }
}
