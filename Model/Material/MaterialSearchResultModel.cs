using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Material
{
    public class MaterialSearchResultModel
    {
        public IEnumerable<MaterialInfoModel> Models { get; set; }
        public Page Page { get; set; }
    }
}
