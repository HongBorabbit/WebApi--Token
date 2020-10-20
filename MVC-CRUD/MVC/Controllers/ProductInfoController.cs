using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class ProductInfoController : Controller
    {
        // GET: ProductInfo
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 上传图片视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit()
        {
            return View();
        }
        /// <summary>
        /// 单图上传图片
        /// </summary>
        /// <returns></returns>
        public string NewSave()
        {
            // 设置一个返回值
            var imageurl = "";
            //2、获取文件对象
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files["File"];
            string FileName = file.FileName;
            string guid1 = "";
            //3、判断是否为空
            if (FileName!=null&& FileName!="")
            {
                //4、截取文件后缀
                string FileType = FileName.Substring(FileName.LastIndexOf(".") + 1); //得到文件的后缀名
                if (FileType.ToLower()=="jpg"|| FileType.ToLower() == "png"||FileType.ToLower()== "jfif")
                {
                    if (file.ContentLength< 5 * 1024 * 1024)
                    {
                    guid1 = System.Guid.NewGuid().ToString() + "." + FileType; //得到重命名的文件名
                    // 5、替换guid的-为空 并保存
                    var guid = guid1.Replace("-", "");
                    file.SaveAs(Server.MapPath("/Upload/") + guid); //保存操作
                    string Upload = "/Upload/" + guid;
                    imageurl = Upload;
                    }
                    else
                    {
                        return "文件大小超出限制!";
                    }
                }
                else
                {
                    return "请选择jpg png jfif后缀的图片！";
                }
            }
            else
            {
                return "上传失败！";
            }
            return imageurl; //成功返回保存后的文件路径
        }
        /// <summary>
        /// 多图上传
        /// </summary>
        /// <returns></returns>
        public bool NewSaves()
        {
            var guid = "";
            var list = Request.Files.GetMultiple("File");
            if (list[0].FileName!="" && list[0].FileName !=null)
            {
                foreach (var item in list)
                {
                    string filename = item.FileName;
                    // 1、获取文件后缀名
                    FileInfo fileInfo = new FileInfo(filename);
                    string ext = fileInfo.Extension;
                    // 2、修改文件名
                    guid = Guid.NewGuid().ToString()+ ext;
                    // 3、替换guid的-为空 并保存
                    var guid1 = guid.Replace("-", "");
                    var path = Server.MapPath("/Upload/");
                    item.SaveAs(path + guid1);
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        public bool NewAdd()
        {
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files["File"];
            return true;
        }
    }
}