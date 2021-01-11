using NFCWebApi.Models;
using System;
using System.Collections.Generic;

namespace H4WebApi.Models
{
    /// <summary>
    /// 前端列表数据的通用参数
    /// </summary>
    public class TableData
    {
        public int Total { get; set; }
        public bool Success { get; set; }
        public int PageSize { get; set; }
        public int Current { get; set; }
        public dynamic Data { get; set; }
    }

    /// <summary>
    /// 用来接收删除参数
    /// </summary>
    public class DelObj
    {
        public List<Guid> Id { get; set; }
    }

    public class ResultObj
    {
        public bool IsSuccess { get; set; }
        public string ErrMsg { get; set; }
        public string Msg { get; set; }
        public string KeyInfo { get; set; }
    }

    public class InspectTaskView : InspectTask
    {
        public string InspectName { get; set; }
        public string DeviceName { get; set; }
        public string InspectItemName { get; set; }
        public string NfcCardNo { get; set; }
        public string Unit { get; set; }
        public string ItemRemark { get; set; }

        public int SumCount { get; set; }
        public int IsCompleteCount { get; set; }
    }

}
