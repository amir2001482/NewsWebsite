using Microsoft.EntityFrameworkCore;
using NewsWebsite.Common;
using NewsWebsite.Data.Contracts;
using NewsWebsite.ViewModels.Video;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsWebsite.Data.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly NewsDBContext _context;
        public VideoRepository(NewsDBContext context)
        {
            _context = context;
        }


        public async Task<List<VideoViewModel>> GetPaginateVideosAsync(int offset, int limit, bool? titleSortAsc, bool? publishDateTimeSortAsc, string searchText)
        {
            List<VideoViewModel> videos= await _context.Videos.Where(c => c.Title.Contains(searchText))
                                    .Select(c => new VideoViewModel { VideoId = c.VideoId, Title = c.Title, Url = c.Url, Poster=c.Poster,PersianPublishDateTime=c.PublishDateTime.ConvertMiladiToShamsi("yyyy/MM/dd ساعت hh:mm:ss")}).Skip(offset).Take(limit).AsNoTracking().ToListAsync();

            if (titleSortAsc != null)
            {
                videos = videos.OrderBy(c => (titleSortAsc == true && titleSortAsc != null) ? c.Title : "")
                                    .OrderByDescending(c => (titleSortAsc == false && titleSortAsc != null) ? c.Title : "").ToList();
            }

            else if (publishDateTimeSortAsc != null)
            {
                videos = videos.OrderBy(c => (publishDateTimeSortAsc == true && publishDateTimeSortAsc != null) ? c.Title : "")
                                   .OrderByDescending(c => (publishDateTimeSortAsc == false && publishDateTimeSortAsc != null) ? c.Title : "").ToList();
            }

            foreach (var item in videos)
                item.Row = ++offset;

            return videos;
        }

        public string CheckVideoFileName(string fileName)
        {
            string fileExtension = Path.GetExtension(fileName);
            int fileNameCount = _context.Videos.Where(f => f.Poster == fileName).Count();
            int j = 1;
            while (fileNameCount != 0)
            {
                fileName = fileName.Replace(fileExtension, "") + j + fileExtension;
                fileNameCount = _context.Videos.Where(f => f.Poster == fileName).Count();
                j++;
            }

            return fileName;
        }
    }
}
