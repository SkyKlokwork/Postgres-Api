using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite.Operation.Buffer;

namespace MessengerApp.Backend.Models;
public class User {
    private string id = UniqueId.CreateUniqueId();
    public string username;
    private readonly string email;
    private readonly string password;
    
    public User(string Username, string Email, string Password) {
        username = Username;
        email = Email;
        password = Password;
    }
    // TODO: add info grabing
    
}