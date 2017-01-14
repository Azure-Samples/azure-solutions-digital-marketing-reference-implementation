using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureKit.Models;
using AzureKit.Areas.Manage.Controllers;
using System.Web.Mvc;
using System.Linq;

namespace AzureKit.Tests.Manage
{
    [TestClass]
    public class ManageSiteMapTest
    {
        private static Data.InMemorySiteContentRepository s_repo;
        private static Data.InMemorySiteMapRepository s_mapRepo;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            s_repo = new Data.InMemorySiteContentRepository();
            s_mapRepo = new Data.InMemorySiteMapRepository();
        }

        [TestMethod]
        public void ItemAddsToSiteMap()
        {
            //Assemble
            var content = new SimpleContent { Id = "Mappable", Title = "MapMe", Content = "<b>mapped</b>" };
            s_repo.CreateContentAsync(content).GetAwaiter().GetResult();

            var controller = new ManageSimpleController(s_repo, s_mapRepo);

            //Act
            var result = controller.Add(content.Id, content.Title).GetAwaiter().GetResult() as ViewResult;
            var isEntryInMap = s_mapRepo.IsItemInSiteMapAsync(content.Id).GetAwaiter().GetResult();
            var foundEntry = s_mapRepo.GetMapAsync().GetAwaiter().GetResult()
                .Entries.Where(
                (e) => e.ContentIdentifier == content.Id)
                .FirstOrDefault();

            //Assert
            Assert.AreEqual<string>("SiteMapConfirm", result.ViewName, "Incorrect view returned");
            Assert.IsTrue(isEntryInMap, "Map entry not reported as in the site map");
            Assert.IsNotNull(foundEntry, "Entry not found manually in map");
        }

        [TestMethod]
        public void ItemRemovesFromSiteMap()
        {
            //Assemble
            var content = new SimpleContent { Id = "Unmappable", Title = "DontMapMe", Content = "<b>not mapped</b>" };
            s_repo.CreateContentAsync(content).GetAwaiter().GetResult();
            s_mapRepo.AddItemToSiteMapAsync(
                new SiteMapEntry { ContentIdentifier = content.Id, Title = content.Title });

            var controller = new ManageSimpleController(s_repo, s_mapRepo);

            //Act
            var result = controller.Remove(content.Id).GetAwaiter().GetResult() as ViewResult;
            var isEntryInMap = s_mapRepo.IsItemInSiteMapAsync(content.Id).GetAwaiter().GetResult();
            var foundEntry = s_mapRepo.GetMapAsync().GetAwaiter().GetResult()
                .Entries.Where(
                (e) => e.ContentIdentifier == content.Id)
                .FirstOrDefault();

            //Assert
            Assert.AreEqual<string>("SiteMapConfirm", result.ViewName, "Incorrect view returned");
            Assert.IsFalse(isEntryInMap, "Map entry reported as in the site map");
            Assert.IsNull(foundEntry, "Entry found manually in map");
        }
    }
}
