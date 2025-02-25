using System.ComponentModel;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Klokwork.ChatApp.DataSources.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace InstantChatService.Test;
public class UnitTests(ITestOutputHelper outputt)
{
	Uri uri = new Uri("wss://localhost:7212");
	private readonly ITestOutputHelper output = outputt;
    public async Task<ClientWebSocket> CreateClient() {
        ClientWebSocket socket = new ();
        // probably bad to initialize here then return it but ohwell
        await socket.ConnectAsync(uri,CancellationToken.None);
        return socket;
    }
	public async Task<IHost?> Start_Test_Server() {
		return await new HostBuilder()
			.ConfigureWebHost(webBuilder =>
			{
				webBuilder
					.UseTestServer()
					.ConfigureServices(services =>
						{
							services
								.AddRouting()
								.AddLogging()
								.AddOptions()
								.Configure<ClientSocketOptions>(options => {})
								.AddSingleton<ClientCache>();
						})
					.Configure(app =>
					{
						app.UseHttpsRedirection();
						var wsoptions = new WebSocketOptions {
						KeepAliveInterval = TimeSpan.FromMinutes(1),
						};
						app.UseWebSockets(wsoptions);
						app.UseClientManager();
						app.UseRouting();
					});
			}).StartAsync();
	}
    [Fact(DisplayName = "Test1")]
	public async Task TEST_CONNECT_AND_DISCONNECT() {
		using var host = await Start_Test_Server() ?? throw new NullReferenceException();
		var wsUri = new UriBuilder(host.GetTestServer().BaseAddress){
			Scheme = "wss",
			Path = "/"
		}.Uri;
		try {
			var test = await host.GetTestServer().CreateWebSocketClient().ConnectAsync(wsUri,default);
			var buffer = JsonSerializer.SerializeToUtf8Bytes(
				new Packet(
					type: PacketType.PACKET_MESSAGE, 
					payload: new TextChat {
							message = "test"
			}));
			await test.SendAsync(new ArraySegment<byte>(buffer),WebSocketMessageType.Text,true,default);
			await Task.Delay(100);
			await test.CloseAsync(WebSocketCloseStatus.NormalClosure," ", default);
			Assert.True(test.State == WebSocketState.Closed);
		} 
		catch (Exception e) {
			Assert.Fail(e.Message);
		}
	}

    // [Fact(DisplayName = "Test2")]
    // public async Task TEST_1() {
    //     var test = await CreateClient();
    //     var buffer = JsonSerializer.SerializeToUtf8Bytes(
	// 			new Packet(
	// 				type: PacketType.PACKET_MESSAGE, 
	// 				payload: new TextChat {
	// 						message = "test"
	// 		}));
    //     await test.SendAsync(new ArraySegment<byte>(buffer),WebSocketMessageType.Text,true,CancellationToken.None);
    //     WebSocketReceiveResult result;
    //     var buf = new byte[1024*4];
    //     result = await test.ReceiveAsync(new ArraySegment<byte>(buf),CancellationToken.None);
    //     string output = Encoding.UTF8.GetString(buf[..result.Count]);
    //     Console.WriteLine(output);
    //     await test.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
    // }
}
