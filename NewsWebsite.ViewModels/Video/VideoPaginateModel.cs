using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.ViewModels.Video
{
    public class VideoPaginateModel
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public Func<VideoViewModel, object> orderByAsc { get; set; }
        public Func<VideoViewModel, object> orderByDes { get; set; }
        public string searchText { get; set; }
    }
}
