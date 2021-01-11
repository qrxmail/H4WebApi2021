using System;
using System.Collections.Generic;
using System.Linq;
using H4WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NFCWebApi.Models;

namespace NFCWebApi.Controllers
{
    // 巡检点相关接口
    [Route("api/inspectcycles")]
    [ApiController]
    public class InspectCyclesController : ControllerBase
    {
        private readonly H4WebContext _context;

        public InspectCyclesController(H4WebContext context)
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
            string cycleName = jObject.Value<string>("cycleName");
           

            //防止查询条件都不满足，先生成一个空的查询
            var where = _context.InspectCycles.Where(p => true);

            if (string.IsNullOrEmpty(cycleName) == false)
            {
                where = where.Where(p => p.CycleName.Contains(cycleName));
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
                // 按照线路名称排序
                if (sorterKey.Equals("cycleName") && sortRule.Equals("descend"))
                {
                    where = where.OrderByDescending(p => p.CycleName);
                }
                else if (sorterKey.Equals("cycleName") && sortRule.Equals("ascend"))
                {
                    where = where.OrderBy(p => p.CycleName);
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
                //结果按照修改时间倒序排序
                where = where.OrderByDescending(p => p.LastUpdateTime);
            }
           
            //跳过翻页的数量
            where = where.Skip(pageSize * (current - 1));
            //获取结果
            List<InspectCycles> dataList = where.Take(pageSize).ToList();

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
        [HttpGet(Name = "GetInspectCyclesObj")]
        public ActionResult<InspectCycles> GetById(Guid gid)
        {
            var item = _context.InspectCycles.Find(gid);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        // 判断是否存在相同名称的巡检路线
        [Route("isexistsamename")]
        public bool IsExistSameName(InspectCycles obj)
        {
            var where = _context.InspectCycles.Where(p=>p.CycleName == obj.CycleName);
            if (obj.GId != null)
            {
                where = where.Where(p => p.GId != obj.GId);
            }
            List<InspectCycles> list = where.ToList();
            if (list.Count>0)
            {
                return true;
            }
            return false;
        }

        // 新增
        [Route("add")]
        [HttpPost]
        public ResultObj Add(InspectCycles obj)
        {
            ResultObj resultObj = new ResultObj();
            if (IsExistSameName(obj))
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "名称已存在。";
                return resultObj;
            }

            obj.CreateUser = obj.CreateUser;
            obj.CreateTime = DateTime.Now;
            obj.LastUpdateUser = obj.LastUpdateUser;
            obj.LastUpdateTime = DateTime.Now;

            _context.InspectCycles.Add(obj);
            _context.SaveChanges();

            resultObj.IsSuccess = true;
            return resultObj;
        }

        // 修改
        [Route("update")]
        [HttpPost]
        public ResultObj Update(InspectCycles newObj)
        {
            ResultObj resultObj = new ResultObj();

            var obj = _context.InspectCycles.Find(newObj.GId);
            if (obj == null)
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "修改对象不存在。";
                return resultObj;
            }

            if (IsExistSameName(newObj))
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "名称已存在。";
                return resultObj;
            }

            obj.CycleName = newObj.CycleName;
            obj.CycleType = newObj.CycleType;
            obj.Remark = newObj.Remark;

            obj.LastUpdateTime = DateTime.Now;
            obj.LastUpdateUser = newObj.LastUpdateUser;

            _context.InspectCycles.Update(obj);
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
                var obj = _context.InspectCycles.Find(delObj.Id[i]);
                if (obj == null)
                {
                    return NotFound();
                }

                _context.InspectCycles.Remove(obj);
                _context.SaveChanges();
            }

            return NoContent();
        }
    }
}
