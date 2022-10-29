using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.Mail;

/// <summary>
/// 
/// </summary>
public class IdleClient : IDisposable
{
    //流程：建立连接=>登录=>绑定变化事件=>开启后台线程一直Idle
    CancellationTokenSource _cancel;
    CancellationTokenSource _done;
    ImapClient _client;

    /// <summary>
    /// 
    /// </summary>
    public readonly string Host;

    /// <summary>
    /// 
    /// </summary>
    public readonly int Port;

    /// <summary>
    /// 
    /// </summary>
    public readonly string UserName;

    /// <summary>
    /// 
    /// </summary>
    public readonly string Password;
    private readonly ILogger _logger;
    private List<UniqueId> _existsUids;

    /// <summary>
    /// 
    /// </summary>
    public Func<UniqueId[], Task> OnNewMessage { get; set; }
    private readonly BlockingCollection<UniqueId[]> _newMessageQueue = new BlockingCollection<UniqueId[]>();
    private readonly Task _idleTask;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="loggerFactory"></param>
    public IdleClient(string host, int port, string userName, string password, ILoggerFactory loggerFactory = null)
    {
        _client = new ImapClient();

        _cancel = new CancellationTokenSource();

        Host = host;
        Port = port;
        UserName = userName;
        Password = password;
        _logger = loggerFactory?.CreateLogger<IdleClient>();

        _idleTask = Task.Factory.StartNew(async () =>
        {
            foreach (var uids in _newMessageQueue.GetConsumingEnumerable())
            {
                try
                {
                    await OnNewMessage(uids);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, ex.Message);
                }
            }
        });
    }

    private async Task ConnectAsync()
    {
        _logger?.LogInformation("IdleClient开始连接 {Host}:{Port} {UserName}", Host, Port, UserName);

        if (!_client.IsConnected)
        {
            await _client.ConnectAsync(Host, Port, SecureSocketOptions.Auto, _cancel.Token);

            var clientImplementation = new ImapImplementation
            {
                Name = GetType().FullName,
                Version = "1.0.0"
            };
            _client.Identify(clientImplementation);
        }

        if (!_client.IsAuthenticated)
        {
            await _client.AuthenticateAsync(UserName, Password, _cancel.Token);

            await _client.Inbox.OpenAsync(FolderAccess.ReadOnly, _cancel.Token);
            _existsUids ??= (await GetAllUids()).ToList();
        }

        _logger?.LogInformation("IdleClient连接成功 {Host}:{Port} {UserName}", Host, Port, UserName);
    }
    private async Task<UniqueId[]> GetAllUids()
    {
        return (await _client.Inbox.FetchAsync(0, -1, MessageSummaryItems.UniqueId))
            .Select(x => x.UniqueId).ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task RunAsync()
    {
        await ConnectAsync();

        _client.Inbox.CountChanged += OnCountChanged;

        _client.Disconnected += async (e, state) =>
        {
            while (true)
            {
                try
                {
                    await ConnectAsync();

                    break;
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, ex.Message);
                }

                await Task.Delay(1000);
            }
        };

        //后台线程处理
        var _ = Task.Factory.StartNew(async () =>
        {
            while (!_cancel.IsCancellationRequested)
            {
                try
                {
                    //先检查数据
                    var nowUids = await GetAllUids();
                    var newUids = from a in nowUids
                                  join b in _existsUids on a equals b into ab
                                  from b in ab.DefaultIfEmpty()
                                  where b == default
                                  select a;

                    if (newUids.Any())
                    {
                        if (OnNewMessage != null)
                        {
                            _logger?.LogInformation("{UserName} 收到新邮件:{Uids}", UserName, string.Join(",", newUids));

                            _newMessageQueue.Add(newUids.ToArray());

                            _existsUids.AddRange(newUids);
                        }
                    }

                    _logger?.LogInformation("IdleClient开始Idle {Host}:{Port} {UserName}", Host, Port, UserName);

                    if (_client.Capabilities.HasFlag(ImapCapabilities.Idle))
                    {
                        // Note: IMAP servers are only supposed to drop the connection after 30 minutes, so normally
                        // we'd IDLE for a max of, say, ~29 minutes... but GMail seems to drop idle connections after
                        // about 10 minutes, so we'll only idle for 9 minutes.
                        _done = new CancellationTokenSource(new TimeSpan(0, 9, 0));
                        try
                        {
                            await _client.IdleAsync(_done.Token, _cancel.Token);
                        }
                        finally
                        {
                            _done.Dispose();
                            _done = null;
                        }
                    }
                    else
                    {
                        // Note: we don't want to spam the IMAP server with NOOP commands, so lets wait a minute
                        // between each NOOP command.
                        await Task.Delay(new TimeSpan(0, 1, 0), _cancel.Token);
                        await _client.NoOpAsync(_cancel.Token);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, ex.Message);
                    await Task.Delay(1000);
                }
            }
        }, TaskCreationOptions.LongRunning);
    }

    // Note: the CountChanged event will fire when new messages arrive in the folder and/or when messages are expunged.
    void OnCountChanged(object sender, EventArgs e)
    {
        _done?.Cancel();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        _cancel.Cancel();
        _cancel.Dispose();

        _client.Disconnect(true);
        _client.Dispose();
    }
}
