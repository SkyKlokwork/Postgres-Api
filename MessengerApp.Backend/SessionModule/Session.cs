using System.Net.WebSockets;
using MessengerApp.Backend.Models;
using MessengerApp.Backend.TCP;
using Microsoft.IdentityModel.Tokens;

namespace MessengerApp.Backend.SessionModule;
public class Session {
    private User _user;
    private WebSocket _ws;
    public Session(HttpContext context) {
    }
}