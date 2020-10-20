using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using WebAPI.Models;
using System.Web.Hosting;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using System.Text;
using WebAPI.MyFile;
using System.Threading;
using System.Security.Cryptography;

namespace WebAPI.Controllers
{
    public class UserInfoController : ApiController
    {
        MvcDemoEntities2 mvcdemo = new MvcDemoEntities2();
        public string Options()
        {
            return "";
        }
        [Route("api/List")]
        public List<ProductInfo> GetList()
        {
            var productlist = mvcdemo.ProductInfo.ToList();
            return productlist;
        }
        #region 用户登录选项
        [Route("api/Userinfo/Registh")]
        [HttpPost]
        public IHttpActionResult UserResgin(UserInfo userInfo)
        {
            if (userInfo != null)
            {
                var strNew = Md(userInfo.upwd);
                userInfo.upwd = strNew;
                mvcdemo.UserInfo.Add(userInfo);
                mvcdemo.SaveChanges();
            }
            else
            {
                // 404
                return Content(HttpStatusCode.NotFound, "erro");
            }
            // 200
            return Content(HttpStatusCode.OK,"ok");
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [Route("api/Userinfo/login")]
        [HttpPost]
        public IHttpActionResult UserInfo(UserInfo userInfo)
        {
            // 1、返货md5加密后的密码
            var strNew = Md(userInfo.upwd);
            // 2、判断用户名和加密后的密码是否正确
            var useid = mvcdemo.UserInfo.FirstOrDefault(x=>x.uname==userInfo.uname && x.upwd== strNew).usid;
            var guid = "";
            Guid guid1 = Guid.NewGuid();
            guid = guid1.ToString().Replace("-", "");
            if (useid > 0)
            {
                 // 3、假设是第一次登录,Token表中没有令牌则生成第一次令牌
                var listtoken = mvcdemo.Token.FirstOrDefault(x => x.usid == useid);
                if (listtoken!=null)
                {
                    DeleteToken(useid);
                    SaveChang(useid, guid);
                }
                else
                {
                    SaveChang(useid, guid);
                }
            }
            return Ok(guid);  //返回一个token令牌到前台
        }
        /// <summary>
        /// 简单MD5加密密码方法
        /// </summary>
        /// <param name="strmd5">upwd参数</param>
        /// <returns>加密后的密码</returns>
        public string Md(string strmd5)
        {
            //创建md5
            MD5 md5 = MD5.Create();
            byte[] buffer = Encoding.GetEncoding("GBK").GetBytes(strmd5);
            byte[] md5buffer = md5.ComputeHash(buffer);
            string strNew = "";
            for (int i = 0; i < md5buffer.Length; i++)
            {
                strNew += md5buffer[i].ToString();
            }
            return strNew;
        }
        /// <summary>
        /// 添加token方法
        /// </summary>
        /// <param name="useid"></param>
        /// <param name="guid"></param>
        private void SaveChang(int useid, string guid)
        {
            mvcdemo.Token.Add(new Token
            {
                usid = useid,
                token_time = DateTime.Now.AddDays(1),
                token_text = guid,
                IsDel =true  //默认设置为true

            });
            mvcdemo.SaveChanges();
        }

        /// <summary>
        /// 如果登录成功删除之前的token
        /// </summary>
        /// <param name="useid"></param>
        public void DeleteToken(int useid)
        {
            var listtoken = mvcdemo.Token.FirstOrDefault(x => x.usid == useid);
            mvcdemo.Token.Remove(listtoken);
            mvcdemo.SaveChanges();
        }
        #endregion

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="productInfo"></param>
        /// <returns></returns>
        public IHttpActionResult PostAddList(ProductInfo productInfo)
        {
            if (productInfo != null)
            {
                string imagefile = "";
                mvcdemo.ProductInfo.Add(productInfo);
                int rs = mvcdemo.SaveChanges();
                if (rs > 0)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }
        #region 百度的方法能跑通 多图上传
        [HttpPost]
        public async Task<HttpResponseMessage> PostAddNew()
        {
            string fileimg = HttpContext.Current.Request["file"];
            // Check whether the POST operation is MultiPart?
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // Prepare CustomMultipartFormDataStreamProvider in which our multipart form
            // data will be loaded.
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/Upload/");
            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
            List<string> files = new List<string>();

            try
            {
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (MultipartFileData file in provider.FileData)
                {
                    files.Add(Path.GetFileName(file.LocalFileName));
                }

                // Send OK Response along with saved file names to the client.
                return Request.CreateResponse(HttpStatusCode.OK, files);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
        // We implement MultipartFormDataStreamProvider to override the filename of File which
        // will be stored on server, or else the default name will be of the format like Body-
        // Part_{GUID}. In the following implementation we simply get the FileName from 
        // ContentDisposition Header of the Request Body.
        public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            }
        }
        #endregion

        /// <summary>
        /// 多图上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Upload()
        {
            //1、多图上传
            HttpContext context = HttpContext.Current;
            HttpFileCollection files = context.Request.Files;
            //2、使用一个list集合存储得到的图片路径
            List<string> finame = new List<string>();
            if (files.Count!=0)
            {
            //string strPath = path + "upload/";
            string guid = "";

            for (int i = 0; i < files.Count; i++)
            {
                // 1、获取文件后缀
                string filename = files[i].FileName;
                FileInfo fileInfo = new FileInfo(filename);
                string ext = fileInfo.Extension;
                //1.1 防止图片名称重复
                guid = Guid.NewGuid().ToString() + ext;
                //2、替换guid -
                guid = guid.Replace("-", "");
                //3、保存图片到~/upload/
                string path = context.Request.MapPath("~/upload/");
                finame.Add(path + guid);
                 //finame = path + guid;
                files[i].SaveAs(path + guid);
            }

            }
            else
            {
                return NotFound();
            }

            return Json(finame); //返回所有保存成功后的文件
        }
        [Route("api/Css/cs")]
        [MyFilter]
        [HttpGet]
        public IHttpActionResult Cs()
        {
            var a = Thread.CurrentPrincipal.Identity.Name;
            return Ok("授权到了"+ a.ToString());
        }
    }
}

  
