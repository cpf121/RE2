
using Business;
using Common;
using Model.Material;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace RoechlingEquipment.Controllers
{
    //[ServerAuthorize]
    public class MaterialController : BaseController
    {
        public ActionResult MaterialIndex()
        {
            return View();
        }

        /// <summary>
        /// 新增物料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddOrUpdate(MaterialInfoModel model)
        {
            var result = MaterialBusiness.SaveNewMaterial(model, this.LoginUser);
            return Json(result);
        }


        /// <summary>
        /// 获取物料
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        public ActionResult GetOneMaterial(int materialId)
        {
            var result = MaterialBusiness.GetMaterialById(materialId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询物料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Search(MaterialSearchModel model)
        {

            //return  IEnumerable<MaterialInfoModel>
            var totalCount = 0;
            var result = MaterialBusiness.SearchMaterialPageList(model, out totalCount);
            var page = new Page(totalCount, model.CurrentPage);

            //var resultModel = new MaterialSearchResultModel
            //{
            //    Models = result.Skip((page.CurrentPage - 1) * page.PageSize).Take(page.PageSize),
            //    Page = page
            //};
            var resultModel = new MaterialSearchResultModel
            {
                Models = result,
                Page = page
            };
            return View(resultModel);
        }
        [HttpPost]
        public ActionResult UpLoadFile(HttpPostedFileBase file)
        {
            StringBuilder strbuild = new StringBuilder();
            string FileName;
            string savePath;

            if (file == null || file.ContentLength <= 0)
            {
                ViewBag.error = "文件不能为空";
                return View();
            }
            else
            {
                string fileName = Path.GetFileName(file.FileName);
                int filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
                string fileEx = Path.GetExtension(fileName);//获取上传文件的扩展名
                string NoFileName = Path.GetFileNameWithoutExtension(fileName);//获取无扩展名的文件名
                int Maxsize = 4000 * 1024;//定义上传文件的最大空间大小为4M
                string FileType = ".xls,.xlsx";//定义上传文件的类型字符串

                FileName = NoFileName + fileEx;
                if (!FileType.Contains(fileEx))
                {
                    ViewBag.error = "文件类型不对，只能导入xls和xlsx格式的文件";
                    return View();
                }
                if (filesize >= Maxsize)
                {
                    ViewBag.error = "上传文件超过4M，不能上传";
                    return View();
                }
                string path = Server.MapPath("~/App_Data/uploads");
                savePath = Path.Combine(path, FileName);
                file.SaveAs(savePath);
            }

            string strConn;
            strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + savePath + ";Extended Properties=Excel 12.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            OleDbDataAdapter myCommand = new OleDbDataAdapter("select * from [Sheet1$]", strConn);
            DataSet myDataSet = new DataSet();
            try
            {
                myCommand.Fill(myDataSet, "ExcelInfo");
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }
            DataTable table = myDataSet.Tables["ExcelInfo"].DefaultView.ToTable();

            //引用事务机制，出错时，事物回滚
            using (TransactionScope transaction = new TransactionScope())
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {

                    MaterialInfoModel model = new MaterialInfoModel();
                    model.MICustomerPart = table.Rows[i][0].ToString();
                    model.MIProductName = table.Rows[i][1].ToString();
                    model.MICustomer = table.Rows[i][2].ToString();
                    model.MIPicture = table.Rows[i][3].ToString();
                    model.MIIsValid =Convert.ToInt32(table.Rows[i][4].ToString());
                    model.MICreateUserId = 33029;
                    model.MICreateUserName ="wq";
                    model.MICreateTime = DateTime.Now;
                    model.MIOperateUserId = 33029;
                    model.MIOperateUserName = "wq";
                    model.MIOperateTime = DateTime.Now;
                    model.MIWorkOrder = table.Rows[i][5].ToString();
                    model.MIMaterial = table.Rows[i][6].ToString();
                    model.MIMaterialText = table.Rows[i][7].ToString();
                    model.MITool = table.Rows[i][8].ToString();
                    model.MITotalQty= Convert.ToInt32(table.Rows[i][9].ToString()); 

                    //_stationRepository.AddStation(station);
                    //Todo Inter DB
                }
                transaction.Complete();
            }
            //ViewBag.error = "导入成功";
            System.Threading.Thread.Sleep(2000); 
            return RedirectToAction("MaterialIndex");
        }

        //public ActionResult Downloads()
        //{
        //    var dir = new System.IO.DirectoryInfo(Server.MapPath("~/App_Data/uploads/"));
        //    System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");
        //    List<string> items = new List<string>();

        //    foreach (var file in fileNames)
        //    {
        //        items.Add(file.Name);
        //    }

        //    return View(items);
        //}

    }
}
