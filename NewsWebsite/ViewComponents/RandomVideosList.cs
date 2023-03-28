using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Common;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWebsite.ViewComponents
{
    public class RandomVideosList : ViewComponent
    {

        private readonly IUnitOfWork _uw;
        public RandomVideosList(IUnitOfWork uw)
        {
            _uw = uw;
        }

        public async Task<IViewComponentResult> InvokeAsync(int number)
        {
            var videoList = new List<VideoViewModel>();
            int randomRow;
            for (int i = 0; i < number; i++)
            {
                randomRow = CustomMethods.RandomNumber(1, _uw.BaseRepository<Video>().CountEntities()+1);
                var video = await _uw._Context.Videos.Select(n => new VideoViewModel { Title = n.Title, Url = n.Url, VideoId = n.VideoId, Poster = n.Poster }).Skip(randomRow - 1).Take(1).FirstOrDefaultAsync();
                videoList.Add(video);
            }

            return View(videoList);
        }
    }
}
