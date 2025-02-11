using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite.Operation.Buffer;

namespace MessengerApp.Backend.Models;
public class User {
    private readonly string _id = UniqueId.CreateUniqueId();
    public string _username;
    private readonly string _email;
    private readonly string _password;
    
    public User(string Username, string Email, string Password) {
        _username = Username;
        _email = Email;
        _password = Password;
    }
    // TODO: add info grabing
    
}