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
        private Mock<IUnitOfWork> moqIUnitOfWork;
        private Mock<IHttpContextAccessor> moqIHttpContextAccessor;
        private HomeController _controller;
        public HomeControllerTest()
        {
            moqIUnitOfWork = new Mock<IUnitOfWork>();
            moqIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _controller = new HomeController(moqIUnitOfWork.Object, moqIHttpContextAccessor.Object);
        }
        [Fact]
        public void CheckReturnViewForTestActionMethod()
        {
            Assert.IsType<ViewResult>(_controller.Test());
        }
        [Fact]
        public async Task CheckReturnnotFoundIfCategoriIdIsNullInNewsInCategory()
        {
            Assert.IsType<NotFoundResult>(await _controller.NewsInCategory(null, null));
        }
    }
}
