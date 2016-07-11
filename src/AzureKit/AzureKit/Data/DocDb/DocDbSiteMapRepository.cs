using AutoMapper;
using AzureKit.Caching;
using System.Linq;
using AzureKit.Models;
using Microsoft.Azure.Documents.Client;
using AzureKit.Data.DocDb.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents.Linq;

namespace AzureKit.Data.DocDb
{
    /// <summary>
    /// Document DB implementation for storing the site map
    /// </summary>
    public class DocDbSiteMapRepository : ISiteMapRepository
    {
        private IMappingEngine map;
        private ICacheService cache;
        private static DocumentClient docDbClient;
        private static Config.DocumentDBConfig config;
        private const string CACHE_KEY_PREFIX = "DocDbSiteMap:";
        private const string KEY_SITE_MAP = "siteMap";

        static DocDbSiteMapRepository()
        {
            config = new Config.DocumentDBConfig();
            config.Load();

            if (config.DatabaseUrl != null &&
                !string.IsNullOrEmpty(config.AccessKey))
            {
                docDbClient = new DocumentClient(
                    config.DatabaseUrl, config.AccessKey,
                    config.ConnectionPolicy,
                    config.Consistency);
            }
        }

        public DocDbSiteMapRepository(IMappingEngine mapper, ICacheService cacheService)
        {
            map = mapper;
            cache = cacheService; 
        }

        public async Task<SiteMap> GetMapAsync()
        {
            //try to get the map from cache
            var cachedMap = cache.GetItem<SiteMap>(CACHE_KEY_PREFIX + KEY_SITE_MAP);

            if (cachedMap != null)
            {
                return cachedMap; 
            }
            if (docDbClient == null)
            {
                //if we can't connect to the store, return null because we won't be able to save the map anyway
                return null;
            }

            //retrieve from docdb if not in cache
            var query = (from smd in docDbClient.CreateDocumentQuery<SiteMapDocument>(config.SiteContentCollectionUrl)
                        where smd.DocumentType == "SiteMap"
                        && smd.Id == KEY_SITE_MAP
                        select smd).AsDocumentQuery();
            var queryResult = await query.ExecuteNextAsync<SiteMapDocument>();
            SiteMapDocument siteMap = queryResult.FirstOrDefault();

            if(siteMap!= null)
            {
                //map to ui model
                SiteMap foundMap =  map.Mapper.Map<SiteMap>(siteMap);

                //put in cache for next time
                cache.PutItem<AzureKit.Models.SiteMap>(CACHE_KEY_PREFIX + KEY_SITE_MAP, foundMap);

                //return
                return foundMap;
            }
            else
            {
                ///default to an empty site map with the right key if we have a DocDBClient but not found map
                return new SiteMap { Id = KEY_SITE_MAP , Entries=new List<SiteMapEntry>()};
            }

        }
        private async Task SaveMapAsync(SiteMap newMap)
        {
            //update the cache
           
            var siteMapDoc = map.Mapper.Map<SiteMapDocument>(newMap);
            try {
                var updatedMap = await docDbClient.UpsertDocumentAsync(config.SiteContentCollectionUrl, siteMapDoc);
                
                cache.PutItem<SiteMap>(CACHE_KEY_PREFIX + KEY_SITE_MAP, newMap);

            }
            catch(Exception ex)
            {
                throw new ApplicationException("Unable to save site map " + ex);
            }
        }
        public async Task AddItemToSiteMapAsync(SiteMapEntry newEntry)
        {
            var map = await GetMapAsync();
            if(map != null)
            {
                map.Entries.Add(newEntry);
                await SaveMapAsync(map);

            }
        }

        public async Task RemoveItemFromSiteMapAsync(string entryToRemove)
        {
            var map = await GetMapAsync();
            var entry = map.Entries.Where(e => e.ContentIdentifier == entryToRemove).Select(e => e).FirstOrDefault();
            if (entry != null)
            {
                map.Entries.Remove(entry);

                await SaveMapAsync(map);
            }
        }

        public async Task<bool> IsItemInSiteMap(string contentIdentifier)
        {
            var map = await GetMapAsync();
            if (map != null)
            {
                foreach (var item in map.Entries)
                {
                    if (item.ContentIdentifier == contentIdentifier)
                    {
                        return true;
                    }
                }
            }
           return false;
        }
    }
}