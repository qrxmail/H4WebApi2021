using System;
using System.Text.RegularExpressions;

namespace NFCWebApi.Controllers
{
    public class VersionHelper
    {
		public static Version GetVersionFromString(string versionString)
		{
			var regexPattern = @"([v,V])(?<major>\d{1,2})[\.|\:](?<minor>\d{1,2})[\.|\:](?<revision>\d{1,3})";
			var regex = new Regex(regexPattern);
			try
			{
				var match = regex.Match(versionString);
				var major = Convert.ToByte(match.Groups["major"].Value);
				var minor = Convert.ToByte(match.Groups["minor"].Value);
				var revision = Convert.ToByte(match.Groups["revision"].Value);

				return new Version(major, minor, revision);
			}
			catch
			{
			}

			return new Version(0, 0, 0);
		}
	}
}
