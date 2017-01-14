using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureKit.Models;
using AzureKit.Areas.Manage.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AzureKit.Tests.Controllers;

namespace AzureKit.Tests.Manage
{
    [TestClass]
    public class ManageListItemControllerTest
    {
        private static Data.InMemorySiteContentRepository s_repo;
        private static Data.InMemorySiteMapRepository s_mapRepo;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            s_repo = new Data.InMemorySiteContentRepository();
            s_repo.CreateContentAsync(
                new ListLandingContent { Id = "NewsList", Title = "News", Content = "<b>News</b>" }
                ).Wait();
            s_repo.CreateContentAsync(
                new ListItemContent { Id = "NewsItem",
                    Title = "News item",
                    ListLandingId = "NewsList",
                    Content = "News content" });

            s_mapRepo = new Data.InMemorySiteMapRepository();

        }
        [TestMethod]
        public void IndexReturnsExistingListsWhenNoneSpecified()
        {
            //Assemble
            var controller = new ManageListItemController(s_repo, s_mapRepo);

            //Act
            var result = controller.Index(String.Empty).GetAwaiter().GetResult() as ViewResult;
            var model = result.Model as List<ContentItemDescriptor>;
            var foundList = model.Where((l) => l.Id == "NewsList").FirstOrDefault();

            //Assert
            Assert.IsNotNull(model, "No lists returned");
            Assert.IsNotNull(foundList, "Existing list not found");
        }

        public void IndexReturnsListItems()
        {
            //Assemble
            var controller = new ManageListItemController(s_repo, s_mapRepo);

            //Act
            var result = controller.Index("NewsList").GetAwaiter().GetResult() as ViewResult;
            var model = result.Model as List<ContentItemDescriptor>;
            var foundItem = model.Where((l) => l.Id == "NewsItem").FirstOrDefault();
            
            //Assert
            Assert.AreEqual<string>("List", result.ViewName);
            Assert.IsNotNull(model, "No list items returned");
            Assert.IsNotNull(foundItem, "Existing item not found");
        }

        [TestMethod]
        public void AddingListItemContent()
        {
            //Assemble
            var controller = new ManageListItemController(s_repo, s_mapRepo);
            var content = new ListItemContent { ListLandingId="NewsList", Title = "Added", Content = "<b>Added</b>" };

            //Act
            var result = controller.Create(content).GetAwaiter().GetResult() as ViewResult;
            var items = s_repo.GetListOfItemsAsync(ContentType.ListItem.ToString()).GetAwaiter().GetResult();
            var targetItem = items.Where((i) => i.Title == "Added").FirstOrDefault();

            //Assert
            Assert.AreEqual<string>("Confirm", result.ViewName, "Confirmation view not returned");
            Assert.IsNotNull(targetItem, "Item not found after adding");
        }

        [TestMethod]
        public void EditListItemContent()
        {
            //Assemble
            var controller = new ManageListItemController(s_repo, s_mapRepo);
            controller.ControllerContext = new ControllerContext(new FakeHttpContext(), new System.Web.Routing.RouteData(), controller);
            var content = new ListItemContent { Id = "Edit", ListLandingId="NewsList", Title = "Edit", Content = "<b>Edit</b>" };
            s_repo.CreateContentAsync(content);
            content.Title = "Edited";
            content.Content = "<i>Edited</i>";

            //Act
            var result = controller.Edit(content.Id, content).GetAwaiter().GetResult() as ViewResult;
            var items = s_repo.GetListOfItemsAsync(ContentType.ListItem.ToString()).GetAwaiter().GetResult();
            var targetItem = items.Where((i) => i.Id == "Edit").FirstOrDefault();

            //Assert
            Assert.AreEqual<string>("Confirm", result.ViewName, "Confirmation view not returned");
            Assert.IsNotNull(targetItem, "Item not found after editing");
            Assert.AreEqual<string>("Edited", targetItem.Title, "Title does not reflect edits");
        }

        [TestMethod]
        public void RemoveListLandingContent()
        {
            //assemble
            var content = new ListItemContent { Id = "DeleteMe", Title = "To be deleted" };
            s_repo.CreateContentAsync(content).GetAwaiter().GetResult();

            var controller = new ManageListItemController(s_repo, s_mapRepo);

            //act
            var result = controller.Delete("DeleteMe", content);
            var items = s_repo.GetListOfItemsAsync(ContentType.ListItem.ToString()).GetAwaiter().GetResult();
            var missingItem = items.Where((i) => i.Id == "DeleteMe").FirstOrDefault();

            //assert
            Assert.IsNull(missingItem, "Item should not be found in repository");
        }
    }
}
