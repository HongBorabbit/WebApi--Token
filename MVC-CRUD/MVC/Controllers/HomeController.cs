using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC.Models;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        MvcDemoEntities MvcDemo = new MvcDemoEntities();
        public ActionResult Index()
        {
            // 一级菜单
            var list = MvcDemo.ProductType.Where(x=>x.tfin=="0").ToList();
            // 二级菜单
            ViewBag.list2 = MvcDemo.ProductType.Where(x => x.tfin != "0").ToList();
            //
            ViewBag.listCont = MvcDemo.ProductType.ToList();
            // select * from ProductType a
            // join ProductType c
            //on a.tid = c.tfin
            ViewBag.listC =(from x in MvcDemo.ProductType
                            join y in MvcDemo.ProductType
                            on x.tid equals y.tfin
                            select new { x.tid, x.tname}).ToList();
            return View(list);
        }
        /// <summary>
        /// 添加一级菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult AddFist()
        {
           
            return View();
        }
        /// <summary>
        /// 一级菜单功能
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public bool AddFist(ProductType productType)
        {
            
            if (productType != null)
            {
                MvcDemo.ProductType.Add(productType);
                int rs = MvcDemo.SaveChanges();
                if (rs>0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 子菜单视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Second()
        {
            // 一级菜单
            ViewBag.list2 = MvcDemo.ProductType.Where(x => x.tfin == "0").ToList();
            return View();
        }
        /// <summary>
        /// 子菜单添加功能
        /// </summary>
        /// <returns></returns>
        public string SecondAdd(ProductType productType)
        {
            // 二级菜单
            ViewBag.list2 = MvcDemo.ProductType.Where(x => x.tfin != "0").ToList();
            //
            if (productType!=null)
            {
                //var maxid = MvcDemo.ProductType.Max(x => x.tid).ToString();
                //int prodnumber =Convert.ToInt32(maxid) + 1;
                //productType.tid = prodnumber.ToString();
                MvcDemo.ProductType.Add(productType);
                MvcDemo.SaveChanges();
                return "ok";
            }
            return "erro";
        }
        /// <summary>
        /// 验证当前生成的菜单id是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string SecondVerifyNumber(string id)
        {
            var number = MvcDemo.ProductType.Where(x => x.tfin == id).ToList();
            if (number.Count>0)
            {
                return "erro";
            }
            return "ok";
        }

        public ActionResult EditEditSecond(string id)
        {
            var number = MvcDemo.ProductType.Where(x => x.tid == id).ToList();
            return View(number);
        }
        /// <summary>
        /// 类别id修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool EditSecond(ProductType productType)
        {
             var number = MvcDemo.ProductType.Where(x => x.tid == productType.tid).ToList();
             
             return false;
        } 

    }
}