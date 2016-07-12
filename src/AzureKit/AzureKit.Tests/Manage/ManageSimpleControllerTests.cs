using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureKit.Models;
using AzureKit.Areas.Manage.Controllers;
using System.Web.Mvc;

namespace AzureKit.Tests.Manage
{
    [TestClass]
    public class ManageSimpleControllerTests
    {
        private static Data.InMemorySiteContentRepository s_repo;
        private static Data.InMemorySiteMapRepository s_mapRepo;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            s_repo = new Data.InMemorySiteContentRepository();
            s_repo.SaveContentAsync(
                new SimpleContent { Id = "page1", Title = "Page 1", Content = "<b>page1</b>" }
                ).Wait();
            
            s_mapRepo = new Data.InMemorySiteMapRepository();

        }
        [TestMethod]
        public void AddingSimpleContent()
        {
            //Assemble
            var controller = new ManageSimpleController(s_repo, s_mapRepo);
            SimpleContent content = new SimpleContent {Id="Added", Title = "Added", Content = "<b>Added</b>" };

            //Act
            var result = controller.Create(content).GetAwaiter().GetResult() as ViewResult;
            var items = s_repo.GetListOfItemsAsync(ContentType.Simple.ToString()).GetAwaiter().GetResult();
            var targetItem = items.Where((i) => i.Title == "Added").FirstOrDefault();

            //Assert
            Assert.AreEqual<string>("Confirm", result.ViewName, "Confirmation view not returned");
            Assert.IsNotNull(targetItem, "Item not found after adding");
        }

        [TestMethod]
        public void EditSimpleContent()
        {
            //Assemble
            var controller = new ManageSimpleController(s_repo, s_mapRepo);
            SimpleContent content = new SimpleContent { Id = "Edit", Title = "Edit", Content = "<b>Edit</b>" };
            s_repo.SaveContentAsync<SimpleContent>(content);
            content.Title = "Edited";
            content.Content = "<i>Edited</i>";

            //Act
            var result = controller.Edit(content.Id, content).GetAwaiter().GetResult() as ViewResult;
            var items = s_repo.GetListOfItemsAsync(ContentType.Simple.ToString()).GetAwaiter().GetResult();
            var targetItem = items.Where((i) => i.Id == "Edit").FirstOrDefault();

            //Assert
            Assert.AreEqual<string>("Confirm", result.ViewName, "Confirmation view not returned");
            Assert.IsNotNull(targetItem, "Item not found after editing");
            Assert.AreEqual<string>("Edited", targetItem.Title, "Title does not reflect edits");
        }

        [TestMethod]
        public void RemoveSimpleContent()
        {
            //assemble
            var content = new SimpleContent { Id = "DeleteMe", Title = "To be deleted" };
            s_repo.SaveContentAsync<SimpleContent>(content).GetAwaiter().GetResult();
                
            var controller = new ManageSimpleController(s_repo, s_mapRepo);

            //act
            var result = controller.Delete("DeleteMe", content);
            var items = s_repo.GetListOfItemsAsync(ContentType.Simple.ToString()).GetAwaiter().GetResult();
            var missingItem = items.Where((i) => i.Id == "DeleteMe").FirstOrDefault();

            //assert
            Assert.IsNull(missingItem, "Item should not be found in repository");
        }

        [TestMethod]
        public void EditSimpleContentReturnsItem()
        {
            //assemble
            var content = new SimpleContent { Id = "EditMe", Title = "To be edited" };
            s_repo.SaveContentAsync<SimpleContent>(content).GetAwaiter().GetResult();

            var controller = new ManageSimpleController(s_repo, s_mapRepo);

            //act
            var result = controller.Edit(content.Id).GetAwaiter().GetResult() as ViewResult;
            var model = result.Model as SimpleContent;

            //assert
            Assert.IsNotNull(model, "Item should be set as the model from repository");
            Assert.AreEqual<string>(content.Id, model.Id);
            Assert.AreEqual<string>(content.Title, model.Title);

        }
    }
}
