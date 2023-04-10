using AutoMapper;
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
using System.Linq.Dynamic.Core;
using NewsWebsite.ViewModels.Models;

namespace NewsWebsite.Data.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly NewsDBContext _context;
        private readonly IMapper _mapper;
        public VideoRepository(NewsDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<VideoViewModel>> GetPaginateVideosAsync(PaginateModel model)
        {
            var startAndEndDate = ConvertDateTime.GetStartAndEndDateForSearch(model.searchText);
            var videos = await _context.Videos
                .Where(c => c.Title.Contains(model.searchText) || (c.PublishDateTime >= startAndEndDate.StartMiladiDate && c.PublishDateTime <= startAndEndDate.EndMiladiDate))
                .OrderBy(model.orderBy)
                .Skip(model.offset).Take(model.limit)
                .Select(c => _mapper.Map<VideoViewModel>(c)).AsNoTracking().ToListAsync();

            foreach (var item in videos)
                item.Row = ++model.offset;
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
