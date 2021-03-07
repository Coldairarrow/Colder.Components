[![Build Status](https://coldairarrow.visualstudio.com/Colder/_apis/build/status/Colder.Components-ci?branchName=master)](https://coldairarrow.visualstudio.com/Colder/_build/latest?definitionId=3&branchName=master)
- [通用基础组件](#通用基础组件)
  - [日志](#日志)
  - [消息总线](#消息总线)
  - [分布式锁](#分布式锁)
  - [Orleans](#orleans)
# 通用基础组件
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
    "Concurrency":0//并发处理数,默认根据逻辑处理器数量自动分配
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

## Orleans
使用方式
```c#
IHostBuilder.ConfigureOrleansDefaults()
```
配置
```javascript
{
  "orleans": {
    "Provider": "InMemory", //可选值InMemory、AdoNet
    "AdoNetInvariant": "Microsoft.Data.SqlClient",
    "AdoNetConString": "Data Source=127.0.0.1;Initial Catalog=Orleans;User Id=sa;Password=123456;",
    "ClusterId": "", //集群Id，默认入口程序集名
    "ServiceId": "", //服务Id，默认入口程序集名
    "Ip": "", //本机Ip，默认自动扫描获取本机Ip
    "Port": 11111
  }
}
```