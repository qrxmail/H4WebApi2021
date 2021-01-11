using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace H4WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OcrController : ControllerBase
    {
        [HttpGet("{id}")]
        public string OcrPic(string id)
        {
            //AK/SK
            
            var API_KEY = "szC3Ne2fkbFD09wxqEVVki4s";
            var SECRET_KEY = "zBdAmT3y7U3OX61PcVr9nZgwwk8e5ryg";

            var client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
 

            var image = System.IO.File.ReadAllBytes("D:\\excel\\"+ id + ".jpg");
            var url = "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1564654456007&di=7832dd6f515e654bdf5074e47b6803b1&imgtype=0&src=http%3A%2F%2Fpic.962.net%2Fup%2F2018-5%2F2018527102938219310.jpg";

            // 调用通用文字识别, 图片参数为本地图片，可能会抛出网络等异常，请使用try/catch捕获
            //用户向服务请求识别某张图中的所有文字
            var result = client.GeneralBasic(image);        //本地图图片
            //var result = client.GeneralBasicUrl(url);     //网络图片
            //var result = client.Accurate(image);          //本地图片：相对于通用文字识别该产品精度更高，但是识别耗时会稍长。

            //var result = client.General(image);           //本地图片：通用文字识别（含位置信息版）
            //var result = client.GeneralUrl(url);          //网络图片：通用文字识别（含位置信息版）

            //var result = client.GeneralEnhanced(image);   //本地图片：调用通用文字识别（含生僻字版）
            //var result = client.GeneralEnhancedUrl(url);  //网络图片：调用通用文字识别（含生僻字版）

            //var result = client.WebImage(image);          //本地图片:用户向服务请求识别一些背景复杂，特殊字体的文字。
            //var result = client.WebImageUrl(url);         //网络图片:用户向服务请求识别一些背景复杂，特殊字体的文字。

            //Console.WriteLine(result);
            return result.ToString();

        }
    }
}
