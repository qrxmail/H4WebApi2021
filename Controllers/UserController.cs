using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using H4WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using H4WebApi.Services;

namespace H4WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly H4WebContext _context;

        public UserController(H4WebContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取当前登录用户
        /// </summary>
        /// <returns></returns>
        [Route("currentUser")]
        [HttpGet]
        public ActionResult<User> GetCurrentUser()
        {
            return CommonService.GetCurrentUser(HttpContext);
        }

        /// <summary>
        /// 登录功能
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <returns></returns>
        [Route("login/account")]
        [HttpPost]
        public ActionResult<User> Login(User loginInfo)
        {
            User loginUser = new User();
            List<User> userList = _context.User.Where(p => p.UserName.Equals(loginInfo.UserName) && p.Password.Equals(loginInfo.Password)).ToList();
            if (userList.Count > 0)
            {
                loginUser = userList.First();
                loginUser.Status = "ok";
                loginUser.Type = "account";
                var loginUserBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(loginUser));
                HttpContext.Session.Set("currentUser", loginUserBytes);
            }

            else
            {
                loginUser.Status = "error";
                loginUser.Type = "account";
                loginUser.CurrentAuthority = "guest";
            }
            return loginUser;
        }

        /// <summary>
        /// 登出功能
        /// </summary>
        /// <returns></returns>
        [Route("login/out")]
        [HttpGet]
        public ActionResult<User> LoginOut()
        {
            User loginUser = new User();
            loginUser.Status = "ok";
            HttpContext.Session.Remove("currentUser");
            return loginUser;
        }

        // 带多个查询条件的查询
        [Route("user/query")]
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
            string userName = jObject.Value<string>("userName");
            string userId = jObject.Value<string>("userId");
           
            //防止查询条件都不满足，先生成一个空的查询
            var where = _context.User.Where(p => true);

            if (string.IsNullOrEmpty(userName) == false)
            {
                where = where.Where(p => p.UserName.Contains(userName));
            }

            if (string.IsNullOrEmpty(userId) == false)
            {
                where = where.Where(p => p.UserId.ToString().Equals(userId));
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
                if (sorterKey.Equals("userName") && sortRule.Equals("descend"))
                {
                    where = where.OrderByDescending(p => p.UserName);
                }
                else if (sorterKey.Equals("userName") && sortRule.Equals("ascend"))
                {
                    where = where.OrderBy(p => p.UserName);
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
            List<User> dataList = where.Take(pageSize).ToList();

            TableData resultObj = new TableData();
            resultObj.Data = dataList;
            resultObj.Current = current;
            resultObj.Success = true;
            resultObj.PageSize = pageSize;
            resultObj.Total = total;

            return resultObj;
        }

        // 根据id查询
        [Route("user/getbyid")]
        [HttpGet(Name = "GetUserObj")]
        public ActionResult<User> GetById(Guid gid)
        {
            var item = _context.User.Find(gid);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        // 判断是否存在相同登录名的用户
        public bool IsExistSame(User obj)
        {
            var where = _context.User.Where(p => p.UserName == obj.UserName);
            if (obj.UserId != null)
            {
                where = where.Where(p => p.UserId != obj.UserId);
            }
            List<User> list = where.ToList();
            if (list.Count > 0)
            {
                return true;
            }
            return false;
        }

        // 新增
        [Route("user/add")]
        [HttpPost]
        public ResultObj Add(User obj)
        {
            // 获取当前登录用户名
            string _currentUserName = CommonService.GetCurrentUser(HttpContext).UserName;
            ResultObj resultObj = new ResultObj();
            if (IsExistSame(obj))
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "该登录名已存在。";
                return resultObj;
            }

            obj.CreateUser = _currentUserName;
            obj.CreateTime = DateTime.Now;
            obj.LastUpdateUser = _currentUserName;
            obj.LastUpdateTime = DateTime.Now;

            _context.User.Add(obj);
            _context.SaveChanges();

            resultObj.IsSuccess = true;
            return resultObj;
        }

        // 修改
        [Route("user/update")]
        [HttpPost]
        public ResultObj Update(User newObj)
        {
            // 获取当前登录用户名
            string _currentUserName = CommonService.GetCurrentUser(HttpContext).UserName;
            ResultObj resultObj = new ResultObj();

            var obj = _context.User.Find(newObj.UserId);
            if (obj == null)
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "修改对象不存在。";
                return resultObj;
            }

            if (IsExistSame(newObj))
            {
                resultObj.IsSuccess = false;
                resultObj.ErrMsg = "该登录名已存在。";
                return resultObj;
            }

            obj.UserName = newObj.UserName;
            obj.Password = newObj.Password;
            obj.Branch = newObj.Branch;
            obj.Name = newObj.Name;
            obj.Type = newObj.Type;
            obj.Avatar = newObj.Avatar;
            obj.Email = newObj.Email;
            obj.Mobile = newObj.Mobile;
            obj.Status = newObj.Status;
            obj.CurrentAuthority = newObj.CurrentAuthority;
            obj.Remark = newObj.Remark;

            obj.LastUpdateTime = DateTime.Now;
            obj.LastUpdateUser = _currentUserName;

            _context.User.Update(obj);
            _context.SaveChanges();

            resultObj.IsSuccess = true;
            return resultObj;
        }

        // 删除
        [Route("user/delete")]
        [HttpPost]
        public IActionResult Delete(DelObj delObj)
        {
            // 获取当前登录用户名
            string _currentUserName = CommonService.GetCurrentUser(HttpContext).UserName;
            for (int i = 0; i < delObj.Id.Count(); i++)
            {
                var obj = _context.User.Find(delObj.Id[i]);
                if (obj == null)
                {
                    return NotFound();
                }
                // 系统管理员或者自己不可以删除
                if (!(obj.UserName.Equals("admin") || obj.UserName.Equals(_currentUserName)))
                {
                    _context.User.Remove(obj);
                    _context.SaveChanges();
                }
            }

            return NoContent();
        }

        // 生成二维码信息
        [Route("user/getqrcode")]
        [HttpGet]
        public ActionResult<string> GetQRCode(Guid userId)
        {
            var item = _context.User.Find(userId);
            if (item == null)
            {
                return NotFound();
            }
            string result = QRCodeEncoder.EncoderUser(item);
            return result;
        }

        /// <summary>
        /// 获取通知
        /// </summary>
        /// <returns></returns>
        [Route("notices")]
        [HttpGet]
        public dynamic GetNotices()
        {
            var list = new JArray();
            var data = new
            {
                id = "000000001",
                title = "任务名称",
                description = "任务需要在 2020-12-12 20:00 前启动",
                extra = "未开始",
                status = "todo",
                type = "event"
            };
            var str = JsonConvert.SerializeObject(data);
            var obj = JObject.Parse(str);
            obj["key"] = 1;
            obj.Add("key2", "1");
            list.Add(obj);
            return list.ToString();
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        [Route("system/menu")]
        [HttpGet]
        public ActionResult<MenuData> GetMenu()
        {
            MenuItem item1 = new MenuItem();
            item1.authority = new string[] { "admin", "user" };
            item1.component = "./Device";
            item1.name = "设备管理";
            item1.path = "/base/device";
            List<MenuItem> routes = new List<MenuItem>();
            routes.Add(item1);

            MenuItem menuData = new MenuItem();
            menuData.name = "基础设置";
            menuData.path = "/base";
            menuData.routes = routes;

            List<MenuItem> menuDatas = new List<MenuItem>();
            menuDatas.Add(menuData);

            MenuData menus = new MenuData();
            menus.menuData = menuDatas;
            return menus;
        }
    }

    /// <summary>
    /// 菜单模型
    /// </summary>
    public class MenuData
    {
        public List<MenuItem> menuData { get; set; }
    }

    /// <summary>
    /// 菜单项
    /// </summary>
    public class MenuItem
    {
        public string path { get; set; }
        public string name { get; set; }
        public string component { get; set; }
        public string[] authority { get; set; }
        public List<MenuItem> routes { get; set; }
    }



}
