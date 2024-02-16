namespace GTE.Mastery.Documents.Api.Exceptions
{
    [Serializable]
    public class DocumentApiBaseException : Exception
    {
        public DocumentApiBaseException()
        {
        }

        public DocumentApiBaseException(string message) : base(message)
        {
        }

        public DocumentApiBaseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
