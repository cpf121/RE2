using Common;
using DataAccess;
using Model.CommonModel;
using Model.Home;
using Model.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business
{
    public class MaterialBusiness
    {
        private static MaterialDAL _materialDal = new MaterialDAL();
        /// <summary>
        /// 描述:保存物料
        /// </summary>
        /// <param name="model"></param>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public static MaterialViewModel SaveNewMaterial(MaterialInfoModel model, UserLoginInfo loginUser)
        {
            var result = new MaterialViewModel() { IsSuccess = true };
            try
            {
                //add
                if (model.Id == 0)
                {
                    model.MIIsValid = 1;
                    model.MICreateUserId = Convert.ToInt32(loginUser.UserId);
                    model.MICreateUserName = loginUser.UserName;
                    model.MICreateTime = DateTime.Now;
                    model.MIOperateUserId = Convert.ToInt32(loginUser.UserId);
                    model.MIOperateUserName = loginUser.UserName;
                    model.MIOperateTime = DateTime.Now;
                    model.Id = _materialDal.Insert(model);
                    result.Message = EncryptHelper.DesEncrypt(model.Id.ToString()); //TODO
                    result.data = model;
                }
                else
                {
                    //Update
                    model.MIOperateUserId = Convert.ToInt32(loginUser.UserId);
                    model.MIOperateUserName = loginUser.UserName;
                    model.MIOperateTime = DateTime.Now;
                    _materialDal.Update(model);
                    result.Message = EncryptHelper.DesEncrypt(model.Id.ToString()); //TODO
                    result.data = model;
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
        /// 查询物料列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<MaterialInfoModel> SearchMaterialPageList(MaterialSearchModel param, out int totalCount)
        {
            var result = _materialDal.SearchMaterialPageList(param, out totalCount);
            return result;
        }


        /// <summary>
        /// 描述：获取物料信息
        /// </summary>
        /// <param name="materialId">物料Id</param>
        /// <returns></returns>
        public static MaterialInfoModel GetMaterialById(int materialId)
        {
            var result = _materialDal.GetMaterialById(materialId);
            return result;
        }
    }
}
