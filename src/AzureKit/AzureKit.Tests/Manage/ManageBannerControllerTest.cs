using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureKit.Areas.Manage.Controllers;
using System.Web.Mvc;
using AzureKit.Models;
using AzureKit.Config;
using AzureKit.Tests.Controllers;

namespace AzureKit.Tests.Manage
{
    [TestClass]
    public class ManageBannerControllerTest
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
        public void GetBannerNoBanner()
        {
            //assemble
            var controller = new ManageBannerController(s_repo, s_mapRepo);

            //act
            var result = controller.Index().GetAwaiter().GetResult() as ViewResult;
            var model = result.Model;

            //Assert
            Assert.IsNotNull(model, "Model not set for missing banner");
            Assert.IsInstanceOfType(model, typeof(BannerContent), "Model is not a banner content model");
        }

        [TestMethod]
        public void GetBannerExistingBanner()
        {
            //assemble
            s_repo.CreateContentAsync(
                new BannerContent { Id = "tb1", Title = "Test Banner", Content = "Test Banner Content" });

            var controller = new ManageBannerController(s_repo, s_mapRepo);

            //act
            var result = controller.Index().GetAwaiter().GetResult() as ViewResult;
            var model = result.Model;

            //Assert
            Assert.IsNotNull(model, "Model not set for missing banner");
            Assert.IsInstanceOfType(model, typeof(BannerContent), "Model is not a banner content model");
            Assert.AreEqual<string>(Constants.KEY_BANNER_CONTENT, ((BannerContent)model).Id);

        }

        [TestMethod]
        public void SaveBanner()
        {
            //assemble
            var controller = new ManageBannerController(s_repo, s_mapRepo);
            controller.ControllerContext = new ControllerContext(new FakeHttpContext(), new System.Web.Routing.RouteData(), controller);
            var bannerContent = new BannerContent { Title = "New Banner", Content = "Banner Content" };
            //act
            var result = controller.Index(bannerContent).GetAwaiter().GetResult() as ViewResult;
            
            var bannerFromRepo = s_repo.GetContentAsync(Constants.KEY_BANNER_CONTENT).GetAwaiter().GetResult() as BannerContent;

            //Assert
            Assert.AreEqual<string>(result.ViewName, "Confirm");
            Assert.IsNotNull(bannerFromRepo, "Banner not found in repository");
            Assert.AreEqual<string>(bannerContent.Title, bannerFromRepo.Title);
            Assert.AreEqual<ContentType>(ContentType.Banner, bannerFromRepo.ContentType);
        }
    }
}
