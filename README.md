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
    "Password": "guest",
    "RetryCount":3, //失败重试次数
    "RetryIntervalMilliseconds":1000, //失败重试间隔毫秒数
    "Endpoint":"Endpoint001",//指定节点名,默认为程序入口程序集名
  }
}
```

## 分布式锁
使用方式
```c#
IHostBuilder.ConfigureDistributedLockDefaults()
```
配置
```javascript
{
  "distributedLock": {
    "LockTypes": "InMemory",//可选值：InMemory、Redis
    "RedisEndPoints": ["localhost:6379"] //Redis节点
  }
}
```