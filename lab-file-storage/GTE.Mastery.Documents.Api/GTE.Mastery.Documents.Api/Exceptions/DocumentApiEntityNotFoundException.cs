namespace GTE.Mastery.Documents.Api.Exceptions
{
    [Serializable]
    public class DocumentApiEntityNotFoundException : DocumentApiBaseException
    {
        public DocumentApiEntityNotFoundException()
        {
        }

        public DocumentApiEntityNotFoundException(string message) : base(message)
        {
        }

        public DocumentApiEntityNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
