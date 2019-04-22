using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Helpers
{
    public class AuthorsResourceParameters
    {
        const int maxAuthorsPageSize = 20;

        public int PageNumber { get; set; } = 1;

        private int pageSize = 10;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > maxAuthorsPageSize ? maxAuthorsPageSize : value; }
        }

        public string Genre { get; set; }

        public string SearchQuery { get; set; }

        public string OrderBy { get; set; } = "Name";
    }
}
