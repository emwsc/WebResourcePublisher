using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;

namespace WebResourcePublisher
{
    class Program
    {

        public static void Main(string[] args)
        {
            try
            {
                if (args == null || args.All(arg => arg != "-p"))
                {
                    Console.WriteLine("WebResourcePublisher. Empty args");
                    return;
                }
                var pathIndex = args.ToList().IndexOf("-p");
                if (args.Length < pathIndex + 1)
                {
                    Console.WriteLine("WebResourcePublisher. Path not set");
                    return;
                }
                var configPath = args[pathIndex + 1];
                if (string.IsNullOrWhiteSpace(configPath))
                {
                    Console.WriteLine("WebResourcePublisher. Path is empty");
                    return;
                }
                Console.WriteLine("WebResourcePublisher. Reading configuration file...");
                var config = new Configuration(configPath);
                Console.WriteLine("WebResourcePublisher. Connection to Dynamics...");
                var _client = new CrmServiceClient(config.connectionString);
                Console.WriteLine("WebResourcePublisher. Connection is established!");
                var dynamics = new Dynamics(_client, config);
                Console.WriteLine("WebResourcePublisher. Updating web resources...");
                dynamics.UpdateWebResources();
                Console.WriteLine("WebResourcePublisher. Publishing...");
                dynamics.Publish();
                Console.WriteLine("WebResourcePublisher. Success!");
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                string message = ex.Message;
                Console.WriteLine("WebResourcePublisher. Can't publish web resources. Error: " + message);
            }
            catch (Exception e)
            {
                string message = e.Message;
                Console.WriteLine("WebResourcePublisher. Something wrong with reading files. Error: " + message);
            }
        }
    }
}