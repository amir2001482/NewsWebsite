using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewsWebsite.Controllers;
using NewsWebsite.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NewsWebsite.XUnitTest.NewsWebsite.Web.Controller
{
    public class HomeControllerTest
    {
        [Fact]
        public void CheckReturnViewForTestActionMethod()
        {
            var moqIUnitOfWork = new Mock<IUnitOfWork>();
            var moqIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var controller = new HomeController(moqIUnitOfWork.Object, moqIHttpContextAccessor.Object);
            Assert.IsType<ViewResult>(controller.Test());
        }
        [Fact]
        public async Task CheckReturnnotFoundIfCategoriIdIsNullInNewsInCategory()
        {
            var moqIUnitOfWork = new Mock<IUnitOfWork>();
            var moqIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var controller = new HomeController(moqIUnitOfWork.Object, moqIHttpContextAccessor.Object);
            Assert.IsType<NotFoundResult>(await controller.NewsInCategory(null, null));
        }
    }
}
