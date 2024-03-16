namespace Mastery.KeeFi.Api.Configurations
{
    public sealed class DocumentStorageOptions
    {
        public const string ConfigKey = "DocumentStorage";

        public required string DocumentBlobPath { get; set; }
    }
}
