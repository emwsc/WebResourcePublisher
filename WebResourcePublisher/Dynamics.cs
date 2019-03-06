using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq;

namespace WebResourcePublisher
{
    class Dynamics
    {

        private readonly IOrganizationService _service;
        private readonly Configuration _config;
        private List<Guid> webResourceIds;

        public Dynamics(IOrganizationService service, Configuration config)
        {
            _service = service;
            _config = config;
        }

        public void UpdateWebResources()
        {
            var files = GetFiles();
            if (files.Any()) webResourceIds = new List<Guid>();
            foreach (var filePath in files)
            {
                var fileName = filePath.Split('\\').Last();
                if (!_config.webResourceMapping.TryGetValue(fileName, out var webResourceName)) continue;
                var webResourceId = FindWebResourceId(webResourceName);
                var fileContent = File.ReadAllBytes(filePath);
                UpdateWebResource(webResourceId, fileContent);
                webResourceIds.Add(webResourceId);
            }
        }

        private void UpdateWebResource(Guid webResourceId, byte[] fileContent)
        {
            var webresource = new Entity("webresource", webResourceId) { ["content"] = Convert.ToBase64String(fileContent) };
            _service.Update(webresource);
        }

        private Guid FindWebResourceId(string webResourceName)
        {
            var query = new QueryExpression("webresource");
            query.Criteria.AddCondition("name", ConditionOperator.Equal, webResourceName);
            var result = _service.RetrieveMultiple(query).Entities;
            if (result.Count > 1) throw new Exception($"Found {result.Count} web resources with name '{webResourceName}'");
            if (!result.Any()) throw new Exception($"Web resource with name {webResourceName} not found");
            return result.First().Id;
        }

        private string[] GetFiles() => Directory.GetFiles(_config.buildPath);

        public void Publish()
        {
            if (webResourceIds == null || !webResourceIds.Any()) return;
            OrganizationRequest request = new OrganizationRequest
            {
                RequestName = "PublishXml",
                Parameters = new ParameterCollection
                {
                    new KeyValuePair<string, object>("ParameterXml",
                        $"<importexportxml><webresources>{string.Join("", webResourceIds.Select(a => $"<webresource>{a}</webresource>"))}</webresources></importexportxml>")
                }
            };
            _service.Execute(request);
        }
    }
}
