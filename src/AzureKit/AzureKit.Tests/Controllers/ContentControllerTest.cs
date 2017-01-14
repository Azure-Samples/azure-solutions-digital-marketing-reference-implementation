using AzureKit.Controllers;
using AzureKit.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;

namespace AzureKit.Tests.Controllers
{
    [TestClass]
    public class ContentControllerTest
    {
        private static Data.InMemorySiteContentRepository _repo;
        private static Data.InMemorySiteMapRepository _mapRepo;
        
        
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            _repo = new Data.InMemorySiteContentRepository();
            _repo.CreateContentAsync(
                new SimpleContent { Id = "page1", Title = "Page 1", Content="<b>page1</b>"}
                ).Wait();
            _repo.CreateContentAsync(
                new ListLandingContent { Id = "page2", Title = "News", Content="<i>News</i> items" }
                ).Wait();
            _repo.CreateContentAsync(
                new ListItemContent { Id = "listItem1", Title = "News1", Content = "News item", ListLandingId = "page2" }).Wait();

            _mapRepo = new Data.InMemorySiteMapRepository();
            
        }

        [TestMethod]
        public void IndexWithSimpleContent()
        {
            ContentController controller = new ContentController(_repo, _mapRepo, null);
            var result = controller.Index("page1").Result as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual<string>("Simple", result.ViewName);

            //test the view model is populated correctly
            SimpleContent model = (SimpleContent)result.ViewData.Model;
            Assert.AreEqual<string>("<b>page1</b>", model.Content);
            Assert.AreEqual<ContentType>(ContentType.Simple, model.ContentType);
            Assert.AreEqual<string>("Page 1", model.Title);

        }

        [TestMethod]
        public void IndexWithListLandingContent()
        {
            ContentController controller = new ContentController(_repo, _mapRepo, null);
            var result = controller.Index("page2").Result as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual<string>("ListLanding", result.ViewName);

            //test the view model is populated correctly
            ListLandingContent model = (ListLandingContent)result.ViewData.Model;
            Assert.AreEqual<string>("<i>News</i> items", model.Content);
            Assert.AreEqual<ContentType>(ContentType.ListLanding, model.ContentType);
            Assert.AreEqual<string>("News", model.Title);

            //validate list items were loaded
            Assert.IsNotNull(model.Items, "No items found for list landing content");
            Assert.AreEqual<int>(1, model.Items.Count, "Unexpected number of items in list landing content.");
            Assert.AreEqual<string>("listItem1", model.Items[0].Id);
            Assert.AreEqual<ContentType>(ContentType.ListItem, model.Items[0].ContentType);

        }

        [TestMethod]
        public void IndexMissingContent()
        {
            ContentController controller = new ContentController(_repo, _mapRepo, null);
            var result = controller.Index("pageNone").Result as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual<string>("Missing", result.ViewName);
        }
    }
}
