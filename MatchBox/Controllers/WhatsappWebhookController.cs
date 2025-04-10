using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Net.Http;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Identity;
using WhatsAppMovieBot.Services;

namespace WhatsAppMovieBot
{
    [ApiController]
    [Route("webhook")]
    public class WhatsAppWebhookController : ControllerBase
    {
        private readonly UserStore _userStore;

        public WhatsAppWebhookController(UserStore userStore)
        {
            _userStore = userStore;
        }

        [HttpGet]
        public IActionResult VerifyWebhook([FromQuery] string hub_mode, [FromQuery] string hub_challenge, [FromQuery] string hub_verify_token)
        {
            if (hub_mode == "subscribe" && hub_verify_token == "giulianne10")
            {
                return Ok(hub_challenge);
            }
            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveMessage([FromBody] JsonElement payload)
        {
            var phone = payload.GetProperty("entry")[0].GetProperty("changes")[0]
                .GetProperty("value").GetProperty("messages")[0]
                .GetProperty("from").GetString();
            var message = payload.GetProperty("entry")[0].GetProperty("changes")[0]
                .GetProperty("value").GetProperty("messages")[0]
                .GetProperty("text").GetProperty("body").GetString();

            if (message.StartsWith("/user setname "))
            {
                var username = message.Replace("/user setname ", "").Trim();
                _userStore.SetUser(phone, username);
                // Aqui você chamaria a API da Meta para enviar uma resposta
            }
            else if (message == "/lista")
            {
                var username = _userStore.GetUser(phone);
                if (username != null)
                {
                    var filmes = await GetWatchedFilms(username);
                    // Aqui você chamaria a API da Meta para enviar a resposta com os filmes
                }
                else
                {
                    // Enviar resposta pedindo para configurar o nome de usuário
                }
            }

            return Ok();
        }

        private async Task<List<string>> GetWatchedFilms(string username)
        {
            var url = $"https://letterboxd.com/{username}/films/";
            var http = new HttpClient();
            var html = await http.GetStringAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var list = new List<string>();
            var posters = doc.DocumentNode.SelectNodes("//div[contains(@class, 'poster-container')]//img");
            if (posters != null)
            {
                foreach (var img in posters)
                {
                    var title = img.GetAttributeValue("alt", null);
                    if (!string.IsNullOrEmpty(title))
                    {
                        list.Add(title);
                    }
                }
            }
            return list;
        }
    }
}
