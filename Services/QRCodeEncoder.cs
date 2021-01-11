using System;
using System.Text;
using System.Security.Cryptography;
using H4WebApi.Models.Work;
using H4WebApi.Models;

namespace H4WebApi.Services
{
	public class QRCodeEncoder
	{
		const string salt = "CosMosSalt";

		// 用户二维码生成
		public static string EncoderUser(User user)
		{
			var createTime = DateTime.Now.ToString("yyyyMMddHHmmss");
			var text = user.UserName + createTime + salt;
			if (user.CurrentAuthority.Equals("admin"))
            {
				return "admin:" + user.UserName + ":" + createTime + ":" + GetEncrptResult(text);
			} else
            {
				return "operUser:" + user.UserName + ":" + createTime + ":" + GetEncrptResult(text);
			}
		}

		// 工单二维码生成
		public static string Encoder(WorkTicket ticket)
		{
			var createTime = ticket.CreateTime.ToString("yyyyMMddHHmmss");
			var text = ticket.SerialNumber + createTime + salt;
			return nameof(WorkTicket) + ":" + ticket.SerialNumber + ":" + createTime + ":" + GetEncrptResult(text);
		}

		// 信息加密
		static string GetEncrptResult(string text)
		{
			var md5 = new MD5CryptoServiceProvider();
			byte[] palindata = Encoding.Default.GetBytes(text);//将要加密的字符串转换为字节数组
			byte[] encryptdata = md5.ComputeHash(palindata);//将字符串加密后也转换为字符数组
			return Convert.ToBase64String(encryptdata).Substring(0, 10);//将加密后的字节数组转换为加密字符串
		}

		// 判断是否是合法的工单二维码
		public static bool IsWorkTicket(string text)
		{
			var fields = text.Split(':');
			if (fields[0] == nameof(WorkTicket) && fields.Length == 4)
			{
				var str = fields[1] + fields[2] + salt;
				if (GetEncrptResult(str) == fields[3])
				{
					return true;
				}
			}

			return false;
		}

		// 获取操作等级，需由Web后台根据识别二维码的结果后设置此变量，0无 1拉油工单 2操作员 3管理员
		// OperationLevel设置
		public static int GetOperationLevel(string text)
		{
			var fields = text.Split(':');
			if (fields[0].Equals(nameof(WorkTicket)))
			{
				return 1;
			}
			else if (fields[0].Equals("operUser")) 
			{
				return 2;
			}
			else if (fields[0].Equals("admin"))
			{
				return 3;
			}

			return 0;
		}
	}
}
