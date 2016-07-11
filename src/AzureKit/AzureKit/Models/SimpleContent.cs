namespace AzureKit.Models
{
    public class SimpleContent : ContentModelBase
    {
        public SimpleContent() :base(ContentType.Simple)
        {}
        //[AllowHtml]
        //public new string Content {
        //    get { return base.Content; }
        //    set { base.Content = value; }
        //}
    }
}