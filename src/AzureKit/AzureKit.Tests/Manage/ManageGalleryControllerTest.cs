using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureKit.Areas.Manage.Controllers;
using AzureKit.Models;
using System.Web.Mvc;
using System.Linq;


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
            var content = new MediaGalleryContent { Id = "Added", Title = "Added", Content = "<b>Added</b>" };

            //Act
            var result = controller.Edit(content.Id, content).GetAwaiter().GetResult() as ViewResult;
            var items = s_repo.GetListOfItemsAsync(ContentType.MediaGallery.ToString()).GetAwaiter().GetResult();
            var targetItem = items.Where((i) => i.Title == "Added").FirstOrDefault();

            //Assert
            Assert.IsNotNull(targetItem, "Item not found after adding");
        }

        [TestMethod]
        public void EditGalleryReturnsNewModel()
        {
            //Assemble
            var controller = new ManageMediaGalleryController(s_repo, s_mapRepo, s_mediaRepo);
            
            //Act
            var result = controller.Edit(string.Empty).GetAwaiter().GetResult() as ViewResult;
            MediaGalleryContent targetItem = result.Model as MediaGalleryContent;

            //Assert
            Assert.IsNotNull(targetItem, "Model is not set for view on new edit");
        }

        [TestMethod]
        public void RemoveSimpleContent()
        {
            //assemble
            var content = new MediaGalleryContent { Id = "DeleteMe", Title = "To be deleted" };
            s_repo.SaveContentAsync<MediaGalleryContent>(content).GetAwaiter().GetResult();

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
