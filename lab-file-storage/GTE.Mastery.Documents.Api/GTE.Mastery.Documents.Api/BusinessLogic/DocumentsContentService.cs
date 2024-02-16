using Bogus;

namespace GTE.Mastery.Documents.Api.BusinessLogic
{
    public class DocumentsContentService : IDocumentsContentService
    {
        private readonly string _blobPath;

        public DocumentsContentService(string blobPath)
        {
            _blobPath = blobPath;
        }

        public async Task UploadDocumentAsync(int clientId, int documentId, MemoryStream content)
        {
            // TODO: Add your implementation here
            throw new NotImplementedException();
        }

        public async Task<ReceiveDocumentResponse> ReceiveDocumentAsync(int clientId, int documentId)
        {
            #region TODO: replace this code block by your implementation

            Faker<DocumentMetadata>? faker = new Faker<DocumentMetadata>()
                // Ensure the ID remains consistent with the input
                .RuleFor(d => d.Id, _ => documentId)
                // Ensure the ClientId remains consistent with the input
                .RuleFor(d => d.ClientId, _ => clientId)
                .RuleFor(d => d.FileName, _ => "logo.svg")
                .RuleFor(d => d.Title, f => f.Lorem.Sentence())
                .RuleFor(d => d.Description, f => f.Lorem.Paragraph())
                .RuleFor(d => d.ContentLength, f => f.Random.Int(0, 100000))
                .RuleFor(d => d.ContentType, f => f.System.MimeType())
                .RuleFor(d => d.ContentMd5, f => Guid.NewGuid().ToString())
                .RuleFor(d => d.Properties,
                    f => new Dictionary<string, string>(f.Make(1,
                        () => new KeyValuePair<string, string>(f.Lorem.Word(), f.Lorem.Word()))));

            DocumentMetadata? updatedDocument = faker.Generate();
            string encodedDocument =
                "PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAxMjggMTI4Ij48cGF0aCBmaWxsPSIjOUI0Rjk2IiBkPSJNMTE1LjQgMzAuN0w2Ny4xIDIuOWMtLjgtLjUtMS45LS43LTMuMS0uNy0xLjIgMC0yLjMuMy0zLjEuN2wtNDggMjcuOWMtMS43IDEtMi45IDMuNS0yLjkgNS40djU1LjdjMCAxLjEuMiAyLjQgMSAzLjVsMTA2LjgtNjJjLS42LTEuMi0xLjUtMi4xLTIuNC0yLjd6Ii8+PHBhdGggZmlsbD0iIzY4MjE3QSIgZD0iTTEwLjcgOTUuM2MuNS44IDEuMiAxLjUgMS45IDEuOWw0OC4yIDI3LjljLjguNSAxLjkuNyAzLjEuNyAxLjIgMCAyLjMtLjMgMy4xLS43bDQ4LTI3LjljMS43LTEgMi45LTMuNSAyLjktNS40VjM2LjFjMC0uOS0uMS0xLjktLjYtMi44bC0xMDYuNiA2MnoiLz48cGF0aCBmaWxsPSIjZmZmIiBkPSJNODUuMyA3Ni4xQzgxLjEgODMuNSA3My4xIDg4LjUgNjQgODguNWMtMTMuNSAwLTI0LjUtMTEtMjQuNS0yNC41czExLTI0LjUgMjQuNS0yNC41YzkuMSAwIDE3LjEgNSAyMS4zIDEyLjVsMTMtNy41Yy02LjgtMTEuOS0xOS42LTIwLTM0LjMtMjAtMjEuOCAwLTM5LjUgMTcuNy0zOS41IDM5LjVzMTcuNyAzOS41IDM5LjUgMzkuNWMxNC42IDAgMjcuNC04IDM0LjItMTkuOGwtMTIuOS03LjZ6TTk3IDY2LjJsLjktNC4zaC00LjJ2LTQuN2g1LjFMMTAwIDUxaDQuOWwtMS4yIDYuMWgzLjhsMS4yLTYuMWg0LjhsLTEuMiA2LjFoMi40djQuN2gtMy4zbC0uOSA0LjNoNC4ydjQuN2gtNS4xbC0xLjIgNmgtNC45bDEuMi02aC0zLjhsLTEuMiA2aC00LjhsMS4yLTZoLTIuNHYtNC43SDk3em00LjggMGgzLjhsLjktNC4zaC0zLjhsLS45IDQuM3oiLz48L3N2Zz4=";
            return new ReceiveDocumentResponse(new MemoryStream(Convert.FromBase64String(encodedDocument)), updatedDocument);

            #endregion TODO: replace this code block by your implementation
        }
    }
}
