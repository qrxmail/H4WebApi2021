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
    // 巡检数据相关接口
    [Route("api/inspectdata")]
    [ApiController]
    public class InspectDataController : ControllerBase
    {
        private readonly H4WebContext _context;

        public InspectDataController(H4WebContext context)
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
            string deviceName = jObject.Value<string>("deviceName");
            string inspectName = jObject.Value<string>("inspectName");
            string inspectItemName = jObject.Value<string>("inspectItemName");
            string taskNo = jObject.Value<string>("taskNo");
            string inspectTime = jObject.Value<string>("inspectTime");
            string inspectUser = jObject.Value<string>("inspectUser");
            string isJumpInspect = jObject.Value<string>("isJumpInspect");

            //防止查询条件都不满足，先生成一个空的查询
            var where = (from data in _context.InspectData
                         join inspect in _context.Inspect on data.InspectNo equals inspect.InspectNo
                         join device in _context.Device on data.DeviceNo equals device.DeviceNo
                         join inspectItem in _context.InspectItems on data.InspectItemNo equals inspectItem.InspectItemNo
                         select new
                         {
                             GId = data.GId,
                             TaskId = data.TaskId,
                             TaskNo = data.TaskNo,
                             InspectNo = data.InspectNo,
                             DeviceNo = data.DeviceNo,
                             InspectItemNo = data.InspectItemNo,
                             ResultValue = data.ResultValue,
                             IsJumpInspect = data.IsJumpInspect,
                             JumpInspectReason = data.JumpInspectReason,
                             InspectTime = data.InspectTime,
                             InspectUser = data.InspectUser,
                             Remark = data.Remark,
                             CreateTime = data.CreateTime,
                             CreateUser = data.CreateUser,
                             LastUpdateTime = data.LastUpdateTime,
                             LastUpdateUser = data.LastUpdateUser,

                             InspectName = inspect.InspectName,
                             DeviceName = device.DeviceName,
                             InspectItemName = inspectItem.InspectItemName,
                             Unit = inspectItem.Unit,

                         }).Where(p => true);


            if (string.IsNullOrEmpty(taskNo) == false)
            {
                where = where.Where(p => p.TaskNo.Contains(taskNo));
            }
            if (string.IsNullOrEmpty(inspectName) == false)
            {
                where = where.Where(p => p.InspectName.Contains(inspectName));
            }
            if (string.IsNullOrEmpty(deviceName) == false)
            {
                where = where.Where(p => p.DeviceName.Contains(deviceName));
            }
            if (string.IsNullOrEmpty(inspectItemName) == false)
            {
                where = where.Where(p => p.InspectItemName.Contains(inspectItemName));
            }
            if (string.IsNullOrEmpty(inspectTime) == false)
            {
                DateTime inpectDate = DateTime.Parse(DateTime.Parse(inspectTime).ToShortDateString());
                DateTime startTime = inpectDate.AddDays(0);
                DateTime endTime = inpectDate.AddDays(1);
                where = where.Where(p => p.InspectTime >= startTime && p.InspectTime <= endTime);
            }
            if (string.IsNullOrEmpty(inspectUser) == false)
            {
                where = where.Where(p => p.InspectUser.Contains(inspectUser));
            }
            if (string.IsNullOrEmpty(isJumpInspect) == false)
            {
                where = where.Where(p => p.IsJumpInspect.Equals(isJumpInspect));
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
                // 按照巡检时间排序
                if (sorterKey.Equals("inspectTime") && sortRule.Equals("descend"))
                {
                    where = where.OrderByDescending(p => p.InspectTime);
                }
                else if (sorterKey.Equals("inspectTime") && sortRule.Equals("ascend"))
                {
                    where = where.OrderBy(p => p.InspectTime);
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
                //默认结果按照最后更新时间倒序排序
                where = where.OrderByDescending(p => p.LastUpdateTime);
            }

            //跳过翻页的数量
            where = where.Skip(pageSize * (current - 1));
            //获取结果
            dynamic dataList = where.Take(pageSize).ToList();

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
        [HttpGet(Name = "GetInspectDataObj")]
        public ActionResult<InspectData> GetById(Guid gid)
        {
            var item = _context.InspectData.Find(gid);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        // 新增
        [Route("add")]
        [HttpPost]
        public ResultObj Add(InspectData obj)
        {
            ResultObj resultObj = new ResultObj();

            obj.CreateUser = obj.CreateUser;
            obj.CreateTime = DateTime.Now;
            obj.LastUpdateUser = obj.LastUpdateUser;
            obj.LastUpdateTime = DateTime.Now;

            _context.InspectData.Add(obj);
            _context.SaveChanges();

            // 更新任务表：巡检完成时间，巡检完成人，是否完成
            var taskObj = _context.InspectTask.Find(obj.TaskId);
            // 如果未跳检，则更新巡检任务表
            if (taskObj != null && obj.IsJumpInspect.Equals("0"))
            {
                taskObj.InspectCompleteTime = obj.InspectTime;
                taskObj.InspectCompleteUser = obj.InspectUser;
                taskObj.IsComplete = "2";
                _context.InspectTask.Update(taskObj);
                _context.SaveChanges();

                // 更新nfc卡的最后巡检时间
                List<NFCCard> nfcCardList = _context.NFCCard.Where(p=>p.DeviceNo.Equals(obj.DeviceNo)).ToList();
                if (nfcCardList.Count > 0)
                {
                    NFCCard nfcCardObj = nfcCardList.First();
                    nfcCardObj.LastInspectTime = obj.InspectTime;
                    nfcCardObj.LastInspectUser = obj.InspectUser;
                    _context.NFCCard.Update(nfcCardObj);
                    _context.SaveChanges();
                }
            }

            resultObj.IsSuccess = true;
            return resultObj;
        }

        // 批量上报巡检数据
        [Route("addbatch")]
        [HttpPost]
        public ResultObj AddBatch(List<InspectData> dataList)
        {
            ResultObj resultObj = new ResultObj();
            for (int i = 0; i < dataList.Count; i++)
            {
                Add(dataList[i]);
            }
            resultObj.IsSuccess = true;
            return resultObj;
        }

        // 修改
        [Route("update")]
        [HttpPost]
        public IActionResult Update(InspectData newObj)
        {
            var obj = _context.InspectData.Find(newObj.GId);
            if (obj == null)
            {
                return NotFound();
            }

            //obj.TaskNo = newObj.TaskNo;
            //obj.InspectNo = newObj.InspectNo;
            //obj.DeviceNo = newObj.DeviceNo;
            //obj.InspectItemNo = newObj.InspectItemNo;
            obj.ResultValue = newObj.ResultValue;
            obj.InspectTime = newObj.InspectTime;
            obj.InspectUser = newObj.InspectUser;
            obj.IsJumpInspect = newObj.IsJumpInspect;
            obj.JumpInspectReason = newObj.JumpInspectReason;
            obj.Remark = newObj.Remark;

            obj.LastUpdateTime = DateTime.Now;
            obj.LastUpdateUser = newObj.LastUpdateUser;

            _context.InspectData.Update(obj);
            _context.SaveChanges();
            return NoContent();
        }

        // 删除
        [Route("delete")]
        [HttpPost]
        public IActionResult Delete(DelObj delObj)
        {
            for (int i = 0; i < delObj.Id.Count(); i++)
            {
                var obj = _context.InspectData.Find(delObj.Id[i]);
                if (obj == null)
                {
                    return NotFound();
                }

                _context.InspectData.Remove(obj);
                _context.SaveChanges();
            }

            return NoContent();
        }
    }
}
