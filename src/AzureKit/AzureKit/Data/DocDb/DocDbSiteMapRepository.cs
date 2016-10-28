using AutoMapper;
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
        private static DocumentClient _docDbClient;
        private static Config.IDocumentDBConfig _config;
        private const string CACHE_KEY_PREFIX = "DocDbSiteMap:";
        private const string KEY_SITE_MAP = "siteMap";

        public DocDbSiteMapRepository(Config.IDocumentDBConfig dbConfig, IMappingEngine mapper)
        {
            _map = mapper;
            _config = dbConfig;
            _docDbClient = _config.Client;
        }

        public async Task<SiteMap> GetMapAsync()
        {
            if (_docDbClient == null)
            {
                //if we can't connect to the store, return null because we won't be able to save the map anyway
                return null;
            }

            //retrieve from docdb 
            var query = (from smd in _docDbClient.CreateDocumentQuery<SiteMapDocument>(_config.SiteContentCollectionUrl)
                        where smd.DocumentType == "SiteMap"
                        && smd.Id == KEY_SITE_MAP
                        select smd).AsDocumentQuery();
            var queryResult = await query.ExecuteNextAsync<SiteMapDocument>().ConfigureAwait(false);
            SiteMapDocument siteMap = queryResult.FirstOrDefault();

            if(siteMap!= null)
            {
                //map to ui model
                SiteMap foundMap =  _map.Mapper.Map<SiteMap>(siteMap);

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
                await _docDbClient.UpsertDocumentAsync(_config.SiteContentCollectionUrl, siteMapDoc).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Unable to save site map ", ex);
            }
        }
        public async Task AddItemToSiteMapAsync(SiteMapEntry newEntry)
        {
            var map = await GetMapAsync().ConfigureAwait(false);
            if(map != null)
            {
                map.Entries.Add(newEntry);
                await SaveMapAsync(map).ConfigureAwait(false);

            }
        }

        public async Task RemoveItemFromSiteMapAsync(string entryToRemove)
        {
            var map = await GetMapAsync().ConfigureAwait(false);
            var entry = map.Entries.Where(e => e.ContentIdentifier == entryToRemove).Select(e => e).FirstOrDefault();
            if (entry != null)
            {
                map.Entries.Remove(entry);

                await SaveMapAsync(map).ConfigureAwait(false);
            }
        }

        public async Task<bool> IsItemInSiteMapAsync(string contentIdentifier)
        {
            var map = await GetMapAsync().ConfigureAwait(false);
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