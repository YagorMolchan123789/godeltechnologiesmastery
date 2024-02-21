namespace GTE.Mastery.Documents.Api.Configurations
{
    public sealed class DocumentStorageOptions
    {
        public const string ConfigKey = "DocumentStorage";

        public required string ClientPath { get; set; }

        public required string DocumentPath { get; set; }

        public required string DocumentBlobPath { get; set; }

        public required string ContentPath { get; set; }
    }
}
