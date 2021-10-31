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
    "LockTypes": "InMemory",//可选值：InMemory、Redis
    "RedisEndPoints": ["localhost:6379"] //Redis节点
  }
}
```

## 分布式Id

nuget包：`Colder.DistributedId` 

使用方式
```c#
IHostBuilder.AddDistributedId()
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
