using Grpc.Core;
using log4net;
using H4WebApi.Models;
using System.Reflection;
using static gRPC;

namespace H4WebApi.Services
{
    public class StationControlService
    {
        private static ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(StationControlService));

        const string GrpcChannelAddress = "localhost:12222";

        // 通过Grpc服务获取站点Json字符串数据
        public static string GetStationJsonData()
        {
            string jsonStrData;
            //try
            //{
            //    var channel = new Channel(GrpcChannelAddress, ChannelCredentials.Insecure);
            //    var client = new gRPCClient(channel);
            //    StationData data = client.GetStationData(new StationName { Name = "1" });
            //    jsonStrData = data.DataJson;
            //}
            //catch (Exception)
            //{
            //    jsonStrData = "{}";
            //}

            // 测试数据
            jsonStrData = "{\"LT101\":{\"IsAlarmHi\":false,\"IsAlarmHiHi\":false,\"IsAlarmLow\":false,\"IsAlarmLowLow\":false,\"Raw\":4696,\"EngValue\":475.578674}," +
                "\"LT102\":{\"IsAlarmHi\":false,\"IsAlarmHiHi\":false,\"IsAlarmLow\":false,\"IsAlarmLowLow\":false,\"Raw\":14986,\"EngValue\":1517.67944}," +
                "\"Pump101\":{\"State\":false,\"Start\":false,\"StartCondition\":true,\"Stop\":false,\"StopCondition\":true,\"Fault\":false,\"Feedback\":false,\"LocalRemote\":false}," +
                "\"Pump102\":{\"State\":false,\"Start\":false,\"StartCondition\":true,\"Stop\":false,\"StopCondition\":true,\"Fault\":false,\"Feedback\":false,\"LocalRemote\":false}," +
                "\"LoopValve01\":{\"OpenOutput\":false,\"Start\":false,\"StartCondition\":true,\"Stop\":false,\"StopCondition\":true,\"Fault\":false,\"Opened\":true,\"Closed\":false,\"OpenFailed\":false,\"CloseFailed\":false,\"CloseOutput\":false}," +
                "\"LoopValve02\":{\"OpenOutput\":false,\"Start\":false,\"StartCondition\":true,\"Stop\":false,\"StopCondition\":true,\"Fault\":false,\"Opened\":true,\"Closed\":false,\"OpenFailed\":false,\"CloseFailed\":false,\"CloseOutput\":false}," +
                "\"OutValve01\":{\"OpenOutput\":false,\"Start\":false,\"StartCondition\":true,\"Stop\":false,\"StopCondition\":true,\"Fault\":false,\"Opened\":false,\"Closed\":true,\"OpenFailed\":false,\"CloseFailed\":false,\"CloseOutput\":false}," +
                "\"OutValve02\":{\"OpenOutput\":false,\"Start\":false,\"StartCondition\":true,\"Stop\":false,\"StopCondition\":true,\"Fault\":false,\"Opened\":false,\"Closed\":true,\"OpenFailed\":false,\"CloseFailed\":false,\"CloseOutput\":false}," +
                "\"Heater01\":{\"State\":false,\"Start\":false,\"StartCondition\":true,\"Stop\":false,\"StopCondition\":true,\"Fault\":false,\"Feedback\":false,\"LocalRemote\":false,\"Mode\":false}," +
                "\"Heater02\":{\"State\":false,\"Start\":false,\"StartCondition\":true,\"Stop\":false,\"StopCondition\":true,\"Fault\":false,\"Feedback\":false,\"LocalRemote\":false,\"Mode\":false}," +
                "\"EstopButton\":{\"State\":false},\"StopOilInjectionButton\":{\"State\":false},\"Heater01Gauge\":{\"CommError\":false,\"Alarm\":0,\"SP\":0,\"SPSet\":0}," +
                "\"Heater02Gauge\":{\"CommError\":false,\"Alarm\":0,\"SP\":0,\"SPSet\":0}," +

                "\"System\":{\"Mode\":2,\"Alarm\":0,\"AckTaskComplete\":1,\"OperationLevel\":0," +
                "\"LevelBeginWork\":0.0,\"LevelAfterWork\":0.0,\"OilLoaded\":100.0,\"TaskDuration\":0.0," +
                "\"RecvNewQrCode\":1,\"SendWorkTicket\":0,\"QrCode\":\"WorkTicket:20201218162742:20201218162857:TBy/qt4gon\",\"WorkTicketInfo\":\"\"," +
                "\"WorkTicketDateTime\":\"2020 - 12 - 18T11: 10:58\",\"TankSelect\":0,\"MaxOilCanLoad\":0.0," +
                "\"StartLoading\":0.0,\"StopLoading\":0," +
                "\"StartLoadingTime\":\"2020 - 12 - 22T18: 00:00\",\"StopLoadingTime\":\"2020 - 12 - 22T18: 13:00\"}}";

            return jsonStrData;
        }

