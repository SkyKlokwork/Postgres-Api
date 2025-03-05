using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Klokwork.ChatApp.DataSources.Client;
using Klokwork.ChatApp.DataSources.RequestHandler;
using Klokwork.ChatApp.DataSources.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InstantChatService.Test;
public class UnitTests
{
	// TODO: make an actual test client to properly test functionality and conncurrently build it so its less of a headache later
	private byte[] buffer = new byte[1024*4];
	private async Task Listener(WebSocket websocket,CancellationToken token = default) {
		while(websocket.State == WebSocketState.Open || websocket.State == WebSocketState.CloseSent) {
			try {
				var result = await websocket.ReceiveAsync(new ArraySegment<byte>(buffer),token);
				if (result.MessageType == WebSocketMessageType.Close) {
					await websocket.CloseAsync(result.CloseStatus!.Value,result.CloseStatusDescription,CancellationToken.None);
					return;
				}
				var message = Encoding.UTF8.GetString(buffer,0,result.Count);
				var test = Packet.ToPacket(message);
				var userid = test.Payload.GetProperty("authorId").GetString()!;
				Console.WriteLine("User id: {0}",userid);
			}
			catch(Exception e) {
				if(e.GetType() == typeof(TaskCanceledException)) {
					Console.WriteLine("Client Stopped Listening");
					return;
				}
				if (e.GetType() == typeof(TaskCanceledException)) {
					Console.WriteLine($"Excetpion: {e.Message}");
					return;
				}
				Console.WriteLine($"Something bad happened! {e.Message}");
				return;
			}
		}
	}
	private WebSocket OpenClient(IHost host) {
		WebSocketClient client = host.GetTestServer().CreateWebSocketClient();
		WebSocket startConnection = client.ConnectAsync(host.GetTestServer().BaseAddress, CancellationToken.None).Result;
		return startConnection;
	}
	private async Task<CancellationToken> CloseClient(WebSocket socket, string message = "") {
		try {
			await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "User Initiated Closing", default);
			Console.WriteLine($"Socket Closed: Associated Test: {message}");
			return new CancellationToken(true);
		}
		catch (Exception e) {
			Console.WriteLine($"{message} Socket Possibly Closed\nException Caught With Closure: {e.Message}");
			return new CancellationToken(true);
		}
	}
	private async Task SendPacket(WebSocket socket,Packet packet) {
		string rawJson = JsonSerializer.Serialize(packet);
		byte[] buffer = Encoding.UTF8.GetBytes(rawJson);
		await socket.SendAsync(new ArraySegment<byte>(buffer),WebSocketMessageType.Text,true,CancellationToken.None);
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
								.Configure<ClientSocketOptions>(e => {
									e.bufferSize = 1024 * 5 ;
								})
								.AddSingleton<RoomManager>()
								.AddSingleton<RequestHandlerProvider>()
								.AddSingleton<ClientWebSocketCache>();
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
	[Fact(DisplayName = "Test User Connection")]
	public async Task Test_User_Connection() {
		IHost? host = await Start_Test_Server() ?? null;
		Assert.NotNull(host);
		var client = OpenClient(host);
		Assert.NotNull(client);
		var Receive = (CancellationToken token) => {return Task.Run(() => Listener(client,token));};
		await SendPacket(client,new Packet(PacketType.PACKET_MESSAGE,new MessagePayload {
			message = "Hello World",
		}));
		var cancel = await CloseClient(client,"fish");
		await Receive.Invoke(cancel);
		await host.StopAsync();
	}
}
