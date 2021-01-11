using System;
using System.Collections.Generic;
using System.Linq;
using H4WebApi.Models;
using H4WebApi.Models.BaseInfo;
using H4WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace H4WebApi.Controllers.BaseInfo
{
    [Route("api/driver")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly H4WebContext _context;

        public DriverController(H4WebContext context)
        {
            _context = context;
        }

        // 带多个查询条件的查询
        [Route("query")]
        [HttpGet]
        public ActionResult<TableData> Query(string queryStr)
        {
            JObject jObject = new JObject();
            if (string.IsNullOrEmpty(queryStr) == false)
            {
                jObject = JsonConvert.DeserializeObject<JObject>(queryStr);
            }

            int current = jObject.Value<int>("current") == 0 ? 1 : jObject.Value<int>("current");
            int pageSize = jObject.Value<int>("pageSize") == 0 ? 20 : jObject.Value<int>("pageSize");
            string name = jObject.Value<string>("name");
            string company = jObject.Value<string>("company");

            //防止查询条件都不满足，先生成一个空的查询
            var where = _context.Driver.Where(p => true);

            if (string.IsNullOrEmpty(name) == false)
            {
                where = where.Where(p => p.Name.Contains(name));
            }

            //统计总记录数
            int total = where.Count();

            // 解析排序规则
            string sorterKey = "";
            string sortRule = "";
            JObject sorterObj = jObject.Value<JObject>("sorter");
            IEnumerable<JProperty> properties = sorterObj.Properties();
            foreach (JProperty item in properties)
            {
                sorterKey = item.Name;
                sortRule = item.Value.ToString();
            }
            if (string.IsNullOrEmpty(sorterKey) == false && string.IsNullOrEmpty(sortRule) == false)
            {
                if (sorterKey.Equals("name") && sortRule.Equals("descend"))
                {
                    where = where.OrderByDescending(p => p.Name);
                }
                else if (sorterKey.Equals("name") && sortRule.Equals("ascend"))
                {
                    where = where.OrderBy(p => p.Name);
                }
                // 按照最后更新时间排序
                if (sorterKey.Equals("lastUpdateTime") && sortRule.Equals("descend"))
                {
                    where = where.OrderByDescending(p => p.LastUpdateTime);
                }
                else if (sorterKey.Equals("lastUpdateTime") && sortRule.Equals("ascend"))
                {
                    where = where.OrderBy(p => p.LastUpdateTime);
                }
            }
            else
            {
                //结果按照最后修改时间倒序排序
                where = where.OrderByDescending(p => p.LastUpdateTime);
            }

            //跳过翻页的数量
            where = where.Skip(pageSize * (current - 1));
            //获取结果
            List<Driver> dataList = where.Take(pageSize).ToList();

            TableData resultObj = new TableData();
            resultObj.Data = dataList;
            resultObj.Current = current;
            resultObj.Success = true;
            resultObj.PageSize = pageSize;
            resultObj.Total = total;

            return resultObj;
        }

        // 根据id查询
        [Route("getbyid")]
        [HttpGet]
        public ActionResult<Driver> GetById(Guid gid)
        {
            var item = _context.Driver.Find(gid);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        // 判断是否存在相同名称
        public bool IsExistSame(Driver obj)
        {
            var where = _context.Driver.Where(p => p.Name == obj.Name);
            if (obj.PK != null)
            {
                where = where.Where(p => p.PK != obj.PK);
            }
            List<Driver> list = where.ToList();
            if (list.Count > 0)
            {
                return true;
            }
            return false;
        }

        // 新增
        [Route("add")]
        [HttpPost]
        public ResultObj Add(Driver obj)
        {
            // 获取当前登录用户名
            string _currentUserName = CommonService.GetCurrentUser(HttpContext).UserName;

            ResultObj resultObj = new ResultObj();
            if (IsExistSame(obj))
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "该名称已存在。";
                return resultObj;
            }

            obj.CreateUser = _currentUserName;
            obj.CreateTime = DateTime.Now;
            obj.LastUpdateUser = _currentUserName;
            obj.LastUpdateTime = DateTime.Now;

            _context.Driver.Add(obj);
            _context.SaveChanges();

            resultObj.IsSuccess = true;
            return resultObj;
        }

        // 修改
        [Route("update")]
        [HttpPost]
        public ResultObj Update(Driver newObj)
        {
            // 获取当前登录用户名
            string _currentUserName = CommonService.GetCurrentUser(HttpContext).UserName;

            ResultObj resultObj = new ResultObj();

            var obj = _context.Driver.Find(newObj.PK);
            if (obj == null)
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "修改对象不存在。";
                return resultObj;
            }

            if (IsExistSame(newObj))
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "该名称已存在。";
                return resultObj;
            }

            obj.Company = newObj.Company;
            obj.Name = newObj.Name;
            obj.Remark = newObj.Remark;

            obj.LastUpdateTime = DateTime.Now;
            obj.LastUpdateUser = _currentUserName;

            _context.Driver.Update(obj);
            _context.SaveChanges();

            resultObj.IsSuccess = true;
            return resultObj;
        }

        // 删除
        [Route("delete")]
        [HttpPost]
        public IActionResult Delete(DelObj delObj)
        {
            for (int i = 0; i < delObj.Id.Count(); i++)
            {
                var obj = _context.Driver.Find(delObj.Id[i]);
                if (obj == null)
                {
                    return NotFound();
                }
                _context.Driver.Remove(obj);
                _context.SaveChanges();
            }

            return NoContent();
        }
    }

}
