using AIProject.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AIProject
{
    public class Proxy
    {
        private string baseUri = "http://localhost:2926/api/";
        private HttpClient _httpClient { get; set; }
        private string _token { get; set; } = null;

        public Proxy(string uri)
        {
            this.baseUri = uri;
            _httpClient = new HttpClient();
        }

        public bool Login(string username, string password)
        {
            var content = new FormUrlEncodedContent(new[]
{
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("pasword", password)
            });

            var result = _httpClient.PostAsync(baseUri + "Users/" + "Token", content).Result;
            var token = JsonConvert.DeserializeObject<TokenRespone>(result.Content.ReadAsStringAsync().Result);

            _token = token.access_token;

            if (_token != null)
                return true;
            else
                return false;
        }

        public User RegisterUser(User user)
        {
            //var content = new FormUrlEncodedContent(new[]
            //{
            //    new KeyValuePair<string, string>("email", email),
            //    new KeyValuePair<string, string>("username", username),
            //    new KeyValuePair<string, string>("pasword", password)
            //});
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var result = _httpClient.PostAsync(baseUri + "PostUser", content).Result;
            var newUser = JsonConvert.DeserializeObject<User>(result.Content.ReadAsStringAsync().Result);

            var content2 = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", newUser.UserName),
                new KeyValuePair<string, string>("pasword", newUser.Password)
            });


            var result2 = _httpClient.PostAsync(baseUri + "Users/" + "Token", content).Result;
            var token = JsonConvert.DeserializeObject<TokenRespone>(result.Content.ReadAsStringAsync().Result);

            _token = token.access_token;

            return newUser;
        }

        public User GetUserDetails(int id)
        {
            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.Headers.Add("Authorization", _token);

            message.RequestUri = new Uri($"{baseUri}Users/{id}");
            var result = _httpClient.SendAsync(message).Result;
            return JsonConvert.DeserializeObject<User>(result.Content.ReadAsStringAsync().Result);
        }

        public void FinishFileUploading(string userName, string fileName, string checkSum)
        {
            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.Headers.Add("Authorization", _token);

            message.RequestUri = new Uri($"{baseUri}Files/{userName}/{fileName}/{checkSum}");
            var result = _httpClient.SendAsync(message).Result;
        }

    }
}
