using System.ComponentModel;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Klokwork.ChatApp.DataSources.Client;
using Klokwork.ChatApp.DataSources.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Xunit.Abstractions;

namespace InstantChatService.Test;
public class UnitTests(ITestOutputHelper outputt)
{
	private readonly ITestOutputHelper output = outputt;
	private byte[] buffer = new byte[1024*4];
    private Task<WebSocket> CreateClient(IHost host) {
		Uri wsUri = new UriBuilder(host.GetTestServer().BaseAddress) {
			Scheme = "wss",
			Path = "/"
		}.Uri;
		return host.GetTestServer().CreateWebSocketClient().ConnectAsync(wsUri,default);
	}
	private async Task Listener(WebSocket socket) {
		try {
			var buf = new byte[1024*4];
			while(socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseSent) {
				WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buf),CancellationToken.None);
				Console.WriteLine("we are here in Listener 1");
				if (result.MessageType == WebSocketMessageType.Close) {
                    await socket.CloseOutputAsync(result.CloseStatus!.Value,result.CloseStatusDescription,default);
                    return;
                }
                var packet = Packet.ToPacket(Encoding.UTF8.GetString(buffer[..result.Count]));
				if (packet.Type == PacketType.PACKET_MESSAGE) {
					var payload = packet.Payload.Deserialize<MessagePayload>()!;
                	Console.WriteLine($"Message From Server: \n{payload.message} \n {payload.authorId} \n {payload.channelId}");
				}
			}
		}
		catch {
			Console.WriteLine("I am here (listener exception)");
			await socket.CloseAsync(WebSocketCloseStatus.InternalServerError, $"something went wrong",default);
			Assert.Fail();
		}
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
								.AddTransient<CreateChannelHandler>()
								.AddTransient<ChatHub>()
								.AddSingleton<HubCollector>()
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
		string userid = null!;
		try {
			var test = await CreateClient(host);
			var buf = JsonSerializer.SerializeToUtf8Bytes(
				new Packet(
					type: PacketType.PACKET_MESSAGE, 
					payload: new MessagePayload {
							message = "This message is technically from a different program :)",
			}));
			await test.SendAsync(new ArraySegment<byte>(buf),WebSocketMessageType.Text,true,default);
			await test.CloseAsync(WebSocketCloseStatus.NormalClosure,"END", default);
			Assert.True(test.State == WebSocketState.Closed);
		} 
		catch (Exception e) {
			Assert.Fail(e.Message);
		}
	}
	[Fact(DisplayName = "Test2")]
	public async Task CHANNEL_TEST() {
		using var host = await Start_Test_Server() ?? throw new  NullReferenceException();
		Tuple<string,WebSocket>[] clients = new Tuple<string,WebSocket>[5];
		Console.WriteLine("1");
		string _channelId = UniqueId.CreateUniqueId();
		Console.WriteLine("2");
		try {
			Console.WriteLine("3");
			for (int i = 0; i < 5; i++) {
				Console.WriteLine("4");
				var client = await CreateClient(host);
				Console.WriteLine("5");
				// await client.ReceiveAsync(new ArraySegment<byte>(buffer),default);
				Console.WriteLine("6");
				string userid = "test";
				// await Listener(client);
				Console.WriteLine($"Client {userid} created!");
				clients[i]= new Tuple<string, WebSocket>(userid,client);
			}
			var buf1 = JsonSerializer.SerializeToUtf8Bytes(new Packet(PacketType.PACKET_CREATE_SERVER,new RoutingComponent() {
				authorId = clients[0].Item1
			}));
			await clients[0].Item2.SendAsync(new ArraySegment<byte>(buf1),WebSocketMessageType.Text,true,default);
			for (int i = 1; i < 5; i++) {
				var buf2 = JsonSerializer.SerializeToUtf8Bytes(new Packet(PacketType.PACKET_SUBSCRIBE,new RoutingComponent {
				authorId = clients[0].Item1,
				channelId = _channelId
				}));
				// TODO: Make a class/function for the tests to send packets
				// this is annyoing as hell!
				await clients[i].Item2.SendAsync(new ArraySegment<byte>(buf2),WebSocketMessageType.Text,true,default);
			}
			
			var buf = JsonSerializer.SerializeToUtf8Bytes(new Packet(PacketType.PACKET_MESSAGE,new MessagePayload() {
				authorId = clients[3].Item1,
				message = "testing! 123!"
			}));
			await clients[3].Item2.SendAsync(new ArraySegment<byte>(buf),WebSocketMessageType.Text,true,default);
		}
		catch(Exception e) {
			Console.WriteLine(e);
			return;
		}
	}
    // [Fact(DisplayName = "Test3")]
    private async Task test_Live() {
		string userid = null!;
		try {
			var test = new ClientWebSocket();
			await test.ConnectAsync(new Uri("wss://localhost:7212/"),default);
			await Listener(test);
			var buf = JsonSerializer.SerializeToUtf8Bytes(
				new Packet(
					type: PacketType.PACKET_MESSAGE, 
					payload: new MessagePayload {
							message = "This message is technically from a different program :)",
							authorId = userid
			}));
			await test.SendAsync(new ArraySegment<byte>(buf),WebSocketMessageType.Text,true,default);
			await test.CloseAsync(WebSocketCloseStatus.NormalClosure,"END", default);
			Assert.True(test.State == WebSocketState.Closed);
		} 
		catch (Exception e) {
			Assert.Fail(e.Message);
		}
    }
}
