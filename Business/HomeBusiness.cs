using System;
using Common.Enum;
using DataAccess;
using Model.Home;
using Common;
using Common.Costant;
using Model.TableModel;
using System.Collections.Generic;
using System.Linq;
using Model.ViewModel.Department;
using Model.CommonModel;
using System.Data.SqlClient;

namespace Business
{
    public class HomeBusiness
    {
        private static HomeDAL _homeDal = new HomeDAL();
        private static AccountDAL _accountDal = new AccountDAL();
        private static UserDAL _userdal = new UserDAL();
        private static DepartmentDAL _departmentDal = new DepartmentDAL();

        public static bool UserLogin(string loginName, string password, string isAdmin, out string workNo)
        {
            return _homeDal.UserLogin(loginName, password, isAdmin, out workNo);
        }

        public static UserLoginInfo Login(string account, string password)
        {
            var userInfo = new UserLoginInfo();
            //根据账号密码获取用户id
            //密码需要加密？？
            var accountInfo = _accountDal.GetAccountByAccount(account);
            if (accountInfo == null)
            {
                throw new Exception("您输入的账号不存在，请先注册。");
            }
            //验证密码是否正确
            //if (CommonHelper.DesDecrypt(accountInfo.BAPassword, CookieKey.COOKIE_KEY_USERPASS) != password)
            //{
            //    var str = CommonHelper.DesEncrypt("123456", CookieKey.COOKIE_KEY_USERPASS);
            //    throw new Exception("用户名或密码不正确");
            //}
            if (accountInfo.BAPassword != password)  //注册完成之后使用上面的代码解密密码
            {
                throw new Exception("用户名或密码不正确");
            }
            if (accountInfo.BAIsValid == EnabledEnum.UnEnabled.GetHashCode())
            {
                throw new Exception("该账户已无效");
            }
            var user = _userdal.GetUserById(accountInfo.BAUserId);
            if (user != null)
            {
                if (user.BUIsValid == EnabledEnum.UnEnabled.GetHashCode())
                {
                    throw new Exception("该用户已无效");
                }
                userInfo.UserId = accountInfo.BAUserId;
                userInfo.Avatars = user.BUAvatars;
                userInfo.PhoneNum = user.BUPhoneNum;
                userInfo.DepartId = user.BUDepartId;
                userInfo.AccountType = accountInfo.BAType;
                userInfo.Account = accountInfo.BAAccount;
                userInfo.UserName = user.BUSurname + user.BUGivenname;
                if (user.BUDepartId > 0)
                {
                    var departmentInfo = _departmentDal.GetDpById(user.BUDepartId);
                    if (departmentInfo != null)
                    {
                        userInfo.DepartName = departmentInfo.BDDeptName;
                    }
                }
            }
            return userInfo;
        }

        #region 部门相关
        /// <summary>
        /// 描述：获取根部门
        /// 创建标识：cpf
        /// 创建时间：2017-9-19 16:08:35
        /// </summary>
        /// <returns></returns>
        public static DepartView GetRootDepartment()
        {
            var dpView = new DepartView();
            var depar= _departmentDal.GetRootDepartment();
            if (depar != null)
            {
                dpView.DesId = EncryptHelper.DesEncrypt(depar.Id.ToString());
                dpView.BDDeptName = depar.BDDeptName;
                dpView.DesParentId = EncryptHelper.DesEncrypt(depar.BDParentId.ToString());
            }
            return dpView;
        }

