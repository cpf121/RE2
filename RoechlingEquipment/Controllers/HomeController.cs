
using Business;
using Common;
using Model.CommonModel;
using Model.Home;
using Model.TableModel;
using Model.ViewModel.Department;
using Model.ViewModel.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RoechlingEquipment.Controllers
{
    [ServerAuthorize] 
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            
            return View();
        }

        public ActionResult Search()
        {
            return View();
        }
        #region 后台设置

        public ActionResult AdSetIndex()
        {
            return View();
        }

        //TODO:CPF
        public ActionResult AdSetDepartments()
        {
            //获取根部门
            var rootDepartment = HomeBusiness.GetRootDepartment();
            ViewBag.RootDepartment = rootDepartment;

            var dpList = new List<SelectListItem>();
            var list = HomeBusiness.GetAllDepartmentName();
            if (list != null & list.Count > 0)
            {
                foreach (var dp in list)
                {
                    var item = new SelectListItem();
                    item.Text = dp.text;
                    item.Value = dp.id;
                    dpList.Add(item);
                }
            }
            ViewBag.dpList = dpList;
            return View(rootDepartment);
        }
        //TODO:CPF
        public ActionResult AdSetUsers()
        {
            return View();
        }

        /// <summary>
        /// 描述：获取组织框架信息
        /// </summary>
        /// <returns></returns>
        public ActionResult DepartmentManagerList()
        {
            var result = HomeBusiness.GetAllDepartmentList();
            return PartialView("DepartmentManagerList", result);
        }

        public ActionResult GetAllDepartment()
        {
            var list = HomeBusiness.GetAllDepartmentName();
            return Json(new { results = JsonHelper.Serializer<List<OrganizationSearch>>(list) },JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 描述：新建部门
        /// </summary>
        /// <param name="departmentId">部门Id</param>
        /// <param name="level">level为1表示新建平级部门</param>
        /// <returns></returns>
        public ActionResult DepartmentCreate(string departmentId,int level)
        {
            if (!string.IsNullOrEmpty(departmentId)) 
            {
                int dpId = Convert.ToInt32(EncryptHelper.DesDecrypt(departmentId));
                DepartmentInfo model;
                if (level == 1)   //1表示新建平级
                {
                    model = HomeBusiness.GetDepartById(dpId);
                }
                else
                {
                    model = HomeBusiness.GetDepartById(dpId);
                    model.ParentId = model.Id;
                    model.ParentName = model.Name;
                }
                return View(model);
            }
            return View();
        }

        public ActionResult DepartmentSave(DepartmentSaveModel model)
        {
            model.ParentId = EncryptHelper.DesDecrypt(model.ParentId);
            ResultInfoModel result;
            if (!string.IsNullOrEmpty(model.DepartId))   //编辑·
            {
                model.DepartId = EncryptHelper.DesDecrypt(model.DepartId);
                result= HomeBusiness.SaveDepartment(model, this.LoginUser);//todo 
            }
            else //新增
            {
                result = HomeBusiness.SaveNewDepartment(model,this.LoginUser);
            }
            return Json(result);
        }

        public ActionResult DepartmentEdit(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int dpId = Convert.ToInt32(EncryptHelper.DesDecrypt(id));
                var model = HomeBusiness.GetDepartById(dpId);
                return View(model);
            }
            return View(new DepartmentInfo());
        }

        public ActionResult UpdateDepartmentValid(string Id)
        {
            var result = new ResultInfoModel();
            if (!string.IsNullOrEmpty(Id))
            {
                result = HomeBusiness.UpdateDepartmentValid(EncryptHelper.DesDecrypt(Id),this.LoginUser);
            }
            return Json(result);
        }

        public ActionResult AdUser(string userId="")
        {
            var dpList = new List<SelectListItem>();
            var list = HomeBusiness.GetAllDepartmentName();
            if (list != null & list.Count > 0)
            {
                foreach (var dp in list)
                {
                    var item = new SelectListItem();
                    item.Text = dp.text;
                    item.Value = dp.id;
                    dpList.Add(item);
                }
            }
            ViewBag.dpList = dpList;
            ViewBag.userId = userId;
            return View();
        }

        /// <summary>
        /// 描述：初始化用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult InitAdUser(string userId)
        {
            long userIdlong = long.Parse(EncryptHelper.DesDecrypt(userId));
            var userInfo = HomeBusiness.GetUserById(userIdlong);
            var userView = new UserView();
            if (userInfo != null)
            {
                userView.UserId = EncryptHelper.DesEncrypt(userInfo.Id.ToString());
                userView.BUSurname = userInfo.BUSurname;
                userView.BUGivenname = userInfo.BUGivenname; 
                userView.BUJobNumber = userInfo.BUJobNumber;
                userView.BUSex = userInfo.BUSex;
                userView.BUAvatars = userInfo.BUAvatars;
                userView.BUPhoneNum = userInfo.BUPhoneNum;
                userView.BUEmail = userInfo.BUEmail;
                userView.DepartId = EncryptHelper.DesEncrypt(userInfo.BUDepartId.ToString());
                userView.BUTitle = userInfo.BUTitle;
                userView.BUIsValid = userInfo.BUIsValid;
            }
            return Json(userView, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UsrSave(UserView model)
        {
            var result = new ResultInfoModel();
            try
            {
                if (string.IsNullOrEmpty(model.UserId))
                {
                    //新增
                    var user = new User();
                    user.BUSurname = model.BUSurname;
                    user.BUGivenname = model.BUGivenname;
                    user.BUJobNumber = model.BUJobNumber;
                    user.BUSex = model.BUSex;
                    user.BUAvatars = model.BUAvatars;
                    user.BUPhoneNum = model.BUPhoneNum;
                    user.BUEmail = model.BUEmail;
                    user.BUDepartId = int.Parse(EncryptHelper.DesDecrypt(model.DepartId));
                    user.BUTitle = model.BUTitle;
                    user.BUIsValid = model.BUIsValid;
                    user.BUCreateUserId = int.Parse(LoginUser.UserId.ToString());
                    user.BUCreateUserName = LoginUser.UserName;
                    user.BUCreateTime = DateTime.Now;
                    user.BUOperateUserId = int.Parse(LoginUser.UserId.ToString());
                    user.BUOperateUserName = LoginUser.UserName;
                    user.BUOperateTime = DateTime.Now;
                    result = HomeBusiness.SaveNewUser(user);
                }
                else
                {
                    //解密id
                }
            }
            catch (Exception ex)
            {
            }
            return Json(new { });
        }
        #endregion
    }
}
