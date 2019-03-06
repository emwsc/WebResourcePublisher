using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WebResourcePublisher
{
    class Configuration
    {
        public string buildPath { get; private set; }
        public Dictionary<string, string> webResourceMapping { get; private set; }

        public string connectionString { get; private set; }

        public Configuration(string configPath)
        {
            if (string.IsNullOrWhiteSpace(configPath)) throw new Exception("Empty config path");
            var jsonConfig = ReadConfigFile(configPath);
            SetConfig(jsonConfig);
        }

        private void SetConfig(string jsonText)
        {
            var jsonConfig = JObject.Parse(jsonText);
            connectionString = jsonConfig["connectionString"]?.ToString();
            if (string.IsNullOrEmpty(connectionString)) throw new Exception("Empty connection string");
            buildPath = jsonConfig["buildPath"]?.ToString();
            if (string.IsNullOrEmpty(buildPath)) throw new Exception("Empty build path");
            if ((jsonConfig["mapping"] as JArray) == null) throw new Exception("Empty mapping");
            webResourceMapping = new Dictionary<string, string>();
            foreach (var item in (JArray)jsonConfig["mapping"])
            {
                var fileName = item["fileName"]?.ToString();
                var webResourceName = item["webResourceName"]?.ToString();
                if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(webResourceName)) throw new Exception("Empty mapping configuration");
                webResourceMapping.Add(fileName, webResourceName);
            }
        }

        private string ReadConfigFile(string configPath)
        {
            var jsonText = File.ReadAllText(configPath, Encoding.Default);
            return jsonText;
        }
    }
}