        /// <summary>
        /// 描述：获取组织架构信息
        /// 创建标识：cpf
        /// 创建时间：2017-9-19 20:31:07
        /// </summary>
        /// <returns></returns>
        public static OrganizationEntity GetAllDepartmentList()
        {
            var entity = new OrganizationEntity();

            var deplist = _departmentDal.GetAllDepartMent(); //所有的部门列表（有效无效的都包含）
            var firstdepart = deplist.FirstOrDefault(x => x.BDParentId == 0 & x.BDIsValid == EnabledEnum.Enabled.GetHashCode());
            var userList = _userdal.GetAllIsValidUser(); //所有有效的人员信息
            if (deplist.Count > 0 && userList.Count > 0)  //获取有效的部门梯队
            {
                entity = GetOrganizationTree(new OrganizationEntity(), userList, deplist, firstdepart.Id);
                entity.DepartmentId = EncryptHelper.DesEncrypt(firstdepart.Id.ToString());
                entity.DepartmentIsValid = (byte)firstdepart.BDIsValid;
                entity.DepartmentName = firstdepart.BDDeptName;
                entity.DepartmentUserNum = userList.Count;
                entity.ParentId = EncryptHelper.DesEncrypt("0");
            }
            var unValidList = deplist.Where(x => x.BDIsValid == EnabledEnum.UnEnabled.GetHashCode()).ToList();
            if (entity.DepartmentChildList == null)
            {
                entity.DepartmentChildList = new List<OrganizationEntity>();
            }
            if (firstdepart != null && unValidList != null)
            {
                var unValidEntity = new List<OrganizationEntity>();
                if (unValidList.Count > 0)
                {
                    unValidEntity.AddRange(unValidList.Select(item => new OrganizationEntity()
                    {
                        DepartmentId = EncryptHelper.DesEncrypt(item.Id.ToString()),
                        DepartmentIsValid = (byte)item.BDIsValid,
                        DepartmentName = item.BDDeptName,
                        DepartmentUserNum = 0,
                        ParentId = EncryptHelper.DesEncrypt("-1")
                    }));
                }
                entity.DepartmentChildList.Add(new OrganizationEntity
                {
                    ParentId = EncryptHelper.DesEncrypt(firstdepart.Id.ToString()),
                    DepartmentId = EncryptHelper.DesEncrypt("-1"), //避免无效部门id和有效部门id冲突
                    DepartmentName = "无效部门",
                    DepartmentUserNum = 0,
                    DepartmentChildList = unValidEntity
                });
            }
            
            return entity;
        }

        /// <summary>
        /// 描述：递归获取组织架构树
        /// 创建标识：cpf
        /// 创建时间：2017-9-20 10:56:01
        /// </summary>
        /// <param name="organization">组织架构树</param>
        /// <param name="userlist">所有的有效用户</param>
        /// <param name="departList">所有的部门</param>
        /// <param name="parentId">上级部门Id</param>
        /// <returns></returns>
        private static OrganizationEntity GetOrganizationTree(OrganizationEntity organization, List<User> userlist, List<Department> departList, long parentId)
        {
            var departmentinterim = departList.Where(x => x.BDParentId == parentId & x.BDIsValid==EnabledEnum.Enabled.GetHashCode()).ToList();

            if (departmentinterim.Any())
            {
                foreach (var item in departmentinterim)
                {
                    var userCount = userlist.Where(x=>x.BUDepartId==item.Id).ToList().Count;
                    var temOrganizationEntity = new OrganizationEntity
                    {
                        DepartmentId=EncryptHelper.DesEncrypt(item.Id.ToString()),
                        ParentId= EncryptHelper.DesEncrypt(parentId.ToString()),
                        DepartmentName=item.BDDeptName,
                        DepartmentUserNum=userCount,
                        DepartmentIsValid=(byte)item.BDIsValid
                    };
                    if (organization.DepartmentChildList == null)
                    {
                        organization.DepartmentChildList = new List<OrganizationEntity>();
                    }
                    organization.DepartmentChildList.Add(temOrganizationEntity);
                    GetOrganizationTree(temOrganizationEntity, userlist, departList, item.Id);
                }
            }
            return organization;
        }

