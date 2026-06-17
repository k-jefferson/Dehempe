namespace TLSiKitEditeur.SoapService
{
    public class DocumentIdentity
    {
        private string repositoryUniqueId;

        public string GetRepositoryUniqueId()
        {
            return this.repositoryUniqueId;
        }
        public void SetRepositoryUniqueId(string value)
        {
            this.repositoryUniqueId = value;
        }

        private string documentUniqueId;
        public string GetDocumentUniqueId()
        {
            return this.documentUniqueId;
        }
        public void SetDocumentUniqueId(string value)
        {
            this.documentUniqueId = value;
        }
    }
}
