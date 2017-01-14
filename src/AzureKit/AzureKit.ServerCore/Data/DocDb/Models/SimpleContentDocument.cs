namespace AzureKit.Data.DocDb.Models
{
    public class SimpleContentDocument :SiteContentItemDocument
    {
        public SimpleContentDocument()
        {
            base.ContentType = AzureKit.Models.ContentType.Simple.ToString();
        }
    }
}