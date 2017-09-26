using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Material
{
    public class MaterialSearchModel
    {
        private int _pageSize = 10;

        private int _currentPage = 1;

        public string CustomerPart { get; set; }

        public string ProductName { get; set; }

        public string WorkOrder { get; set; }

        public int CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = value;
            }
        }

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
            }
        }
    }
}
