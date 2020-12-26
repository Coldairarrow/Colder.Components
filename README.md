# 用法详见Demo

## 日志
使用方式
```c#
IHostBuilder.ConfigureLoggingDefaults()
```
配置
```javascript
{
  "log": { //日志配置
    "minlevel": "Trace", //定义详见Microsoft.Extensions.Logging.LogLevel
    "console": {
      "enabled": true
    },
    "debug": {
      "enabled": true
    },
    "file": {
      "enabled": true
    },
    "elasticsearch": {
      "enabled": false,
      "nodes": [ "http://localhost:9200/" ],
      "indexformat": "Demo-Logging-{0:yyyyMM}"
    },
    "overrides": [ //重写日志输出级别
      {
        "source": "Microsoft.AspNetCore",
        "minlevel": "Information"
      }
    ]
  }
}

```
## 消息总线
使用方式
```c#
IHostBuilder.ConfigureMessageBusDefaults()
```
配置
```javascript
{
  "messagebus": {
    "Transport": "RabbitMQ",
    "Host": "amqp://localhost:5672/",
    "Username": "guest",
    "Password": "guest"
  }
}
```