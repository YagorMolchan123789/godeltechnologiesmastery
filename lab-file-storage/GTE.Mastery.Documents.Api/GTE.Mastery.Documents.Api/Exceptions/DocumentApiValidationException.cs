namespace GTE.Mastery.Documents.Api.Exceptions
{
    [Serializable]
    public sealed class DocumentApiValidationException : DocumentApiBaseException
    {
        public DocumentApiValidationException()
        {
        }

        public DocumentApiValidationException(string message) : base(message)
        {
        }

        public DocumentApiValidationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}