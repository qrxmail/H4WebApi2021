using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace NFCWebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UpdateManageController : ControllerBase
	{
		string APK_Directory = @"D:\CMTapks";
		[HttpGet]
		public IActionResult Get()
		{
			var files = Directory.GetFiles(APK_Directory);
			var latestVersion = files.Select(x => VersionHelper.GetVersionFromString(Path.GetFileName(x)))
				.OrderByDescending(x => x).FirstOrDefault();
			var physicalPath = files.FirstOrDefault(x => VersionHelper.GetVersionFromString(Path.GetFileName(x)) == latestVersion);
			var contentType = "application/vnd.android.package-archive";
			var fileName = Path.GetFileName(physicalPath);
			if (System.IO.File.Exists(physicalPath))
			{
				return PhysicalFile(physicalPath, contentType, fileName);
			}

			return BadRequest("File not exsit");
		}

		[HttpPost]
		public IActionResult Post([FromBody] string value)
		{
			var currentVersion = VersionHelper.GetVersionFromString(value);
			if (currentVersion == new Version(0, 0, 0))
			{
				return Ok(false);
			}

			var latestVersion = Directory.GetFiles(APK_Directory)
				.Select(x => VersionHelper.GetVersionFromString(Path.GetFileName(x)))
				.OrderByDescending(x => x).FirstOrDefault();
			return Ok(latestVersion > currentVersion);
		}
	}
}
