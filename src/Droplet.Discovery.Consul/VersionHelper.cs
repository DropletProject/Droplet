using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Droplet.Discovery.Consul
{
    public class VersionHelper
    {
        public const string VERSION_PREFIX = "version-";

        public static string GetVersionFromTags(IEnumerable<string> tags)
        {
            return tags
                ?.FirstOrDefault(x => x.StartsWith(VERSION_PREFIX, StringComparison.Ordinal))
                .Replace(VERSION_PREFIX, "");
        }

        public static List<string> SetVersionToTags(IEnumerable<string> tags,string version)
        {
            string versionLabel = GetVersion(version);
            var tagList = (tags ?? Enumerable.Empty<string>()).ToList();
            tagList.Add(versionLabel);
            return tagList;
        }

        public static string GetVersion(string version)
        {
            return $"{VERSION_PREFIX}{version}";
        }
    }
}
