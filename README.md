This is plugin that allows you to automatically publish web resources to Dynamics 365. It's useful when you compile your js code with webpack and want to upload and publish changes automatically into Dynamics 365.
How it works:

- Build project WebResourcePublisher in Visual Studio
- Import publish method from dynamics.js into your build pipeline script
- Call publish like:
  ```
  const publish = require("./dynamics").publish;
  publish(process.cwd());
  ```
  -In project root folder (near package.json) create publisher.json

```
{
  "publisherToolPath": "Path to exe WebResourcePublisher",
  "buildPath": "Full path to build folder",
  "connectionString": "Dynamics 365 connection string",
  "mapping": [
    {
      "fileName": "Filename in build folder",
      "webResourceName": "Web resource name"
    }
  ]
}

```
