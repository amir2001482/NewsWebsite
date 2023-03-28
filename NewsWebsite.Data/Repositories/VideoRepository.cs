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

namespace NewsWebsite.Data.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly NewsDBContext _context;
        private readonly IMapper _mapper;
        public VideoRepository(NewsDBContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<List<VideoViewModel>> GetPaginateVideosAsync(VideoPaginateModel model)
        {
            try
            {
                var obj = await _context.Videos.AsNoTracking()
               .Where(c => c.Title.Contains(model.searchText))
               .ToListAsync();

                var videos = _mapper.Map<List<VideoViewModel>>(obj)
                     .OrderBy(model.orderByAsc)
                     .OrderByDescending(model.orderByDes)
                     .Skip(model.offset).Take(model.limit).ToList();

                foreach (var item in videos)
                    item.Row = ++model.offset;

                return videos;
            }
            catch (Exception ex)
            {
                return new List<VideoViewModel>();
            }
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
