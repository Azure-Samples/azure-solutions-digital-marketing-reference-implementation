using AutoMapper;
using AzureKit.Caching;
using AzureKit.Data.DocDb.Models;
using AzureKit.Models;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureKit.Data.DocDb
{
    /// <summary>
    /// Document DB implementation for storing the site map
    /// </summary>
    public class DocDbSiteMapRepository : ISiteMapRepository
    {
        private IMappingEngine _map;
        private ICacheService _cache;
        private static DocumentClient _docDbClient;
        private static Config.DocumentDBConfig _config;
        private const string CACHE_KEY_PREFIX = "DocDbSiteMap:";
        private const string KEY_SITE_MAP = "siteMap";

        public DocDbSiteMapRepository(Config.DocumentDBConfig dbConfig, IMappingEngine mapper, ICacheService cacheService)
        {
            _map = mapper;
            _cache = cacheService;
            _config = dbConfig;
            _docDbClient = _config.Client;
        }

        public async Task<SiteMap> GetMapAsync()
        {
            //try to get the map from cache
            var cachedMap = _cache.GetItem<SiteMap>(CACHE_KEY_PREFIX + KEY_SITE_MAP);

            if (cachedMap != null)
            {
                return cachedMap; 
            }
            if (_docDbClient == null)
            {
                //if we can't connect to the store, return null because we won't be able to save the map anyway
                return null;
            }

            //retrieve from docdb if not in cache
            var query = (from smd in _docDbClient.CreateDocumentQuery<SiteMapDocument>(_config.SiteContentCollectionUrl)
                        where smd.DocumentType == "SiteMap"
                        && smd.Id == KEY_SITE_MAP
                        select smd).AsDocumentQuery();
            var queryResult = await query.ExecuteNextAsync<SiteMapDocument>();
            SiteMapDocument siteMap = queryResult.FirstOrDefault();

            if(siteMap!= null)
            {
                //map to ui model
                SiteMap foundMap =  _map.Mapper.Map<SiteMap>(siteMap);

                //put in cache for next time
                _cache.PutItem<AzureKit.Models.SiteMap>(CACHE_KEY_PREFIX + KEY_SITE_MAP, foundMap);

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
           
            var siteMapDoc = _map.Mapper.Map<SiteMapDocument>(newMap);
            try {
                var updatedMap = await _docDbClient.UpsertDocumentAsync(_config.SiteContentCollectionUrl, siteMapDoc);
                
                _cache.PutItem<SiteMap>(CACHE_KEY_PREFIX + KEY_SITE_MAP, newMap);

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

        public async Task<bool> IsItemInSiteMapAsync(string contentIdentifier)
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