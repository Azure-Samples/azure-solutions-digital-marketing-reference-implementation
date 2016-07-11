using AzureKit.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureKit.Models;

namespace AzureKit.Tests.Data
{
    public class InMemorySiteMapRepository : ISiteMapRepository
    {
        public Task AddItemToSiteMapAsync(SiteMapEntry newEntry)
        {
            throw new NotImplementedException();
        }

        public Task<SiteMap> GetMapAsync()
        {
            return Task.FromResult(new SiteMap
            {
                Entries = new List<SiteMapEntry>{
                new SiteMapEntry {ContentIdentifier = "news", Title="News" },
                new SiteMapEntry {ContentIdentifier = "events", Title="Events" }
                }
            });
        }

        public Task<bool> IsItemInSiteMap(string contentIdentifier)
        {
            return Task.FromResult(true);
        }

        public Task RemoveItemFromSiteMapAsync(string entryToRemove)
        {
            return Task.CompletedTask;
        }
    }
}