        // 通过Grpc服务获取站点配置数据
        public static string GetStationConfigJsonData()
        {
            string jsonStrData;
            //try
            //{
            //    var channel = new Channel(GrpcChannelAddress, ChannelCredentials.Insecure);
            //    var client = new gRPCClient(channel);
            //    StationConfigData data = client.GetStationConfigData(new StationName { Name = "1" });
            //    jsonStrData = data.DataJson;

            //    // 将json字符串序列化为CmsSystemSetting对象
            //    //CmsSystemSetting cmsSystem = JsonConvert.DeserializeObject<CmsSystemSetting>(data.DataJson);
            //}
            //catch (Exception)
            //{
            //    jsonStrData = "{}";
            //}

            // 测试数据
            jsonStrData = "{\"delayToTurnOffPump\":1,\"levelStopPumpEnable\":\"是\"," +
                   "\"maxDurationPerTask\":1,\"levelReadyForWork\":1," +
                   "\"levelStopPump\":1,\"pumpFloatRate\":1," +
                   "\"areaPerMM\":1,\"valveOpTimeOut\":1}";

            return jsonStrData;
        }

        // 设备指令发送
        public static ControlResult DeviceControlService(ControlModel model)
        {
            ControlResult result = new ControlResult();
            var channel = new Channel(GrpcChannelAddress, ChannelCredentials.Insecure);
            var client = new gRPCClient(channel);
            switch (model.Name)
            {
                case "TurnOnLoopValve01":
                    result = client.TurnOnLoopValve01(model.WriteCommand);
                    break;
                case "TurnOffLoopValve01":
                    result = client.TurnOffLoopValve01(model.WriteCommand);
                    break;
                case "TurnOnLoopValve02":
                    result = client.TurnOnLoopValve02(model.WriteCommand);
                    break;
                case "TurnOffLoopValve02":
                    result = client.TurnOffLoopValve02(model.WriteCommand);
                    break;
                case "TurnOnOutValve01":
                    result = client.TurnOnOutValve01(model.WriteCommand);
                    break;
                case "TurnOffOutValve01":
                    result = client.TurnOffOutValve01(model.WriteCommand);
                    break;
                case "TurnOnOutValve02":
                    result = client.TurnOnOutValve02(model.WriteCommand);
                    break;
                case "TurnOffOutValve02":
                    result = client.TurnOffOutValve02(model.WriteCommand);
                    break;
                case "TurnOnPump01":
                    result = client.TurnOnPump01(model.WriteCommand);
                    break;
                case "TurnOffPump01":
                    result = client.TurnOffPump01(model.WriteCommand);
                    break;
                case "TurnOnPump02":
                    result = client.TurnOnPump02(model.WriteCommand);
                    break;
                case "TurnOffPump02":
                    result = client.TurnOffPump02(model.WriteCommand);
                    break;
                case "OperateDevice":
                    result = client.OperateDevice(model.WriteCommand);
                    break;
                default:
                    break;
            }
            return result;
        }

        // 系统参数设置
        public static void SetSystemSettingParaService(CmsSystemSetting obj)
        {
            var channel = new Channel(GrpcChannelAddress, ChannelCredentials.Insecure);
            var client = new gRPCClient(channel);

            foreach (PropertyInfo p in obj.GetType().GetProperties())
            {
                object value = p.GetValue(obj);
                // 处理是否
                if (value != null && p.PropertyType == typeof(string))
                {
                    if (value.ToString().Equals("是"))
                    {
                        value = 1;
                    }
                    else if (value.ToString().Equals("否"))
                    {
                        value = 0;
                    }
                    SystemPara systemPara = new SystemPara();
                    systemPara.ParaName = p.Name;
                    systemPara.ParaValue = value.ToString();
                    ControlResult result = client.setSystemSettingPara(systemPara);

                    // 日志输出
                    if (result.Result.Equals("1"))
                    {
                        log.Info($"参数设置成功：{systemPara.ParaName},{systemPara.ParaValue}");
                    }
                    else
                    {
                        log.Info($"参数设置失败：{systemPara.ParaName},{systemPara.ParaValue}");
                    }
                }
            }
        }

        // 单个参数设置
        public static ResultObj SetSystemSettingSingleParaService(SystemPara systemPara)
        {
            ResultObj resultObj = new ResultObj();

            //var channel = new Channel(GrpcChannelAddress, ChannelCredentials.Insecure);
            //var client = new gRPCClient(channel);
            //ControlResult result = client.setSystemSettingPara(systemPara);

            //// 日志输出
            //if (result.Result.Equals("1"))
            //{
            //    log.Info($"参数设置成功：{systemPara.ParaName},{systemPara.ParaValue}");
            //    resultObj.IsSuccess = true;
            //}
            //else
            //{
            //    log.Info($"参数设置失败：{systemPara.ParaName},{systemPara.ParaValue}");
            //    resultObj.IsSuccess = false;
            //}

            // 测试数据
            resultObj.IsSuccess = true;
            return resultObj;
        }
    }

    // 设备控制指令下发参数
    public class ControlModel
    {
        public string Name { get; set; }
        public WriteCommand WriteCommand { get; set; }
    }

    // 系统参数设置
    public class CmsSystemSetting
    {
        public float DelayToTurnOffPump { get; set; } //延时关装车泵时间，单位为秒
        public string LevelStopPumpEnable { get; set; } //根据液位自动停泵，是否启用
        public float MaxDurationPerTask { get; set; } //单次拉油最大时长
        public float LevelReadyForWork { get; set; } //液位达到此值时，应提醒开具工单
        public float LevelStopPump { get; set; } //停泵液位
        public float PumpFloatRate { get; set; } //泵流速
        public float AreaPerMM { get; set; } //罐单位mm面积，计算液量使用
        public float ValveOpTimeOut { get; set; } //阀动作超时时间设置，单位为秒

    }
}
