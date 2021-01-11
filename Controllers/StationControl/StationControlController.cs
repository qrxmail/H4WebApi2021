using Microsoft.AspNetCore.Mvc;
using H4WebApi.Models;
using H4WebApi.Services;
using System;

namespace H4WebApi.Controllers.BaseInfo
{
    [Route("api")]
    [ApiController]
    public class StationControlController : ControllerBase
    {
        // 获取站点数据
        [Route("getStationData")]
        [HttpGet]
        public IActionResult GetStationData()
        {
            string jsonStrData = StationControlService.GetStationJsonData();
            return new OkObjectResult(jsonStrData);
        }

        // 设备控制
        [Route("deviceControl")]
        [HttpPost]
        public ResultObj DeviceControl(ControlModel model)
        {
            ResultObj resultObj = new ResultObj();
            try
            {
                var result = StationControlService.DeviceControlService(model);
                resultObj.IsSuccess = result.Result.Equals("1");
            }
            catch (Exception e)
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = e.Message;
            }
            return resultObj;
        }

        // 获取系统参数
        [Route("getStationConfigData")]
        [HttpGet]
        public IActionResult GetStationConfigData()
        {
            string jsonStrData = StationControlService.GetStationConfigJsonData();
            return new OkObjectResult(jsonStrData);
        }

        // 设置系统参数
        [Route("setSystemSettingPara")]
        [HttpPost]
        public ResultObj SetSystemSettingPara(CmsSystemSetting obj)
        {
            ResultObj resultObj = new ResultObj();
            try
            {
                StationControlService.SetSystemSettingParaService(obj);
            }
            catch (Exception e)
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = e.Message;
            }
            return resultObj;
        }
    }

}
