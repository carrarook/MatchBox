using System.Collections.Concurrent;

namespace WhatsAppMovieBot.Services

{
    public class UserStore
    {
        private readonly ConcurrentDictionary<string, string> _usernames = new();

        public void SetUser(string phone, string username)
        {
            _usernames[phone] = username;
        }

        public string? GetUser(string phone)
        {
            return _usernames.TryGetValue(phone, out var username) ? username : null;
        }
    }
}