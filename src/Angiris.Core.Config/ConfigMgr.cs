using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Angiris.Core.Config
{
    public class ConfigMgr
    {
        private static dynamic _releaseCfg = null;
        public static dynamic ReleaseCfg
        {
            get
            {
                if (_releaseCfg == null)
                {
                    string fileName = "config.release.json";

                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uri.Path);
                    var binFolderPath = Path.GetDirectoryName(path);

                    fileName = Path.Combine(binFolderPath, fileName);

                    if (File.Exists(fileName))
                    {
                        string fileContent = File.ReadAllText(fileName);
                        _releaseCfg = JsonConvert.DeserializeObject(fileContent);
                    }
                    else
                    {
                        throw new FileNotFoundException("config.release.json not found");
                    }
                }
                return _releaseCfg;

            }
        }
    }
}
