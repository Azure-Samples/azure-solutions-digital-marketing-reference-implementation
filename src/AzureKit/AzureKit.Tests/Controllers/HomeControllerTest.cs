﻿using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureKit.Controllers;
using AzureKit.Tests.Data;
using AzureKit.Models;

namespace AzureKit.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        //wrapper class to allow for loading site map
        private class HomeControllerWrapper : HomeController
        {
            public HomeControllerWrapper(AzureKit.Data.ISiteMapRepository mapRepo, AzureKit.Data.ISiteContentRepository contentRepo) :base(mapRepo, contentRepo)
            {}
            public new void LoadSiteMap()
            {
                base.LoadSiteMap();
            }
        }
        static AzureKit.Data.ISiteMapRepository repo;
        static AzureKit.Data.ISiteContentRepository contentRepo;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            repo = new InMemorySiteMapRepository();
            contentRepo = new InMemorySiteContentRepository();
        }
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController(repo, contentRepo);
            
            // Act
            ViewResult result = controller.Index().GetAwaiter().GetResult() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            SiteMap map = result.ViewBag.SiteMap;

            //test that map is null, but we should not have any exceptions
            Assert.IsNull(map, "Site map is NOT null on home page.");
        }

        public void IndexWithSiteMap()
        {
            // Arrange
            HomeControllerWrapper controller = new HomeControllerWrapper(repo, contentRepo);
            //call wrapper method to invoke protected load from base
            controller.LoadSiteMap();
            // Act
            ViewResult result = controller.Index().GetAwaiter().GetResult() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            SiteMap map = result.ViewBag.SiteMap;

            //test that map is not null, and has the default entries
            Assert.IsNotNull(map, "Site map is null on home page.");
            Assert.AreEqual<int>(2, map.Entries.Count);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController(repo, contentRepo);

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController(repo, contentRepo);

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void SiteMapRenders()
        {
           
            // Arrange
            HomeController controller = new HomeController(repo, contentRepo);
            
            // Act
            ViewResult result = controller.Index().GetAwaiter().GetResult() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
