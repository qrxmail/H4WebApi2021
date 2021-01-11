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
    // nfc卡相关接口
    [Route("api/nfccard")]
    [ApiController]
    public class NFCCardController : ControllerBase
    {
        private readonly H4WebContext _context;

        public NFCCardController(H4WebContext context)
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
            string nfcCardNo = jObject.Value<string>("nfcCardNo");
            string printNo = jObject.Value<string>("printNo");
            string deviceName = jObject.Value<string>("deviceName");


            //防止查询条件都不满足，先生成一个空的查询
            var where = (from nfcCard in _context.NFCCard
                         join device in _context.Device on nfcCard.DeviceNo equals device.DeviceNo
                         select new 
                         {
                             GId = nfcCard.GId,
                             NFCCardNo = nfcCard.NFCCardNo,
                             PrintNo = nfcCard.PrintNo,
                             DeviceNo = nfcCard.DeviceNo,
                             Remark = nfcCard.Remark,
                             CreateTime = nfcCard.CreateTime,
                             CreateUser = nfcCard.CreateUser,
                             LastUpdateTime = nfcCard.LastUpdateTime,
                             LastUpdateUser = nfcCard.LastUpdateUser,
                             LastInspectTime = nfcCard.LastInspectTime,
                             LastInspectUser = nfcCard.LastInspectUser,

                             DeviceName = device.DeviceName,
                         }).Where(p => true);

            if (string.IsNullOrEmpty(nfcCardNo) == false)
            {
                where = where.Where(p => p.NFCCardNo.Contains(nfcCardNo));
            }

            if (string.IsNullOrEmpty(printNo) == false)
            {
                where = where.Where(p => p.PrintNo.Contains(printNo));
            }

            if (string.IsNullOrEmpty(deviceName) == false)
            {
                where = where.Where(p => p.DeviceName.Contains(deviceName));
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
                // 按照nfc卡编号排序
                if (sorterKey.Equals("nfcCardNo") && sortRule.Equals("descend"))
                {
                    where = where.OrderByDescending(p => p.NFCCardNo);
                }
                else if (sorterKey.Equals("nfcCardNo") && sortRule.Equals("ascend"))
                {
                    where = where.OrderBy(p => p.NFCCardNo);
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
                // 按照最后巡检时间排序
                if (sorterKey.Equals("lastInspectTime") && sortRule.Equals("descend"))
                {
                    where = where.OrderByDescending(p => p.LastInspectTime);
                }
                else if (sorterKey.Equals("lastInspectTime") && sortRule.Equals("ascend"))
                {
                    where = where.OrderBy(p => p.LastInspectTime);
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
        [HttpGet(Name = "GetNFCCardObj")]
        public ActionResult<NFCCard> GetById(Guid gid)
        {
            var item = _context.NFCCard.Find(gid);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        // 判断是否存在相同编号的NFC卡
        [Route("isexistsameno")]
        public bool IsExistSameNo(NFCCard obj)
        {
            var where = _context.NFCCard.Where(p => p.NFCCardNo == obj.NFCCardNo);
            if (obj.GId != null)
            {
                where = where.Where(p => p.GId != obj.GId);
            }
            List<NFCCard> list = where.ToList();
            if (list.Count > 0)
            {
                return true;
            }
            return false;
        }

        // 判断是否存在相同设备编号的NFC卡
        [Route("isexistsamedeviceno")]
        public bool IsExistSameDeviceNo(NFCCard obj)
        {
            var where = _context.NFCCard.Where(p => p.DeviceNo == obj.DeviceNo);
            if (obj.GId != null)
            {
                where = where.Where(p => p.GId != obj.GId);
            }
            List<NFCCard> list = where.ToList();
            if (list.Count > 0)
            {
                return true;
            }
            return false;
        }

        // 新增
        [Route("add")]
        [HttpPost]
        public ResultObj Add(NFCCard obj)
        {
            ResultObj resultObj = new ResultObj();
            if (IsExistSameNo(obj))
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "编号已存在。";
                return resultObj;
            }

            if (IsExistSameDeviceNo(obj))
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "设备已被其他NFC卡绑定过了。";
                return resultObj;
            }

            obj.CreateUser = obj.CreateUser;
            obj.CreateTime = DateTime.Now;
            obj.LastUpdateUser = obj.LastUpdateUser;
            obj.LastUpdateTime = DateTime.Now;

            _context.NFCCard.Add(obj);
            _context.SaveChanges();

            resultObj.IsSuccess = true;
            return resultObj;
        }

        // 修改
        [Route("update")]
        [HttpPost]
        public ResultObj Update(NFCCard newObj)
        {
            ResultObj resultObj = new ResultObj();

            var obj = _context.NFCCard.Find(newObj.GId);
            if (obj == null)
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "修改对象不存在。";
                return resultObj;
            }

            if (IsExistSameNo(newObj))
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "编号已存在。";
                return resultObj;
            }

            if (IsExistSameDeviceNo(obj))
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "设备已被其他NFC卡绑定过了。";
                return resultObj;
            }

            obj.NFCCardNo = newObj.NFCCardNo;
            obj.PrintNo = newObj.PrintNo;
            obj.DeviceNo = newObj.DeviceNo;
            obj.Remark = newObj.Remark;

            obj.LastUpdateTime = DateTime.Now;
            obj.LastUpdateUser = newObj.LastUpdateUser;

            _context.NFCCard.Update(obj);
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
                var obj = _context.NFCCard.Find(delObj.Id[i]);
                if (obj == null)
                {
                    return NotFound();
                }

                _context.NFCCard.Remove(obj);
                _context.SaveChanges();
            }

            return NoContent();
        }
    }
}

