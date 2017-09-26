using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.TableModel
{
    public class Account
    {
        public int Id { get; set; }

        public string BAAccount { get; set; }

        public string BAPassword { get; set; }

        public int BAUserId { get; set; }

        public int BAType { get; set; }

        public int BAIsValid { get; set; }

        public int BACreateUserId { get; set; }

        public string BACreateUserName { get; set; }

        public DateTime BACreateTime { get; set; }

        public int BAOperateUserId { get; set; }

        public string BAOperateUserName { get; set; }

        public DateTime BAOperateTime { get; set; }
    }
}
