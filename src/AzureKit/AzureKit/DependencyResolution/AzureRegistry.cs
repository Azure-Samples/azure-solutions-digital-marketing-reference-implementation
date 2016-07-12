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
            Config.DocumentDBConfig dbConfig = new Config.DocumentDBConfig();
            dbConfig.Load();
            For<Config.DocumentDBConfig>().Use(dbConfig);

            Config.AzureBlobConfig blobConfig = new Config.AzureBlobConfig();
            blobConfig.Load();
            For<Config.AzureBlobConfig>().Use(blobConfig);

            //data
            For<Data.ISiteContentRepository>().Use<Data.DocDb.DocDbSiteContentRepository>();
            For<Data.ISiteMapRepository>().Use<Data.DocDb.DocDbSiteMapRepository>();


            //media
            For<Media.IMediaStorage>().Use<Media.AzureBlob.AzureBlobMediaStorage>();

            //cache
            For<Caching.ICacheService>().Use<Caching.Local.LocalCacheService>().SetLifecycleTo(Lifecycles.Singleton);
            //For<Caching.ICacheService>().Use<Caching.Azure.RedisCacheService>();

            //automapper
            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());
            For<IMappingEngine>().Use(()=>Mapper.Engine);
        }
    }
}