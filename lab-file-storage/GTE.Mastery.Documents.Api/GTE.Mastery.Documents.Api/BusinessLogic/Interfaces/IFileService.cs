namespace GTE.Mastery.Documents.Api.BusinessLogic.Interfaces
{
    public interface IFileService
    {
        void CreateFile(string filePath);
        void CreateDirectory(string directory);
    }
}
