using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NewsWebsite.Areas.Admin.Controllers;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Tag;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NewsWebsite.XUnitTest.NewsWebsite.Web.Controller
{
    public class TagControllerTest
    {
        private Mock<IUnitOfWork> moqIUnitOfwork;
        private Mock<IMapper> moqIMapper;
        private TagController _controller;
        public TagControllerTest()
        {
            moqIUnitOfwork = new Mock<IUnitOfWork>();
            moqIMapper = new Mock<IMapper>();
            _controller = new TagController(moqIUnitOfwork.Object, moqIMapper.Object);
        }
        [Fact]
        public async Task notSaveDataWhenModelhasErrorForCreateOrUpdateAction()
        {
            _controller.ModelState.AddModelError("x", "test for error");
            await _controller.CreateOrUpdate(null);
            moqIUnitOfwork.Verify(x => x.BaseRepository<Tag>().CreateAsync(It.IsAny<Tag>()), Times.Never);
        }
        [Fact]
        public async Task SaveDataWhenModelDontErrorforCreatOrUpdateAction()
        {
            moqIUnitOfwork.Setup(x => x.BaseRepository<Tag>().CreateAsync(It.IsAny<Tag>()))
             .Returns(Task.CompletedTask);

            moqIUnitOfwork.Setup(x => x.Commit())
             .Returns(Task.CompletedTask);
            var tag = new TagViewModel { TagName = "test100" };
            moqIUnitOfwork.Setup(x => x.TagRepository.IsExistTag(tag.TagName, null)).Returns(false);
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            await _controller.CreateOrUpdate(tag);
            moqIUnitOfwork.Verify(x => x.BaseRepository<Tag>().CreateAsync(It.IsAny<Tag>()), Times.Once);

        }
    }
}
