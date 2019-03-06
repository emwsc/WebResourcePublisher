const spawn = require("child_process").spawn;
const fs = require("fs");

function runPublisher(webResourcePublisherPath, configPath) {
  const ipc = spawn(webResourcePublisherPath, ["-p", configPath]);
  ipc.stdin.setEncoding("utf8");
  ipc.stderr.on("data", function(data) {
    process.stdout.write(data.toString());
  });
  ipc.stdout.on("data", function(data) {
    process.stdout.write(data.toString());
  });
}

function publish(folderPath) {
  const configPath = folderPath + "/publisher.json";
  if (!fs.existsSync(configPath)) {
    console.log("Publisher config not found.");
    return;
  }
  fs.readFile(configPath, "utf8", function(err, data) {
    if (err) {
      console.log("Can't read publisher config.");
      return;
    }
    runPublisher(JSON.parse(data).publisherToolPath, configPath);
  });
}

module.exports.publish = publish;