        /// <summary>
        /// 描述：获取所有的有效部门的下拉数据
        /// 创建标识：cpf
        /// 创建时间：13点38分
        /// </summary>
        /// <returns></returns>
        public static List<OrganizationSearch> GetAllDepartmentName()
        {
            var list = new List<OrganizationSearch>();
            var deplist = _departmentDal.GetAllDepartMent();
            if (deplist != null && deplist.Count > 0)
            {
                deplist = deplist.Where(x => x.BDIsValid == EnabledEnum.Enabled.GetHashCode()).ToList();
                foreach (var item in deplist)
                {
                    var search = new OrganizationSearch();
                    search.id = EncryptHelper.DesEncrypt(item.Id.ToString());
                    search.text = item.BDDeptName;
                    list.Add(search);
                }
            }
            return list;
        }

        /// <summary>
        /// 描述：根据id获取部门信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DepartmentInfo GetDepartById(int id)
        {
            var departmetInfo = _departmentDal.GetDepartById(id);
            if (departmetInfo != null)
            {
                departmetInfo.Id= EncryptHelper.DesEncrypt(departmetInfo.Id);
                departmetInfo.ParentId = EncryptHelper.DesEncrypt(departmetInfo.ParentId);
            }
            return departmetInfo;
        }

        /// <summary>
        /// 描述:保存新部门
        /// 创建标识：cpf
        /// 创建时间：2017-9-21 21:56:37
        /// </summary>
        /// <param name="model"></param>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public static ResultInfoModel SaveNewDepartment(DepartmentSaveModel model, UserLoginInfo loginUser)
        {
            var result = new ResultInfoModel() { IsSuccess = true };
            var flag = CheckDepartment(model.DepartName);
            try
            {
                if (!flag)
                {
                    var entity = new Department()
                    {
                        BDParentId = long.Parse(model.ParentId),
                        BDDeptName = model.DepartName,
                        BDDeptDescription = model.DepartDesc,
                        BDIsValid = model.IsValid,
                        BDCreateUserId = Convert.ToInt32(loginUser.UserId),
                        BDCreateUserName = loginUser.UserName,
                        BDCreateTime = DateTime.Now,
                        BDOperateUserId = Convert.ToInt32(loginUser.UserId),
                        BDOperateUserName = loginUser.UserName,
                        BDOperateTime = DateTime.Now
                    };
                    var ids=_departmentDal.Insert(entity);
                    result.Code = EncryptHelper.DesEncrypt(ids.ToString()); 
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "已存在相同名称的部门";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public static ResultInfoModel SaveDepartment(DepartmentSaveModel model, UserLoginInfo loginUser)
        {
            var result = new ResultInfoModel() { IsSuccess = true };
            var flag = CheckDepartment(model.DepartName);
            try
            {
                if (!flag)
                {
                    var entity = new Department()
                    {
                        Id=long.Parse(model.DepartId),
                        BDParentId = long.Parse(model.ParentId),
                        BDDeptName = model.DepartName,
                        BDDeptDescription = model.DepartDesc,
                        BDIsValid = model.IsValid,
                        BDOperateUserId = Convert.ToInt32(loginUser.UserId),
                        BDOperateUserName = loginUser.UserName,
                        BDOperateTime = DateTime.Now
                    };
                    _departmentDal.Update(entity);                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "已存在相同名称的部门";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 描述：根据部门名称判断是否有相同的部门true表示已经重名
        /// 创建标识：cpf
        /// 创建时间：2017-9-21 21:56:54
        /// </summary>
        /// <param name="departName"></param>
        /// <returns></returns>
        private static bool CheckDepartment(string departName)
        {
            return _departmentDal.CheckDepartment(departName);
        }

        /// <summary>
        /// 描述：启用或停用部门
        /// 创建标识：cpf
        /// 创建时间：2017-9-24 18:17:48
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="loginuser"></param>
        /// <returns></returns>
        public static ResultInfoModel UpdateDepartmentValid(string Id,UserLoginInfo loginuser)
        {
            var result = new ResultInfoModel();
            var model = _departmentDal.GetDpById(Convert.ToInt32(Id));
            if (model != null)
            {
                if (model.BDIsValid == EnabledEnum.Enabled.GetHashCode())
                {
                    var deptlist = _departmentDal.GetDepartmentByParentId(long.Parse(Id));
                    if (deptlist != null && deptlist.Count > 0)
                    {
                        result.IsSuccess = false;
                        result.Message = "该部门下存在有效的子部门，不能停用!";
                        return result;
                    }
                    var userList = _userdal.GetUserListByDeptId(long.Parse(Id));
                    if (userList != null && userList.Count > 0)
                    {
                        result.IsSuccess = false;
                        result.Message = "该部门下存在在职员工，不能停用!";
                        return result;
                    }
                    model.BDIsValid = EnabledEnum.UnEnabled.GetHashCode();
                    model.BDOperateUserId = int.Parse(loginuser.UserId.ToString());
                    model.BDOperateUserName = loginuser.UserName;
                    model.BDOperateTime = DateTime.Now;
                    result.IsSuccess = _departmentDal.UpdateIsValid(model);
                }
                else
                {
                    var dpInfo = _departmentDal.GetDepartById(Convert.ToInt32(Id));
                    if (!CheckDepartment(dpInfo.Name))
                    {
                        var parentDept = _departmentDal.GetDpById(int.Parse(model.BDParentId.ToString()));
                        if (parentDept != null && parentDept.BDIsValid == EnabledEnum.UnEnabled.GetHashCode())
                        {
                            result.IsSuccess = false;
                            result.Message = "上级部门已停用，故不能启用";
                            return result;
                        }
                        model.BDIsValid = EnabledEnum.Enabled.GetHashCode();
                        model.BDOperateUserId = int.Parse(loginuser.UserId.ToString());
                        model.BDOperateUserName = loginuser.UserName;
                        model.BDOperateTime = DateTime.Now;
                        result.IsSuccess = _departmentDal.UpdateIsValid(model);
                        result.Code = EncryptHelper.DesEncrypt(model.BDParentId.ToString());
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "部门名" + dpInfo.Name + "已存在无法启用!";
                        return result;
                    }
                }
                result.Message = "编辑成功";
            }
            return result;
        }
        #endregion

        #region 用户相关
        public static User GetUserById(long userId)
        {
            var userinfo = _userdal.GetUserById(Convert.ToInt32(userId));
            return userinfo;
        }

        public static ResultInfoModel SaveNewUser(User model)
        {
            var result = new ResultInfoModel();
            if (IfHaveSamePhone(model.BUPhoneNum))
            {
                result.IsSuccess = false;
                result.Message = "已存在相同手机号的用户";
                return result;
            }
            if (string.IsNullOrEmpty(model.BUJobNumber) && IfHaveSameJobNumber(model.BUJobNumber))
            {
                result.IsSuccess = false;
                result.Message = "已存在相同工号的用户";
                return result;
            }
            var userId = string.Empty;
            try
            {
                using (var tran = SqlHelper.BeginTransaction())
                {
                    userId = EncryptHelper.DesEncrypt(_userdal.InsertUser(model, tran).ToString());
                    var account = new Account();
                    account.BAAccount = "默认账号";
                    account.BAAccount = "123456";
                }
            }
            catch (Exception ex)
            { }
            
            return result;
        }

        /// <summary>
        /// 描述：验证是否已存在手机号true表示有false表示没有
        /// 创建标识；cpf
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        public static bool IfHaveSamePhone(string phoneNum)
        {
            var dt = _userdal.GetUserByPhone(phoneNum);
            return dt != null && dt.Count > 0;
        }

        /// <summary>
        /// 描述：验证工号是否存在true表示有false表示没有
        /// 创建标识；cpf
        /// </summary>
        /// <param name="JobNumber"></param>
        /// <returns></returns>
        public static bool IfHaveSameJobNumber(string jobNumber)
        {
            var dt = _userdal.GetUserByJobNumber(jobNumber);
            return dt != null && dt.Count > 0;
        }
        #endregion
    }
}
