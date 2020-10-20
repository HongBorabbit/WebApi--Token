using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class ProductTypeController : ApiController
    {
        // Entities
        MvcDemoEntities2 mvcdemo = new MvcDemoEntities2();
        // GET: api/ProductType
        public IEnumerable<string> Get()
        {

            return new string[] { "value1", "value2" };
        }
        [HttpGet]
        public List<ProductType> AllList()
        {
            // return mvcapi.ProductType.ToList();
            return null;
        }
        // GET: api/ProductType/5
        public string Get(int id)
        {

            return "value";
        }

        // POST: api/ProductType

        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ProductType/5
        [HttpPost]
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE: api/ProductType/5
        [HttpPost]
        public string Delete(int id)
        {

            return "删除";
        }
    }
}
