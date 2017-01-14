using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureKit.Models;
using AzureKit.Areas.Manage.Controllers;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using AzureKit.Tests.Controllers;

namespace AzureKit.Tests.Manage
{
    [TestClass]
    public class ManageListControllerTest
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

            s_mapRepo = new Data.InMemorySiteMapRepository();

        }
        [TestMethod]
        public void IndexReturnsExistingLists()
        {
            //Assemble
            var controller = new ManageListController(s_repo, s_mapRepo);

            //Act
            var result = controller.Index().GetAwaiter().GetResult() as ViewResult;
            var model = result.Model as List<ContentItemDescriptor>;
            var foundList = model.Where((l) => l.Id == "NewsList").FirstOrDefault();

            //Assert
            Assert.IsNotNull(model, "No lists returned");
            Assert.IsNotNull(foundList, "Existing list not found");
        }

        [TestMethod]
        public void AddingListLandingContent()
        {
            //Assemble
            var controller = new ManageListController(s_repo, s_mapRepo);
            var content = new ListLandingContent { Title = "Added", Content = "<b>Added</b>" };

            //Act
            var result = controller.Create(content).GetAwaiter().GetResult() as ViewResult;
            var items = s_repo.GetListOfItemsAsync(ContentType.ListLanding.ToString()).GetAwaiter().GetResult();
            var targetItem = items.Where((i) => i.Title == "Added").FirstOrDefault();

            //Assert
            Assert.AreEqual<string>("Confirm", result.ViewName, "Confirmation view not returned");
            Assert.IsNotNull(targetItem, "Item not found after adding");
        }

        [TestMethod]
        public void EditListLandingContent()
        {
            //Assemble
            var controller = new ManageListController(s_repo, s_mapRepo);
            controller.ControllerContext = new ControllerContext(new FakeHttpContext(), new System.Web.Routing.RouteData(), controller);
            var content = new ListLandingContent { Id = "Edit", Title = "Edit", Content = "<b>Edit</b>" };
            s_repo.CreateContentAsync(content);
            content.Title = "Edited";
            content.Content = "<i>Edited</i>";

            //Act
            var result = controller.Edit(content.Id, content).GetAwaiter().GetResult() as ViewResult;
            var items = s_repo.GetListOfItemsAsync(ContentType.ListLanding.ToString()).GetAwaiter().GetResult();
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
            var content = new ListLandingContent { Id = "DeleteMe", Title = "To be deleted" };
            s_repo.CreateContentAsync(content).GetAwaiter().GetResult();

            var controller = new ManageListController(s_repo, s_mapRepo);

            //act
            var result = controller.Delete("DeleteMe", content);
            var items = s_repo.GetListOfItemsAsync(ContentType.ListLanding.ToString()).GetAwaiter().GetResult();
            var missingItem = items.Where((i) => i.Id == "DeleteMe").FirstOrDefault();

            //assert
            Assert.IsNull(missingItem, "Item should not be found in repository");
        }

        [TestMethod]
        public void EditListLandingReturnsItem()
        {
            //assemble
            var content = new ListLandingContent { Id = "EditMe", Title = "To be edited" };
            s_repo.CreateContentAsync(content).GetAwaiter().GetResult();

            var controller = new ManageListController(s_repo, s_mapRepo);

            //act
            var result = controller.Edit(content.Id).GetAwaiter().GetResult() as ViewResult;
            var model = result.Model as ListLandingContent;

            //assert
            Assert.IsNotNull(model, "Item should be set as the model from repository");
            Assert.AreEqual<string>(content.Id, model.Id);
            Assert.AreEqual<string>(content.Title, model.Title);

        }
    }
}
