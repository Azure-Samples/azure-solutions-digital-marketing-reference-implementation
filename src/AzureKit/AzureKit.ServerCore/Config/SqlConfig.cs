using System.Configuration;

namespace AzureKit.Config
{
    public class SqlConfig : ISqlConfig
    {
        public SqlConfig()
        {
            if (!string.IsNullOrWhiteSpace(
                ConfigurationManager.ConnectionStrings[Constants.KEY_SQL_CONNECTION_STRING]?.ConnectionString))
            {
                // EF works with the connection name, so we just give it that, or
                // null if there's no connection present.
                ConnectionName = Constants.KEY_SQL_CONNECTION_STRING;
            }

        }
        public string ConnectionName { get; }
    }
}
