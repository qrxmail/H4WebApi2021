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
    [Route("api/truck")]
    [ApiController]
    public class TruckController : ControllerBase
    {
        private readonly H4WebContext _context;

        public TruckController(H4WebContext context)
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
            string number = jObject.Value<string>("number");
            string company = jObject.Value<string>("company");
            string leadSealNumber = jObject.Value<string>("leadSealNumber");

            //防止查询条件都不满足，先生成一个空的查询
            var where = _context.Truck.Where(p => true);

            if (string.IsNullOrEmpty(number) == false)
            {
                where = where.Where(p => p.Number.Contains(number));
            }
            if (string.IsNullOrEmpty(company) == false)
            {
                where = where.Where(p => p.Company.Contains(company));
            }
            if (string.IsNullOrEmpty(leadSealNumber) == false)
            {
                where = where.Where(p => p.LeadSealNumber.Contains(leadSealNumber));
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
                if (sorterKey.Equals("number") && sortRule.Equals("descend"))
                {
                    where = where.OrderByDescending(p => p.Number);
                }
                else if (sorterKey.Equals("number") && sortRule.Equals("ascend"))
                {
                    where = where.OrderBy(p => p.Number);
                }

                if (sorterKey.Equals("company") && sortRule.Equals("ascend"))
                {
                    where = where.OrderBy(p => p.Company);
                }
                else if (sorterKey.Equals("company") && sortRule.Equals("ascend"))
                {
                    where = where.OrderBy(p => p.Company);
                }

                if (sorterKey.Equals("leadSealNumber") && sortRule.Equals("ascend"))
                {
                    where = where.OrderBy(p => p.LeadSealNumber);
                }
                else if (sorterKey.Equals("leadSealNumber") && sortRule.Equals("ascend"))
                {
                    where = where.OrderBy(p => p.LeadSealNumber);
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
            List<Truck> dataList = where.Take(pageSize).ToList();

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
        public ActionResult<Truck> GetById(Guid gid)
        {
            var item = _context.Truck.Find(gid);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        // 判断是否存在相同车牌号
        public bool IsExistSame(Truck obj)
        {
            var where = _context.Truck.Where(p => p.Number == obj.Number);
            if (obj.PK != null)
            {
                where = where.Where(p => p.PK != obj.PK);
            }
            List<Truck> list = where.ToList();
            if (list.Count > 0)
            {
                return true;
            }
            return false;
        }

        // 新增
        [Route("add")]
        [HttpPost]
        public ResultObj Add(Truck obj)
        {
            // 获取当前登录用户名
            string _currentUserName = CommonService.GetCurrentUser(HttpContext).UserName;

            ResultObj resultObj = new ResultObj();
            if (IsExistSame(obj))
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "该车牌号已存在。";
                return resultObj;
            }

            obj.CreateUser = _currentUserName;
            obj.CreateTime = DateTime.Now;
            obj.LastUpdateUser = _currentUserName;
            obj.LastUpdateTime = DateTime.Now;

            _context.Truck.Add(obj);
            _context.SaveChanges();

            resultObj.IsSuccess = true;
            return resultObj;
        }

        // 修改
        [Route("update")]
        [HttpPost]
        public ResultObj Update(Truck newObj)
        {
            // 获取当前登录用户名
            string _currentUserName = CommonService.GetCurrentUser(HttpContext).UserName;

            ResultObj resultObj = new ResultObj();

            var obj = _context.Truck.Find(newObj.PK);
            if (obj == null)
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "修改对象不存在。";
                return resultObj;
            }

            if (IsExistSame(newObj))
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "该车牌号已存在。";
                return resultObj;
            }

            obj.Company = newObj.Company;
            obj.Number = newObj.Number;
            obj.Volumn = newObj.Volumn;
            obj.LeadSealNumber = newObj.LeadSealNumber;
            obj.Remark = newObj.Remark;

            obj.LastUpdateTime = DateTime.Now;
            obj.LastUpdateUser = _currentUserName;

            _context.Truck.Update(obj);
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
                var obj = _context.Truck.Find(delObj.Id[i]);
                if (obj == null)
                {
                    return NotFound();
                }
                _context.Truck.Remove(obj);
                _context.SaveChanges();
            }

            return NoContent();
        }
    }

}
