﻿using NewsWebsite.ViewModels.Models;
using NewsWebsite.ViewModels.Video;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsWebsite.Data.Contracts
{
    public interface IVideoRepository
    {
        string CheckVideoFileName(string fileName);
        Task<List<VideoViewModel>> GetPaginateVideosAsync(PaginateModel model);
    }
}
