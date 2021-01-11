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
    [Route("api/oilStation")]
    [ApiController]
    public class OilStationController : ControllerBase
    {
        private readonly H4WebContext _context;

        public OilStationController(H4WebContext context)
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
            string district = jObject.Value<string>("district");

            //防止查询条件都不满足，先生成一个空的查询
            var where = _context.OilStation.Where(p => true);

            if (string.IsNullOrEmpty(name) == false)
            {
                where = where.Where(p => p.Name.Contains(name));
            }
            if (string.IsNullOrEmpty(district) == false)
            {
                where = where.Where(p => p.District.Contains(district));
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

                if (sorterKey.Equals("district") && sortRule.Equals("descend"))
                {
                    where = where.OrderByDescending(p => p.District);
                }
                else if (sorterKey.Equals("district") && sortRule.Equals("ascend"))
                {
                    where = where.OrderBy(p => p.District);
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
            List<OilStation> dataList = where.Take(pageSize).ToList();

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
        public ActionResult<OilStation> GetById(Guid gid)
        {
            var item = _context.OilStation.Find(gid);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        // 判断是否存在相同站名
        public bool IsExistSame(OilStation obj)
        {
            var where = _context.OilStation.Where(p => p.Name == obj.Name);
            if (obj.PK != null)
            {
                where = where.Where(p => p.PK != obj.PK);
            }
            List<OilStation> list = where.ToList();
            if (list.Count > 0)
            {
                return true;
            }
            return false;
        }

        // 新增
        [Route("add")]
        [HttpPost]
        public ResultObj Add(OilStation obj)
        {
            // 获取当前登录用户名
            string _currentUserName = CommonService.GetCurrentUser(HttpContext).UserName;

            ResultObj resultObj = new ResultObj();
            if (IsExistSame(obj))
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "该站名已存在。";
                return resultObj;
            }

            obj.CreateUser = _currentUserName;
            obj.CreateTime = DateTime.Now;
            obj.LastUpdateUser = _currentUserName;
            obj.LastUpdateTime = DateTime.Now;

            _context.OilStation.Add(obj);
            _context.SaveChanges();

            resultObj.IsSuccess = true;
            return resultObj;
        }

        // 修改
        [Route("update")]
        [HttpPost]
        public ResultObj Update(OilStation newObj)
        {
            // 获取当前登录用户名
            string _currentUserName = CommonService.GetCurrentUser(HttpContext).UserName;

            ResultObj resultObj = new ResultObj();

            var obj = _context.OilStation.Find(newObj.PK);
            if (obj == null)
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "修改对象不存在。";
                return resultObj;
            }

            if (IsExistSame(newObj))
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "该站名已存在。";
                return resultObj;
            }
            obj.Branch = newObj.Branch;
            obj.Name = newObj.Name;
            obj.District = newObj.District;
            obj.PLCIP = newObj.PLCIP;
            obj.HMIIP = newObj.HMIIP;
            obj.VolumnPer1cm = newObj.VolumnPer1cm;
            obj.LevelCalcFactor = newObj.LevelCalcFactor;
            obj.LevelCalcOffset = newObj.LevelCalcOffset;
            obj.PumpRatedFlow = newObj.PumpRatedFlow;
            obj.PumpCalcFactor = newObj.PumpCalcFactor;
            obj.PumpCalcOffset = newObj.PumpCalcOffset;
            obj.Latitude = newObj.Latitude;
            obj.Longitude = newObj.Longitude;
            obj.Remark = newObj.Remark;

            obj.LastUpdateTime = DateTime.Now;
            obj.LastUpdateUser = _currentUserName;

            _context.OilStation.Update(obj);
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
                var obj = _context.OilStation.Find(delObj.Id[i]);
                if (obj == null)
                {
                    return NotFound();
                }
                _context.OilStation.Remove(obj);
                _context.SaveChanges();
            }

            return NoContent();
        }
    }

}
