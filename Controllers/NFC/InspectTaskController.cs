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
    // 巡检任务相关接口
    [Route("api/inspecttask")]
    [ApiController]
    public class InspectTaskController : ControllerBase
    {
        private readonly H4WebContext _context;

        public InspectTaskController(H4WebContext context)
        {
            _context = context;
        }

        // 带多个查询条件的查询：查询巡检任务
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
            string taskName = jObject.Value<string>("taskName");
            string taskNo = jObject.Value<string>("taskNo");
            string inspectTime = jObject.Value<string>("inspectTime");
            string inspectUser = jObject.Value<string>("inspectUser");

            //防止查询条件都不满足，先生成一个空的查询
            var where = (from task in _context.InspectTask.GroupBy(t => new { t.TaskNo, t.TaskName, t.InspectCycles, t.InspectUser, t.InspectTime, t.LineName})
                         select new InspectTaskView
                         {
                             TaskNo = task.Key.TaskNo,
                             TaskName = task.Key.TaskName,
                             InspectTime = task.Key.InspectTime,
                             InspectUser = task.Key.InspectUser,
                             LineName = task.Key.LineName,
                             InspectCycles = task.Key.InspectCycles,
                             SumCount = _context.InspectTask.Where(p => p.TaskNo.Equals(task.Key.TaskNo)).Count(),
                             IsCompleteCount = _context.InspectTask.Where(p => p.TaskNo.Equals(task.Key.TaskNo) && p.IsComplete.Equals("2")).Count(),
                         }).Where(p => true);

            if (string.IsNullOrEmpty(taskNo) == false)
            {
                where = where.Where(p => p.TaskNo.Contains(taskNo));
            }
            if (string.IsNullOrEmpty(taskName) == false)
            {
                where = where.Where(p => p.TaskName.Contains(taskName));
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
            }
            else
            {
                //默认结果按照任务编号、任务顺序号正序排序
                where = where.OrderBy(p => p.TaskNo);
            }
           
            //跳过翻页的数量
            where = where.Skip(pageSize * (current - 1));
            //获取结果
            List<InspectTaskView> dataList = where.Take(pageSize).ToList();

            TableData resultObj = new TableData
            {
                Data = dataList,
                Current = current,
                Success = true,
                PageSize = pageSize,
                Total = total
            };

            return resultObj;
        }

        // 查询巡检任务详情
        [Route("querydetail")]
        [HttpGet]
        public ActionResult<TableData> QueryDetail(string queryStr)
        {

            JObject jObject = new JObject();
            if (string.IsNullOrEmpty(queryStr) == false)
            {
                jObject = JsonConvert.DeserializeObject<JObject>(queryStr);
            }

            int current = jObject.Value<int>("current") == 0 ? 1 : jObject.Value<int>("current");
            int pageSize = jObject.Value<int>("pageSize") == 0 ? 20 : jObject.Value<int>("pageSize");
            string taskName = jObject.Value<string>("taskName");
            string taskNo = jObject.Value<string>("taskNo");
            string deviceName = jObject.Value<string>("deviceName");
            string inspectName = jObject.Value<string>("inspectName");
            string inspectItemName = jObject.Value<string>("inspectItemName");
            string isComplete = jObject.Value<string>("isComplete");

            //防止查询条件都不满足，先生成一个空的查询
            var where = (from task in _context.InspectTask
                         join inspect in _context.Inspect on task.InspectNo equals inspect.InspectNo
                         join device in _context.Device on task.DeviceNo equals device.DeviceNo
                         join nfcCard in _context.NFCCard on device.DeviceNo equals nfcCard.DeviceNo
                         join inspectItem in _context.InspectItems on task.InspectItemNo equals inspectItem.InspectItemNo
                         select new InspectTaskView
                         {
                             GId = task.GId,
                             InspectNo = task.InspectNo,
                             DeviceNo = task.DeviceNo,
                             InspectItemNo = task.InspectItemNo,
                             TaskOrderNo = task.TaskOrderNo,
                             TaskNo = task.TaskNo,
                             TaskName = task.TaskName,
                             InspectTime = task.InspectTime,
                             InspectUser = task.InspectUser,
                             IsComplete = task.IsComplete,
                             InspectCompleteTime = task.InspectCompleteTime,
                             InspectCompleteUser = task.InspectCompleteUser,
                             Remark = task.Remark,
                             CreateTime = task.CreateTime,
                             CreateUser = task.CreateUser,
                             LastUpdateTime = task.LastUpdateTime,
                             LastUpdateUser = task.LastUpdateUser,

                             InspectName = inspect.InspectName,
                             DeviceName = device.DeviceName,
                             InspectItemName = inspectItem.InspectItemName,
                             NfcCardNo = nfcCard.NFCCardNo,
                             Unit = inspectItem.Unit,
                             ItemRemark = inspectItem.Remark,

                         }).Where(p => true);

            if (string.IsNullOrEmpty(taskNo) == false)
            {
                where = where.Where(p => p.TaskNo.Contains(taskNo));
            }
            if (string.IsNullOrEmpty(taskName) == false)
            {
                where = where.Where(p => p.TaskName.Contains(taskName));
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
            if (string.IsNullOrEmpty(isComplete) == false)
            {
                where = where.Where(p => p.IsComplete.Equals(isComplete));
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
                // 按照巡检完成时间排序
                if (sorterKey.Equals("inspectCompleteTime") && sortRule.Equals("descend"))
                {
                    where = where.OrderByDescending(p => p.InspectCompleteTime);
                }
                else if (sorterKey.Equals("inspectCompleteTime") && sortRule.Equals("ascend"))
                {
                    where = where.OrderBy(p => p.InspectCompleteTime);
                }
            }
            else
            {
                //默认结果按照任务编号、任务顺序号正序排序
                where = where.OrderBy(p => p.TaskNo).ThenBy(p => p.TaskOrderNo);
            }

            //跳过翻页的数量
            where = where.Skip(pageSize * (current - 1));
            //获取结果
            List<InspectTaskView> dataList = where.Take(pageSize).ToList();

            TableData tableData = new TableData
            {
                Data = dataList,
                Current = current,
                Success = true,
                PageSize = pageSize,
                Total = total
            };
            TableData resultObj = tableData;

            return resultObj;
        }

        // 手机端获取任务数据
        [Route("gettasklist")]
        [HttpGet]
        public List<InspectTaskView> GetTaskList(string queryStr)
        {

            JObject jObject = new JObject();
            if (string.IsNullOrEmpty(queryStr) == false)
            {
                jObject = JsonConvert.DeserializeObject<JObject>(queryStr);
            }
         
            string taskName = jObject.Value<string>("taskName");
            string taskNo = jObject.Value<string>("taskNo");
            string deviceName = jObject.Value<string>("deviceName");
            string inspectName = jObject.Value<string>("inspectName");
            string inspectItemName = jObject.Value<string>("inspectItemName");
            string inspectTime = jObject.Value<string>("inspectTime");
            string inspectUser = jObject.Value<string>("inspectUser");

            //防止查询条件都不满足，先生成一个空的查询
            var where = (from task in _context.InspectTask
                         join inspect in _context.Inspect on task.InspectNo equals inspect.InspectNo
                         join device in _context.Device on task.DeviceNo equals device.DeviceNo
                         join nfcCard in _context.NFCCard on device.DeviceNo equals nfcCard.DeviceNo
                         join inspectItem in _context.InspectItems on task.InspectItemNo equals inspectItem.InspectItemNo
                         select new InspectTaskView
                         {
                             GId = task.GId,
                             InspectNo = task.InspectNo,
                             DeviceNo = task.DeviceNo,
                             InspectItemNo = task.InspectItemNo,
                             TaskOrderNo = task.TaskOrderNo,
                             TaskNo = task.TaskNo,
                             TaskName = task.TaskName,
                             InspectTime = task.InspectTime,
                             InspectUser = task.InspectUser,
                             IsComplete = task.IsComplete,
                             InspectCompleteTime = task.InspectCompleteTime,
                             InspectCompleteUser = task.InspectCompleteUser,
                             Remark = task.Remark,
                             CreateTime = task.CreateTime,
                             CreateUser = task.CreateUser,
                             LastUpdateTime = task.LastUpdateTime,
                             LastUpdateUser = task.LastUpdateUser,
                             LineName = task.LineName,
                             InspectCycles = task.InspectCycles,
                             CycleStartTime = task.CycleStartTime,
                             CycleEndTime = task.CycleEndTime,

                             InspectName = inspect.InspectName,
                             DeviceName = device.DeviceName,
                             InspectItemName = inspectItem.InspectItemName,
                             NfcCardNo = nfcCard.NFCCardNo,
                             Unit = inspectItem.Unit,
                             ItemRemark = inspectItem.Remark,

                             //待下发或者待处理的数据都可以请求到,只请求当天的数据
                         }).Where(p => (p.IsComplete.Equals("0") || p.IsComplete.Equals("1")) && p.InspectTime.Date == DateTime.Now.Date);

            if (string.IsNullOrEmpty(taskNo) == false)
            {
                where = where.Where(p => p.TaskNo.Contains(taskNo));
            }
            if (string.IsNullOrEmpty(taskName) == false)
            {
                where = where.Where(p => p.TaskName.Contains(taskName));
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

            //默认结果按照任务编号、任务顺序号正序排序
            where = where.OrderBy(p => p.TaskNo).ThenBy(p => p.TaskOrderNo);
            //获取结果
            List<InspectTaskView> dataList = where.ToList();

            // 批量更新任务状态：待完成1
            List<InspectTask> updateList = new List<InspectTask>();
            for (int i = 0; i < dataList.Count; i++)
            {
                InspectTask updateObj = _context.InspectTask.Find(dataList[i].GId);
                if (updateObj != null)
                {
                    updateObj.IsComplete = "1";//待完成
                    updateObj.LastUpdateTime = DateTime.Now;
                    updateObj.LastUpdateUser = inspectUser;
                    updateList.Add(updateObj);
                }
            }
            _context.InspectTask.UpdateRange(updateList);
            _context.SaveChanges();

            return dataList;
        }

        // 根据id查询
        [Route("getbyid")]
        [HttpGet(Name = "GetInspectTaskObj")]
        public ActionResult<InspectTask> GetById(Guid gid)
        {
            var item = _context.InspectTask.Find(gid);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        // 判断是否存在相同编号的巡检任务
        [Route("isexistsameno")]
        public bool IsExistSameNo(InspectTask obj)
        {
            var where = _context.InspectTask.Where(p => p.TaskNo == obj.TaskNo);
            if (obj.GId != null)
            {
                where = where.Where(p => p.GId != obj.GId);
            }
            List<InspectTask> list = where.ToList();
            if (list.Count > 0)
            {
                return true;
            }
            return false;
        }

        // 新增
        [Route("add")]
        [HttpPost]
        public ResultObj Add(InspectTask obj)
        {
            ResultObj resultObj = new ResultObj();
            if (IsExistSameNo(obj))
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "编号已存在。";
                return resultObj;
            }

            obj.CreateUser = obj.CreateUser;
            obj.CreateTime = DateTime.Now;
            obj.LastUpdateUser = obj.LastUpdateUser;
            obj.LastUpdateTime = DateTime.Now;

            _context.InspectTask.Add(obj);
            _context.SaveChanges();

            resultObj.IsSuccess = true;
            return resultObj;
        }


        // 新增批量巡检任务（所有巡检点）
        [Route("addbatchtask")]
        [HttpPost]
        public ResultObj AddBatchTask(InspectTask obj)
        {
            ResultObj resultObj = new ResultObj();

            // 根据设备巡检项目表，先查询需要生成的任务数据
            var query = from deviceInspectItem in _context.DeviceInspectItem
                        join device in _context.Device on deviceInspectItem.DeviceNo equals device.DeviceNo
                        join nfcCard in _context.NFCCard on device.DeviceNo equals nfcCard.DeviceNo
                        join inspect in _context.Inspect on device.InspectNo equals inspect.InspectNo
                        orderby inspect.InspectOrderNo, device.DeviceNo, deviceInspectItem.InspectItemNo
                        select new InspectTask
                        {
                            InspectNo = device.InspectNo,
                            DeviceNo = device.DeviceNo,
                            InspectItemNo = deviceInspectItem.InspectItemNo
                        };
            List<InspectTask> taskList = query.ToList();

            List<InspectTask> addList = new List<InspectTask>();
            // 巡检顺序编号
            int taskOrderNo = 0;
            for (int i = 0;i< taskList.Count; i++)
            {
                
                taskOrderNo++;
                InspectTask inspectTaskObj = new InspectTask();
                inspectTaskObj.InspectNo = taskList[i].InspectNo;
                inspectTaskObj.DeviceNo = taskList[i].DeviceNo;
                inspectTaskObj.InspectItemNo = taskList[i].InspectItemNo;
                inspectTaskObj.TaskOrderNo = taskOrderNo;
                inspectTaskObj.TaskName = string.Format("{0:yyyyMMddHH}", obj.InspectTime) + "全巡检点任务";
                inspectTaskObj.TaskNo = string.Format("{0:yyyyMMddHH}", obj.InspectTime) + "ALL"; 
                inspectTaskObj.InspectTime = obj.InspectTime;
                inspectTaskObj.InspectUser = obj.InspectUser;
                inspectTaskObj.IsComplete = "0";
                inspectTaskObj.Remark = obj.Remark;
                inspectTaskObj.CreateUser = obj.CreateUser;
                inspectTaskObj.CreateTime = DateTime.Now;
                inspectTaskObj.LastUpdateUser = obj.LastUpdateUser;
                inspectTaskObj.LastUpdateTime = DateTime.Now;
                addList.Add(inspectTaskObj);
            }
            _context.InspectTask.AddRange(addList);
            _context.SaveChanges();

            resultObj.IsSuccess = true;
            return resultObj;
        }

        // 新增临时巡检任务（选择巡检路线）
        [Route("addtemptask")]
        [HttpPost]
        public ResultObj AddTempTask(InspectTask obj)
        {
            ResultObj resultObj = new ResultObj();

            // 根据巡检路线，计算出巡检任务的巡检项目
            InspectLine inspectLine = _context.InspectLine.Where(p => p.LineName.Equals(obj.LineName)).FirstOrDefault();
            string[] lineDeviceItem = inspectLine.DeviceInspectItems.Split(",");
            List<Guid> lineDeviceItemList = new List<Guid>();
            for (int i = 0; i < lineDeviceItem.Length; i++)
            {
                lineDeviceItemList.Add(new Guid(lineDeviceItem[i]));
            }

            // 根据设备巡检项目表，先查询需要生成的任务数据
            var query = from deviceInspectItem in _context.DeviceInspectItem
                        join device in _context.Device on deviceInspectItem.DeviceNo equals device.DeviceNo
                        join nfcCard in _context.NFCCard on device.DeviceNo equals nfcCard.DeviceNo
                        join inspect in _context.Inspect on device.InspectNo equals inspect.InspectNo
                        orderby inspect.InspectOrderNo, device.DeviceNo, deviceInspectItem.InspectItemNo
                        select new InspectTask
                        {
                            InspectNo = device.InspectNo,
                            DeviceNo = device.DeviceNo,
                            InspectItemNo = deviceInspectItem.InspectItemNo,
                            GId = deviceInspectItem.GId,
                        };
            List<InspectTask> taskList = query.ToList();

            List<InspectTask> addList = new List<InspectTask>();
            // 巡检顺序编号
            int taskOrderNo = 0;
            for (int i = 0; i < taskList.Count; i++)
            {
                if (lineDeviceItemList.Contains(taskList[i].GId))
                {
                    taskOrderNo++;
                    InspectTask inspectTaskObj = new InspectTask();
                    inspectTaskObj.InspectNo = taskList[i].InspectNo;
                    inspectTaskObj.DeviceNo = taskList[i].DeviceNo;
                    inspectTaskObj.InspectItemNo = taskList[i].InspectItemNo;
                    inspectTaskObj.TaskOrderNo = taskOrderNo;
                    inspectTaskObj.TaskName = string.Format("{0:yyyyMMddHHmmss}", obj.InspectTime) + "临时任务";
                    inspectTaskObj.TaskNo = string.Format("{0:yyyyMMddHHmmss}", obj.InspectTime)+"TEMP";
                    inspectTaskObj.InspectTime = obj.InspectTime;
                    inspectTaskObj.InspectUser = obj.InspectUser;
                    inspectTaskObj.LineName = obj.LineName;
                    inspectTaskObj.IsComplete = "0";
                    inspectTaskObj.Remark = obj.Remark;
                    inspectTaskObj.CreateUser = obj.CreateUser;
                    inspectTaskObj.CreateTime = DateTime.Now;
                    inspectTaskObj.LastUpdateUser = obj.LastUpdateUser;
                    inspectTaskObj.LastUpdateTime = DateTime.Now;
                    addList.Add(inspectTaskObj);
                }
            }
            _context.InspectTask.AddRange(addList);
            _context.SaveChanges();

            resultObj.IsSuccess = true;
            return resultObj;
        }

        // 新增巡检任务(选择巡检周期、巡检路线)
        [Route("addlinetask")]
        [HttpPost]
        public ResultObj AddLineTask(InspectTask obj)
        {
            ResultObj resultObj = new ResultObj();

            // 根据巡检周期，周期起始、结束时间，计算需要生成的任务巡检时间数组
            List<DateTime> inspectTimeList = new List<DateTime>();

            // 如果选择的开始时间小于当前时间，则取当前时间
            DateTime startTime = obj.CycleStartTime < DateTime.Now ? DateTime.Now.AddHours(1) : obj.CycleStartTime.Date;
            // 偶数整点开始执行
            int startHour = (startTime.Hour) % 2 == 0 ? startTime.Hour : startTime.Hour + 1;
            startTime = DateTime.Parse(string.Format("{0:yyyy/MM/dd }",startTime) + startHour + ":00:00");

            // 如果结束时间小于开始时间，则取开始时间
            DateTime endTime = obj.CycleEndTime < startTime ? startTime : obj.CycleEndTime;
            // 结束日期后一天的0点为周期的结束时间
            endTime = DateTime.Parse(string.Format("{0:yyyy/MM/dd }", endTime.AddDays(1)) + "00:00:00");

            while (startTime < endTime)
            {
                inspectTimeList.Add(startTime);
                if (obj.InspectCycles.Equals("每两小时巡检一次"))
                {
                    startTime = startTime.AddHours(2);
                }
            }

            // 根据巡检路线，计算出巡检任务的巡检项目
            InspectLine inspectLine = _context.InspectLine.Where(p=>p.LineName.Equals(obj.LineName)).FirstOrDefault();
            string[] lineDeviceItem = inspectLine.DeviceInspectItems.Split(",");
            List<Guid> lineDeviceItemList = new List<Guid>();
            for (int i = 0; i < lineDeviceItem.Length; i++)
            {
                lineDeviceItemList.Add(new Guid(lineDeviceItem[i]));
            }

            // 根据设备巡检项目表，先查询需要生成的任务数据
            var query = from deviceInspectItem in _context.DeviceInspectItem
                        join device in _context.Device on deviceInspectItem.DeviceNo equals device.DeviceNo
                        join nfcCard in _context.NFCCard on device.DeviceNo equals nfcCard.DeviceNo
                        join inspect in _context.Inspect on device.InspectNo equals inspect.InspectNo
                        orderby inspect.InspectOrderNo, device.DeviceNo, deviceInspectItem.InspectItemNo
                        select new InspectTask
                        {
                            InspectNo = device.InspectNo,
                            DeviceNo = device.DeviceNo,
                            InspectItemNo = deviceInspectItem.InspectItemNo,
                            GId = deviceInspectItem.GId,
                        };
            List<InspectTask> taskList = query.ToList();

            List<InspectTask> addList = new List<InspectTask>();
            for (int j = 0; j < inspectTimeList.Count; j++)
            {
                // 任务编号(按照巡检时间生成任务编号)
                string taskNo = string.Format("{0:yyyyMMddHH}", inspectTimeList[j]) + "CYCLE"; ;
                string taskName = string.Format("{0:yyyyMMddHH}", inspectTimeList[j])+"定期任务";
                // 巡检顺序编号
                int taskOrderNo = 0;
                for (int i = 0; i < taskList.Count; i++)
                {
                    if (lineDeviceItemList.Contains(taskList[i].GId))
                    {
                        taskOrderNo++;
                        InspectTask inspectTaskObj = new InspectTask();
                        inspectTaskObj.InspectNo = taskList[i].InspectNo;
                        inspectTaskObj.DeviceNo = taskList[i].DeviceNo;
                        inspectTaskObj.InspectItemNo = taskList[i].InspectItemNo;
                        inspectTaskObj.TaskOrderNo = taskOrderNo;
                        inspectTaskObj.TaskName = taskName;
                        inspectTaskObj.TaskNo = taskNo;
                        inspectTaskObj.LineName = obj.LineName;
                        inspectTaskObj.InspectCycles = obj.InspectCycles;
                        inspectTaskObj.CycleStartTime = obj.CycleStartTime;
                        inspectTaskObj.CycleEndTime = obj.CycleEndTime;
                        inspectTaskObj.InspectTime = inspectTimeList[j];
                        inspectTaskObj.InspectUser = obj.InspectUser;
                        inspectTaskObj.IsComplete = "0";
                        inspectTaskObj.Remark = obj.Remark;
                        inspectTaskObj.CreateUser = obj.CreateUser;
                        inspectTaskObj.CreateTime = DateTime.Now;
                        inspectTaskObj.LastUpdateUser = obj.LastUpdateUser;
                        inspectTaskObj.LastUpdateTime = DateTime.Now;
                        addList.Add(inspectTaskObj);
                    }
                }
            }
            _context.InspectTask.AddRange(addList);
            _context.SaveChanges();

            resultObj.IsSuccess = true;
            return resultObj;
        }

        // 修改
        [Route("update")]
        [HttpPost]
        public ResultObj Update(InspectTask newObj)
        {
            ResultObj resultObj = new ResultObj();

            var obj = _context.InspectTask.Find(newObj.GId);
            if (obj == null)
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "修改对象不存在。";
                return resultObj;
            }

            //obj.TaskNo = newObj.TaskNo;
            //obj.TaskName = newObj.TaskName;
            obj.TaskOrderNo = newObj.TaskOrderNo;
            obj.DeviceNo = newObj.DeviceNo;
            obj.InspectItemNo = newObj.InspectItemNo;
            obj.InspectNo = newObj.InspectNo;
            obj.InspectTime = newObj.InspectTime;
            obj.InspectUser = newObj.InspectUser;
            obj.Remark = newObj.Remark;

            obj.LastUpdateTime = DateTime.Now;
            obj.LastUpdateUser = newObj.LastUpdateUser;

            _context.InspectTask.Update(obj);
            _context.SaveChanges();

            resultObj.IsSuccess = true;
            return resultObj;
        }

        // 删除
        [Route("delete")]
        [HttpPost]
        public IActionResult Delete(DelObj delObj)
        {
            List<InspectTask> delList = new List<InspectTask>();
            for (int i = 0; i < delObj.Id.Count(); i++)
            {
                var obj = _context.InspectTask.Find(delObj.Id[i]);
                if (obj == null)
                {
                    return NotFound();
                }
                delList.Add(obj);
            }
            _context.InspectTask.RemoveRange(delList);
            _context.SaveChanges();

            return NoContent();
        }

        // 根据任务编号删除
        [Route("deletebyno")]
        [HttpPost]
        public IActionResult DeleteByNo(DelObjStr delObj)
        {
            List<InspectTask> delList = new List<InspectTask>();
            for (int i = 0; i < delObj.TaskNo.Count(); i++)
            {
                List<InspectTask> list = _context.InspectTask.Where(p=>p.TaskNo.Equals(delObj.TaskNo[i])).ToList();
                delList.AddRange(list);
            }
            _context.InspectTask.RemoveRange(delList);
            _context.SaveChanges();

            return NoContent();
        }
    }

    public class DelObjStr
    {
        public List<string> TaskNo { get; set; }
    }
}
