using System;
using System.Threading.Tasks;
using System.Threading;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Discord;

using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
class Program
{
    public static DiscordSocketClient _client;
    private CommandService _comands;
    private IServiceProvider _services;
    public string token = "NDYxODU0MDExNDE5NTI1MTMx.Dha21g.w-0qd9iUUvrUoG_FO3aa-BUDtig";
    static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();
    public async Task RunBotAsync()
    {
        _client = new DiscordSocketClient();
        _comands = new CommandService();
        _services = new ServiceCollection()
                    .AddSingleton(_client)
                    .AddSingleton(_comands)
                    .BuildServiceProvider();
        _client.Log += Log;


        await RegisterCommandAsync();
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        await Task.Delay(-1);
    }
    private Task Log(LogMessage arg)
    {
        Console.WriteLine(arg);
        if (arg.ToString().Contains("Disconnecting"))
        {
            System.Diagnostics.Process.Start(Directory.GetCurrentDirectory() + "\\Black Bot.exe");
            System.Environment.Exit(1);
        }
        return Task.CompletedTask;
    }
    public async Task RegisterCommandAsync()
    {
        _client.MessageReceived += HandleCommandAsync;
        await _comands.AddModulesAsync(Assembly.GetEntryAssembly());
    }
    private async Task HandleCommandAsync(SocketMessage arg)
    {
        var message = arg as SocketUserMessage;
        int argPos = 0;
        if (message.Author.IsBot)
            return;
        if ((message.HasStringPrefix("-", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
        {
            var context = new SocketCommandContext(_client, message);
            var result = await _comands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
                Console.WriteLine(result.ErrorReason);
        }
    }
}