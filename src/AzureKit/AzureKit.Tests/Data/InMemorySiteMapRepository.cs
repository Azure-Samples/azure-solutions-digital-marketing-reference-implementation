using AzureKit.Data;
using AzureKit.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureKit.Tests.Data
{
    public class InMemorySiteMapRepository : ISiteMapRepository
    {
        private SiteMap _map;
        public InMemorySiteMapRepository()
        {
            _map = new SiteMap();
            _map.Entries = new List<SiteMapEntry>();
        }

        public Task AddItemToSiteMapAsync(SiteMapEntry newEntry)
        {
            _map.Entries.Add(newEntry);
            return Task.CompletedTask;
        }

        public Task<SiteMap> GetMapAsync()
        {
            return Task.FromResult<SiteMap>(_map);
        }

        public Task<bool> IsItemInSiteMapAsync(string contentIdentifier)
        {
            bool found = _map.Entries.Find(
                (e) => e.ContentIdentifier == contentIdentifier) != null;

            return Task.FromResult<bool>(found);
        }

        public Task RemoveItemFromSiteMapAsync(string entryToRemove)
        {
            _map.Entries.RemoveAll((sme) => sme.ContentIdentifier == entryToRemove);
            return Task.CompletedTask;
        }
    }
}
