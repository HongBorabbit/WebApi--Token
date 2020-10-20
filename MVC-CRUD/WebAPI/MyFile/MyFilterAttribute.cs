using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http; //引入命名空间
using System.Web.Http.Controllers;
using System.Threading;
using System.Security.Principal;
using WebAPI.Models;

namespace WebAPI.MyFile
{
    public class MyFilterAttribute: AuthorizeAttribute
    {
        MvcDemoEntities2 mvcdemo = new MvcDemoEntities2();
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            //1、获取授权对象（请求头部中包含得Authrization对象）
            var auth = actionContext.Request.Headers.Authorization;
            if (auth != null)
            {
                //不为空
                if (auth.Scheme.ToLower() == "basic" && auth.Parameter != "")
                {
                    //认证的方式位basic认证，并且参数值不为空
                    //可以来进行验证你的令牌或者票据是否是正确的，如果正确返回true
                    //否则false
                    string token = auth.Parameter;
                    return CheckToken(token);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //为空 授权失败
                return false;
            }

        }

        public bool CheckToken(string token)
        {
            var rstoken = mvcdemo.Token.FirstOrDefault(x => x.token_text == token);
            if (rstoken != null)
            {
                //不为空，意味着查询到，但是不知道是否过期
                if (rstoken.token_time > DateTime.Now)
                {
                    //验证是否过了有效期 在有效期内
                    //还可以来进一步设置一个传递一个身份的id
                    //利用线程中的身份表示来存储用户信息（id）
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(rstoken.usid.ToString()), null);
                    // 使用线程来获取当前用户id
                    //  Thread.CurrentPrincipal.Identity.Name;
                    return true;
                }
                else
                {

                    return false;
                }
            }
            else
            {
                //token错误
                return false;
            }

        }
    }
}