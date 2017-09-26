using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Material
{
    public class MaterialInfoModel
    {
        public long Id { get; set; }
        public string MICustomerPart { get; set; }
        public string MIProductName { get; set; }
        public string MICustomer { get; set; }
        public string MIPicture { get; set; }
        public int MIIsValid { get; set; }
        public long MICreateUserId { get; set; }
        public string MICreateUserName { get; set; }
        public DateTime MICreateTime { get; set; }
        public long MIOperateUserId { get; set; }
        public string MIOperateUserName { get; set; }
        public DateTime MIOperateTime { get; set; }
        public string MIWorkOrder { get; set; }
        public string MIMaterial { get; set; }
        public string MIMaterialText { get; set; }
        public string MITool { get; set; }
        public int MITotalQty { get; set; }

        public MaterialInfoModel()
        {
            if (Id == 0)
            {
                this.MICreateTime = this.MIOperateTime = DateTime.Now;
            }
            else
            {
                this.MIOperateTime = DateTime.Now;
            }
        }
    }
}
