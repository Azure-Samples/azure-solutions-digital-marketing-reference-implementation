using AzureKit.Models;
using System.Threading.Tasks;

namespace AzureKit.Data
{
    public interface ISiteMapRepository
    {
        Task<SiteMap> GetMapAsync();
        Task AddItemToSiteMapAsync(SiteMapEntry newEntry);

        Task RemoveItemFromSiteMapAsync(string entryToRemove);

        Task<bool> IsItemInSiteMapAsync(string contentIdentifier);

    }
}
