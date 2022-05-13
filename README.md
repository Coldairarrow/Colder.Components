[![Build Status](https://coldairarrow.visualstudio.com/Colder/_apis/build/status/Colder.Components-ci?branchName=master)](https://coldairarrow.visualstudio.com/Colder/_build/latest?definitionId=3&branchName=master)
- [通用基础组件](#通用基础组件)
  - [日志](#日志)
  - [分布式缓存](#分布式缓存)
  - [分布式锁](#分布式锁)
  - [分布式Id](#分布式id)
  - [自动注入](#自动注入)
  - [消息总线](#消息总线)
  - [Orleans](#orleans)
  - [OpenService(RPC调用)](#openservicerpc调用)
  - [WebSocket服务端](#websocket服务端)
# 通用基础组件
**完整使用案例见源码中demos**
## 日志

nuget包：`Colder.Logging.Serilog`

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
      "nodes": [ "http://elastic:123456@localhost:9200/" ],
      "indexformat": "log-{0:yyyyMMdd}"
    },
    "kafka": {
      "enabled": false,
      "brokers": "192.168.56.201:9092",
      "userName": "user",
      "password": "bitnami",
      "topic": "log"
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
## 分布式缓存

nuget包：`Colder.Cache`

使用方式
```c#
IHostBuilder.ConfigureCacheDefaults()
```
配置
```javascript
{
  "cache": {
    "CacheType": "InMemory",//可选值：InMemory、Redis
    "RedisConnectionString":"localhost:6379,password=123456"//Redis连接字符串,格式按照https://stackexchange.github.io/StackExchange.Redis/Configuration.html
  }
}
```

## 分布式锁

nuget包：
- `Colder.DistributedLock.Hosting` 
- `Colder.DistributedLock.InMemory` 
- `Colder.DistributedLock.Redis`

使用方式
```c#
IHostBuilder.ConfigureDistributedLockDefaults()
```
配置
```javascript
{
  "distributedLock": {
    "LockType": "InMemory",//可选值：InMemory、Redis
    "RedisEndPoints": ["localhost:6379"] //Redis节点
  }
}
```

## 分布式Id

nuget包：`Colder.DistributedId` 

使用方式
```c#
IHostBuilder.ConfigureDistributedIdDefaults()
```

```javascript
{
  "distributedid": {
    "Distributed":false,//是否为分布式(即多实例部署),若开启则需要提前配置分布式缓存(Colder.Cache)与分布式锁(Colder.DistributedLock),多实例部署并且使用LongId(即雪花Id)时建议开启此选项
    "GuidType": "AtBegin",//GUID序列类型，可选值：AtBegin、AtEnd，默认为AtBegin，建议SQLServer配置为AtEnd，其余数据库配置为AtBegin
    "WorkderId": 0 //指定机器Id,范围1-1023,若不指定则在范围内随机取
  }
}
```

## 自动注入
nuget包：`Colder.Dependency`

服务必须继承ITransientDependency、IScopedDependency或ISingletonDependency

使用方式
```c#
IServiceCollection.AddServices()
```

## 消息总线

nuget包：
- `Colder.MessageBus.Hosting` 
- `Colder.MessageBus.InMemory` 
- `Colder.MessageBus.RabbitMQ`
- `Colder.MessageBus.MQTT`
- `Colder.MessageBus.Redis`

使用方式
```c#
IHostBuilder.ConfigureMessageBusDefaults()
```
配置
```javascript
{
  "messagebus": {
    "Transport": "RabbitMQ",//可选值:InMemory,RabbitMQ,MQTT,Redis
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

## Orleans

nuget包：`Colder.Orleans.Hosting`

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

## OpenService(RPC调用)

nuget包：
- `Colder.OpenService.Abstractions`
- `Colder.OpenService.Client`
- `Colder.OpenService.Hosting`

接口定义
```c#
/// <summary>
    /// 
    /// </summary>
    [Route("hello")]
    public interface IHelloOpenService : IOpenService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idInput"></param>
        /// <returns></returns>
        [Route("say")]
        Task<string> SayHello(IdInput<string> idInput);
    }
```

服务端
```c#
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((host, services) =>
                {
                    services.AddOpenServiceClient(Assembly.GetEntryAssembly(), new OpenServiceOptions
                    {
                        BaseUrl="http://localhost:5000/api/"
                    });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
```

客户端
```c#
        static async Task Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddOpenServiceClient(typeof(IHelloOpenService).Assembly, new OpenServiceOptions
            {
                BaseUrl = "http://localhost:5000/api/"
            });
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            IHelloOpenService helloOpenService = serviceProvider.GetService<IHelloOpenService>();
            var response = await helloOpenService.SayHello(new IdInput<string> { Id = "Hello World" });
            Console.WriteLine($"请求成功 返回参:{response}");

            Console.ReadLine();
        }
```

## WebSocket服务端

nuget包：`Colder.WebSockets.Server`

使用方式
```c#
services.AddWebSocketServer(x =>
{
    x.OnConnected = async (serviceProvider, connection) =>
    {
        connection.Id = DateTime.Now.ToString();
        await Task.CompletedTask;
    };
    x.OnReceive = async (serviceProvider, connection, msg) =>
    {
        var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger(GetType());
        logger.LogInformation("收到来自 {Id} 的消息:{Msg}", connection.Id, msg);
        await Task.CompletedTask;
    };
});

app.UseWebSocketServer();//放到最前面
```
