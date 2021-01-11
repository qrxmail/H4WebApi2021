using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using H4WebApi.Models;
using H4WebApi.Models.Work;
using System;
using System.Linq;

namespace H4WebApi.Services
{
    public class JobService
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(JobService));

        private readonly IConfiguration _configuration;

        public JobService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // 自动处理工单方法
        public ResultObj AutoOperWorkTicket(string jsonStrData)
        {
            ResultObj resultObj = new ResultObj();
            string msg = "";
            string tempStr;
            try
            {
                // 序列化为JObject
                JObject jObject = JsonConvert.DeserializeObject<JObject>(jsonStrData);
                if (jObject == null)
                {
                    tempStr = "序列化对象为空：" + jsonStrData;
                    log.Info(tempStr);
                    msg += tempStr + "\n";
                    resultObj.IsSuccess = false;
                    resultObj.Msg = msg;
                    return resultObj;
                }

                // 解析工单数据
                JObject jObjectWork = jObject.Value<JObject>("System");
                if (jObjectWork == null)
                {
                    tempStr = "序列化工单数据为空：" + jObject.Value<string>("System");
                    log.Info(tempStr);
                    msg += tempStr + "\n";
                    resultObj.IsSuccess = false;
                    resultObj.Msg = msg;
                    return resultObj;
                }

                // 拉油完成后，此变量会置1，需将拉油相关信息，如拉油时长、拉液量等保存后，清除此位
                int AckTaskComplete = jObjectWork.Value<int>("AckTaskComplete");
                tempStr = "AckTaskComplete的值为：" + AckTaskComplete;
                log.Info(tempStr);
                msg += tempStr + "\n";
                if (AckTaskComplete == 1)
                {
                    TimeSpan interval;

                    // 实际开始拉油时间
                    DateTime StartLoadingTime = jObjectWork.Value<DateTime>("StartLoadingTime");
                    interval = DateTime.Now - StartLoadingTime;
                    // 计算开始拉油时间和当前时间的时间差（天）
                    if (interval.TotalDays > 1 || interval.TotalDays < 0)
                    {
                        tempStr = "实际开始拉油时间StartLoadingTime不合理：" + StartLoadingTime;
                        log.Info(tempStr);
                        msg += tempStr + "\n";
                        resultObj.IsSuccess = false;
                        resultObj.Msg = msg;
                        return resultObj;
                    }

                    // 实际结束拉油时间
                    DateTime StopLoadingTime = jObjectWork.Value<DateTime>("StopLoadingTime");
                    interval = DateTime.Now - StopLoadingTime;
                    // 计算结束拉油时间和当前时间的时间差（天）
                    if (interval.TotalDays > 1 || interval.TotalDays < 0)
                    {
                        tempStr = "实际结束拉油时间StopLoadingTime不合理：" + StopLoadingTime;
                        log.Info(tempStr);
                        msg += tempStr + "\n";
                        resultObj.IsSuccess = false;
                        resultObj.Msg = msg;
                        return resultObj;
                    }

                    // 开始时间和结束时间差计算（天）
                    interval = StopLoadingTime - StartLoadingTime;
                    if (interval.TotalDays > 1 || interval.TotalDays < 0)
                    {
                        tempStr = "实际开始拉油时间StartLoadingTime：" + StartLoadingTime + ",实际结束拉油时间StopLoadingTime：" + StopLoadingTime + "时间顺序不合理";
                        log.Info(tempStr);
                        msg += tempStr + "\n";
                        resultObj.IsSuccess = false;
                        resultObj.Msg = msg;
                        return resultObj;
                    }

                    // 实际拉液量
                    float OilLoaded = jObjectWork.Value<float>("OilLoaded");
                    if (OilLoaded <= 0)
                    {
                        tempStr = "实际拉液量不合理：" + OilLoaded;
                        log.Info(tempStr);
                        msg += tempStr + "\n";
                        resultObj.IsSuccess = false;
                        resultObj.Msg = msg;
                        return resultObj;
                    }

                    // 接收到的二维码
                    string QrCode = jObjectWork.Value<string>("QrCode");
                    tempStr = "QrCode为：" + QrCode;
                    log.Info(tempStr);
                    msg += tempStr + "\n";

                    // 判断是否是工单二维码
                    if (QRCodeEncoder.IsWorkTicket(QrCode) == false )
                    {
                        tempStr = "工单二维码解析不正确：" + QrCode;
                        log.Info(tempStr);
                        msg += tempStr + "\n";
                        resultObj.IsSuccess = false;
                        resultObj.Msg = msg;
                        return resultObj;
                    }
                    string SerialNumber = QrCode.Split(":")[1];
                    tempStr = "解析出的工单编号为：" + SerialNumber;
                    log.Info(tempStr);
                    msg += tempStr + "\n";

                    // 根据工单编号和工单状态查询工单
                    DbContextOptions<H4WebContext> dbContextOption = new DbContextOptions<H4WebContext>();
                    DbContextOptionsBuilder<H4WebContext> dbContextOptionBuilder = new DbContextOptionsBuilder<H4WebContext>(dbContextOption);
                    H4WebContext _context = new H4WebContext(dbContextOptionBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreSql")).Options);
                    var obj = _context.WorkTicket.Where(p => p.SerialNumber.Equals(SerialNumber) && p.Status.Equals(WorkTicketStatus.待拉油.ToString())).FirstOrDefault();
                    if (obj == null)
                    {
                        tempStr = "满足条件的工单不存在（工单编号" + SerialNumber + ",状态待拉油）";
                        log.Info(tempStr);
                        msg += tempStr + "\n";
                        resultObj.IsSuccess = false;
                        resultObj.Msg = msg;
                        return resultObj;
                    }

                    // 如果工单存在则修改工单
                    obj.LoadingActualBeginTime = StartLoadingTime;
                    obj.LoadingActualEndTime = StopLoadingTime;
                    obj.OilLoaded = OilLoaded;
                    obj.OilLoader = obj.DriverPhone;

                    obj.Status = WorkTicketStatus.待卸油.ToString();
                    obj.Description = obj.Description + "\n【拉油】操作人：" + obj.DriverPhone + "，时间：" + DateTime.Now;

                    obj.LastUpdateTime = DateTime.Now;
                    obj.LastUpdateUser = obj.DriverPhone;

                    _context.WorkTicket.Update(obj);
                    _context.SaveChanges();

                    // 通过Grpc修改参数
                    SystemPara systemPara = new SystemPara();
                    systemPara.ParaName = "AckTaskComplete";
                    systemPara.ParaValue = "0";
                    // 拉油完成后，此变量会置1，需将拉油相关信息，如拉油时长、拉液量等保存后，清除此位
                    ResultObj setParamsResult = StationControlService.SetSystemSettingSingleParaService(systemPara);
                    if (setParamsResult.IsSuccess == false)
                    {
                        tempStr = "修改参数AckTaskComplete失败！";
                        log.Info(tempStr);
                        msg += tempStr + "\n";
                        resultObj.IsSuccess = false;
                        resultObj.Msg = msg;
                        return resultObj;
                    }
                   
                    tempStr = "自动处理工单成功。";
                    log.Info(tempStr);
                    msg += tempStr + "\n";
                    resultObj.IsSuccess = true;
                    resultObj.KeyInfo = obj.SerialNumber;
                    resultObj.Msg = msg;
                    return resultObj;
                }
                else
                {
                    tempStr = "不用处理工单。";
                    log.Info(tempStr);
                    msg += tempStr + "\n";
                    resultObj.IsSuccess = false;
                    resultObj.Msg = msg;
                    return resultObj;
                }
            }
            catch(Exception e)
            {
                tempStr = "自动处理工单发生异常：" + e.Message;
                log.Info(tempStr);
                msg += tempStr + "\n";
                resultObj.IsSuccess = false;
                resultObj.Msg = msg;
                return resultObj;
            }
        }

        // 自动处理二维码信息
        public ResultObj AutoOperQrCode(string jsonStrData)
        {
            ResultObj resultObj = new ResultObj();
            string msg = "";
            string tempStr;
            try
            {
                // 序列化为JObject
                JObject jObject = JsonConvert.DeserializeObject<JObject>(jsonStrData);
                if (jObject == null)
                {
                    tempStr = "序列化对象为空：" + jsonStrData;
                    log.Info(tempStr);
                    msg += tempStr + "\n";
                    resultObj.IsSuccess = false;
                    resultObj.Msg = msg;
                    return resultObj;
                }

                // 解析工单数据
                JObject jObjectWork = jObject.Value<JObject>("System");
                if (jObjectWork == null)
                {
                    tempStr = "序列化工单数据为空：" + jObject.Value<string>("System");
                    log.Info(tempStr);
                    msg += tempStr + "\n";
                    resultObj.IsSuccess = false;
                    resultObj.Msg = msg;
                    return resultObj;
                }

                // 接收到新的二维码，Web后台需在拿到二维码字符串后，清除此位
                int RecvNewQrCode = jObjectWork.Value<int>("RecvNewQrCode");
                tempStr = "RecvNewQrCode：" + RecvNewQrCode;
                log.Info(tempStr);
                msg += tempStr + "\n";
                if (RecvNewQrCode == 1)
                {
                    // 接收到的二维码
                    string QrCode = jObjectWork.Value<string>("QrCode");
                    tempStr = "QrCode为：" + QrCode;
                    log.Info(tempStr);
                    msg += tempStr + "\n";

                    // 获取操作等级
                    int operationLevel = QRCodeEncoder.GetOperationLevel(QrCode);
                    // 通过Grpc修改参数
                    SystemPara systemPara = new SystemPara();
                    systemPara.ParaName = "OperationLevel";
                    systemPara.ParaValue = operationLevel.ToString();
                    // 操作等级，需由Web后台根据识别二维码的结果后设置此变量，0无 1拉油工单 2操作员 3管理员
                    ResultObj setParamsResult = StationControlService.SetSystemSettingSingleParaService(systemPara);
                    if (setParamsResult.IsSuccess == false)
                    {
                        tempStr = "修改参数OperationLevel失败！";
                        log.Info(tempStr);
                        msg += tempStr + "\n";
                        resultObj.IsSuccess = false;
                        resultObj.Msg = msg;
                        return resultObj;
                    }

                    // 判断是否是工单二维码
                    if (QRCodeEncoder.IsWorkTicket(QrCode) == false)
                    {
                        tempStr = "工单二维码解析不正确：" + QrCode;
                        log.Info(tempStr);
                        msg += tempStr + "\n";
                        resultObj.IsSuccess = false;
                        resultObj.Msg = msg;
                        return resultObj;
                    }
                    string SerialNumber = QrCode.Split(":")[1];
                    tempStr = "解析出的工单编号为：" + SerialNumber;
                    log.Info(tempStr);
                    msg += tempStr + "\n";

                    // 根据工单编号和工单状态查询工单
                    DbContextOptions<H4WebContext> dbContextOption = new DbContextOptions<H4WebContext>();
                    DbContextOptionsBuilder<H4WebContext> dbContextOptionBuilder = new DbContextOptionsBuilder<H4WebContext>(dbContextOption);
                    H4WebContext _context = new H4WebContext(dbContextOptionBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreSql")).Options);
                    var obj = _context.WorkTicket.Where(p => p.SerialNumber.Equals(SerialNumber) && p.Status.Equals(WorkTicketStatus.待拉油.ToString())).FirstOrDefault();
                    if (obj == null)
                    {
                        tempStr = "满足条件的工单不存在（工单编号" + SerialNumber + ",状态待拉油）";
                        log.Info(tempStr);
                        msg += tempStr + "\n";
                        resultObj.IsSuccess = false;
                        resultObj.Msg = msg;
                        return resultObj;
                    }

                    // 通过Grpc修改参数
                    systemPara = new SystemPara();
                    systemPara.ParaName = "WorkTicketInfo";
                    systemPara.ParaValue = JsonConvert.SerializeObject(obj);
                    // Web后台将工单信息写入PLC，此字符串，0~49位为工单编号，50~59为司机姓名，60~69为车牌号，70~90为联系方式
                    setParamsResult = StationControlService.SetSystemSettingSingleParaService(systemPara);
                    if (setParamsResult.IsSuccess == false)
                    {
                        tempStr = "修改参数WorkTicketInfo失败！";
                        log.Info(tempStr);
                        msg += tempStr + "\n";
                        resultObj.IsSuccess = false;
                        resultObj.Msg = msg;
                        return resultObj;
                    }

                    // 通过Grpc修改参数
                    systemPara.ParaName = "WorkTicketDateTime";
                    systemPara.ParaValue = obj.LoadingBeginTime.ToString();
                    // 工单上的拉油时间
                    setParamsResult = StationControlService.SetSystemSettingSingleParaService(systemPara);
                    if (setParamsResult.IsSuccess == false)
                    {
                        tempStr = "修改参数WorkTicketDateTime失败！";
                        log.Info(tempStr);
                        msg += tempStr + "\n";
                        resultObj.IsSuccess = false;
                        resultObj.Msg = msg;
                        return resultObj;
                    }

                    // 通过Grpc修改参数
                    systemPara.ParaName = "TankSelect";
                    systemPara.ParaValue = obj.OilPot.Equals("1#罐") ? "1" : "2";
                    // 工单上的拉油时间
                    setParamsResult = StationControlService.SetSystemSettingSingleParaService(systemPara);
                    if (setParamsResult.IsSuccess == false)
                    {
                        tempStr = "修改参数TankSelect失败！";
                        log.Info(tempStr);
                        msg += tempStr + "\n";
                        resultObj.IsSuccess = false;
                        resultObj.Msg = msg;
                        return resultObj;
                    }

                    // 通过Grpc修改参数
                    systemPara.ParaName = "MaxOilCanLoad";
                    systemPara.ParaValue = obj.OilLoadedMax.ToString();
                    // 工单上的最大拉液量
                    setParamsResult = StationControlService.SetSystemSettingSingleParaService(systemPara);
                    if (setParamsResult.IsSuccess == false)
                    {
                        tempStr = "修改参数MaxOilCanLoad失败！";
                        log.Info(tempStr);
                        msg += tempStr + "\n";
                        resultObj.IsSuccess = false;
                        resultObj.Msg = msg;
                        return resultObj;
                    }

                    // 通过Grpc修改参数
                    systemPara.ParaName = "SendWorkTicket";
                    systemPara.ParaValue = "1";
                    // 如识别到二维码为工单信息后，将工单信息给PLC，置此位为1，由PLC侧清除此痊
                    setParamsResult = StationControlService.SetSystemSettingSingleParaService(systemPara);
                    if (setParamsResult.IsSuccess == false)
                    {
                        tempStr = "修改参数SendWorkTicket失败！";
                        log.Info(tempStr);
                        msg += tempStr + "\n";
                        resultObj.IsSuccess = false;
                        resultObj.Msg = msg;
                        return resultObj;
                    }

                    // 通过Grpc修改参数
                    systemPara.ParaName = "RecvNewQrCode";
                    systemPara.ParaValue = "0";
                    // 接收到新的二维码，Web后台需在拿到二维码字符串后，清除此位
                    setParamsResult = StationControlService.SetSystemSettingSingleParaService(systemPara);
                    if (setParamsResult.IsSuccess == false)
                    {
                        tempStr = "修改参数RecvNewQrCode失败！";
                        log.Info(tempStr);
                        msg += tempStr + "\n";
                        resultObj.IsSuccess = false;
                        resultObj.Msg = msg;
                        return resultObj;
                    }
                    
                    tempStr = "自动处理二维码信息完成。";
                    log.Info(tempStr);
                    msg += tempStr + "\n";
                    resultObj.IsSuccess = true;
                    resultObj.KeyInfo = obj.SerialNumber;
                    resultObj.Msg = msg;
                    return resultObj;
                }
                else
                {
                    tempStr = "没有新的二维码信息。";
                    log.Info(tempStr);
                    msg += tempStr + "\n";
                    resultObj.IsSuccess = false;
                    resultObj.Msg = msg;
                    return resultObj;
                }
            }
            catch (Exception e)
            {
                tempStr = "自动处理二维码发生异常：" + e.Message;
                log.Info(tempStr);
                msg += tempStr + "\n";
                resultObj.IsSuccess = false;
                resultObj.Msg = msg;
                return resultObj;
            }
        }

        // 自动提醒开工单
        public ResultObj AutoRemindToWork(string jsonStrData)
        {
            ResultObj resultObj = new ResultObj();
            string msg = "";
            string tempStr;
            try
            {
                // 序列化为JObject
                JObject jObject = JsonConvert.DeserializeObject<JObject>(jsonStrData);
                if (jObject == null)
                {
                    tempStr = "序列化对象为空：" + jsonStrData;
                    log.Info(tempStr);
                    msg += tempStr + "\n";
                    resultObj.IsSuccess = false;
                    resultObj.Msg = msg;
                    return resultObj;
                }

                // 解析1号罐数据
                JObject jObjectLT101 = jObject.Value<JObject>("LT101");
                if (jObjectLT101 == null)
                {
                    tempStr = "序列化1号罐数据为空：" + jObject.Value<string>("LT101");
                    log.Info(tempStr);
                    msg += tempStr + "\n";
                    resultObj.IsSuccess = false;
                    resultObj.Msg = msg;
                    return resultObj;
                }
                // 1号罐液位值
                float engValueLT101 = jObjectLT101.Value<float>("EngValue");
                tempStr = "1号罐液位值为：" + engValueLT101;
                log.Info(tempStr);
                msg += tempStr + "\n";

                // 解析2号罐数据
                JObject jObjectLT102 = jObject.Value<JObject>("LT102");
                if (jObjectLT101 == null)
                {
                    tempStr = "序列化2号罐数据为空：" + jObject.Value<string>("LT102");
                    log.Info(tempStr);
                    msg += tempStr + "\n";
                    resultObj.IsSuccess = false;
                    resultObj.Msg = msg;
                    return resultObj;
                }

                // 2号罐液位值
                float engValueLT102 = jObjectLT102.Value<float>("EngValue");
                tempStr = "2号罐液位值为：" + engValueLT102;
                log.Info(tempStr);
                msg += tempStr + "\n";

                // 获取拉油液位值
                string stationConfigJsonData = StationControlService.GetStationConfigJsonData();
                JObject jObjectstationConfig = JsonConvert.DeserializeObject<JObject>(stationConfigJsonData);
                if (jObjectstationConfig == null)
                {
                    tempStr = "序列化系统参数为空：" + stationConfigJsonData;
                    log.Info(tempStr);
                    msg += tempStr + "\n";
                    resultObj.IsSuccess = false;
                    resultObj.Msg = msg;
                    return resultObj;
                }
                float levelReadyForWork = jObjectstationConfig.Value<float>("levelReadyForWork");
                tempStr = "levelReadyForWork的值为：" + levelReadyForWork;
                log.Info(tempStr);
                msg += tempStr + "\n";

                if (levelReadyForWork < engValueLT101 && levelReadyForWork < engValueLT102)
                {
                    tempStr = "1号罐和1号罐液位已达拉油液位值，可以开工单了。";
                    log.Info(tempStr);
                    msg += tempStr + "\n";
                    resultObj.IsSuccess = true;
                    resultObj.KeyInfo = tempStr;
                    resultObj.Msg = msg;
                    return resultObj;
                }
                else if (levelReadyForWork < engValueLT101)
                {
                    tempStr = "1号罐液位已达拉油液位值，可以开工单了。";
                    log.Info(tempStr);
                    msg += tempStr + "\n";
                    resultObj.IsSuccess = true;
                    resultObj.KeyInfo = tempStr;
                    resultObj.Msg = msg;
                    return resultObj;
                }
                else if (levelReadyForWork < engValueLT102)
                {
                    tempStr = "2号罐液位已达拉油液位值，可以开工单了。";
                    log.Info(tempStr);
                    msg += tempStr + "\n";
                    resultObj.IsSuccess = true;
                    resultObj.KeyInfo = tempStr;
                    resultObj.Msg = msg;
                    return resultObj;
                }
                else
                {
                    tempStr = "罐液位未达拉油液位值。";
                    log.Info(tempStr);
                    msg += tempStr + "\n";
                    resultObj.IsSuccess = false;
                    resultObj.Msg = msg;
                    return resultObj;
                }
              
            }
            catch (Exception e)
            {
                tempStr = "自动提醒开工单发生异常：" + e.Message;
                log.Info(tempStr);
                msg += tempStr + "\n";
                resultObj.IsSuccess = false;
                resultObj.Msg = msg;
                return resultObj;
            }
        }
    }
}
