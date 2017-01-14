using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureKit.Models;
using AzureKit.Areas.Manage.Controllers;
using System.Web.Mvc;
using System.Threading.Tasks;
using AzureKit.Tests.Controllers;

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
            s_repo.CreateContentAsync(
                new SimpleContent { Id = "page1", Title = "Page 1", Content = "<b>page1</b>" }
                ).Wait();
            
            s_mapRepo = new Data.InMemorySiteMapRepository();

        }
        [TestMethod]
        public void AddingSimpleContent()
        {
            //Assemble
            var controller = new ManageSimpleController(s_repo, s_mapRepo);
            SimpleContent content = new SimpleContent { Title = "Added", Content = "<b>Added</b>" };

            //Act
            var result = controller.Create(content).GetAwaiter().GetResult() as ViewResult;
            var items = s_repo.GetListOfItemsAsync(ContentType.Simple.ToString()).GetAwaiter().GetResult();
            var targetItem = items.Where((i) => i.Title == "Added").FirstOrDefault();

            //Assert
            Assert.AreEqual<string>("Confirm", result.ViewName, "Confirmation view not returned");
            Assert.IsNotNull(targetItem, "Item not found after adding");
        }

        [TestMethod]
        public async Task AttemptingToCreateSimpleContentWithIdAlreadyInUse()
        {
            await s_repo.CreateContentAsync(new SimpleContent { Id = "existing", Title = "Exists", Content = "Already here" });

            var toAdd = new SimpleContent { Id = "existing", Title = "Usurper", Content = "Trying to overwrite" };
            var controller = new ManageSimpleController(s_repo, s_mapRepo);
            var result = await controller.Create(toAdd) as ViewResult;

            Assert.AreEqual("Create", result.ViewName, "Shows Create view again");
            Assert.AreSame(toAdd, result.Model, "Passes model back to view");
            Assert.AreEqual("Content with this id already exists", controller.ModelState["Id"].Errors.Single().ErrorMessage);
        }

        [TestMethod]
        public void EditSimpleContent()
        {
            //Assemble
            var controller = new ManageSimpleController(s_repo, s_mapRepo);
            controller.ControllerContext = new ControllerContext(new FakeHttpContext(), new System.Web.Routing.RouteData(), controller);
            SimpleContent content = new SimpleContent { Id = "Edit", Title = "Edit", Content = "<b>Edit</b>" };
            s_repo.CreateContentAsync(content);
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
            s_repo.CreateContentAsync(content).GetAwaiter().GetResult();
                
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
            s_repo.CreateContentAsync(content).GetAwaiter().GetResult();

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
