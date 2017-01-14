using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureKit.Areas.Manage.Controllers;
using AzureKit.Models;
using System.Web.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AzureKit.Tests.Manage
{
    [TestClass]
    public class ManageGalleryControllerTest
    {
        private static Data.InMemorySiteContentRepository s_repo;
        private static Data.InMemorySiteMapRepository s_mapRepo;
        private static Media.InMemoryMediaStorage s_mediaRepo;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            s_repo = new Data.InMemorySiteContentRepository();
            s_mapRepo = new Data.InMemorySiteMapRepository();
            s_mediaRepo = new Media.InMemoryMediaStorage();

        }
        [TestMethod]
        public void AddingGalleryContent()
        {
            //Assemble
            var controller = new ManageMediaGalleryController(s_repo, s_mapRepo, s_mediaRepo);
            var content = new MediaGalleryContent { Title = "Added", Content = "<b>Added</b>" };

            //Act
            var result = controller.Create(content).GetAwaiter().GetResult() as ViewResult;
            var items = s_repo.GetListOfItemsAsync(ContentType.MediaGallery.ToString()).GetAwaiter().GetResult();
            var targetItem = items.Where((i) => i.Title == "Added").FirstOrDefault();

            //Assert
            Assert.IsNotNull(targetItem, "Item not found after adding");
        }

        [TestMethod]
        public async Task AttemptingToCreateGalleryContentWithIdAlreadyInUse()
        {
            await s_repo.CreateContentAsync(new MediaGalleryContent { Id = "existing", Title = "Exists", Content = "Already here" });

            var toAdd = new MediaGalleryContent { Id = "existing", Title = "Usurper", Content = "Trying to overwrite" };
            var controller = new ManageMediaGalleryController(s_repo, s_mapRepo, s_mediaRepo);
            var result = await controller.Create(toAdd) as ViewResult;

            Assert.AreEqual("", result.ViewName, "Shows same view again");
            Assert.AreSame(toAdd, result.Model, "Passes model back to view");
            Assert.AreEqual("Content with this id already exists", controller.ModelState["Id"].Errors.Single().ErrorMessage);
        }

        [TestMethod]
        public void GetCreateGalleryReturnsNewModel()
        {
            //Assemble
            var controller = new ManageMediaGalleryController(s_repo, s_mapRepo, s_mediaRepo);
            
            //Act
            var result = controller.Create() as ViewResult;
            MediaGalleryContent targetItem = result.Model as MediaGalleryContent;

            //Assert
            Assert.IsNotNull(targetItem, "Model is not set for view on new edit");
        }

        [TestMethod]
        public void RemoveSimpleContent()
        {
            //assemble
            var content = new MediaGalleryContent { Id = "DeleteMe", Title = "To be deleted" };
            s_repo.CreateContentAsync(content).GetAwaiter().GetResult();

            var controller = new ManageMediaGalleryController(s_repo, s_mapRepo, s_mediaRepo);

            //act
            var result = controller.Delete("DeleteMe", content);
            var items = s_repo.GetListOfItemsAsync(ContentType.MediaGallery.ToString()).GetAwaiter().GetResult();
            var missingItem = items.Where((i) => i.Id == "DeleteMe").FirstOrDefault();

            //assert
            Assert.IsNull(missingItem, "Item should not be found in repository");
        }
    }
}
