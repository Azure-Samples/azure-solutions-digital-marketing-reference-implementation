using AutoMapper;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace AzureKit.DependencyResolution
{
    /// <summary>
    /// the registry used to setup DI for doc db and azure blob
    /// This could be replace or changed for custom implementations
    /// </summary>
    public class AzureRegistry : Registry
    {
        public AzureRegistry()
        {
            //configuration - use single instances
            For<Config.IDocumentDBConfig>().Use<Config.DocumentDBConfig>().Singleton();
            
            For<Config.AzureBlobConfig>().Use<Config.AzureBlobConfig>().Singleton();

            //data
            For<Data.ISiteContentRepository>().Use<Data.DocDb.DocDbSiteContentRepository>();
            For<Data.ISiteMapRepository>().Use<Data.DocDb.DocDbSiteMapRepository>();
            For<Data.IUserProfileRepository>().Use<Data.Sql.SqlUserProfileRepository>();

            //media
            For<Media.IMediaStorage>().Use<Media.AzureBlob.AzureBlobMediaStorage>();

            //automapper
            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());
            For<IMappingEngine>().Use(()=>Mapper.Engine);
        }
    }
}