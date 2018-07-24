using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.rtc.Model.V20180111;

using System.Security.Cryptography;

namespace rtc_app_csharp
{
    class ChannelAuth
    {
        public string AppId;
        public string ChannelId;
        public string Nonce;
        public Int64 Timestamp;
        public string ChannelKey;
    }

    class Program
    {
        static ChannelAuth CreateChannel(
            string appid, string channelId,
            string regionId, string accessKeyId, 
            string accessKeySecret)
        {
            IClientProfile profile = DefaultProfile.GetProfile(
                regionId, accessKeyId, accessKeySecret);
            IAcsClient client = new DefaultAcsClient(profile);

            CreateChannelRequest request = new CreateChannelRequest();
            request.AppId = appid;
            request.ChannelId = channelId;

            CreateChannelResponse response = client.GetAcsResponse(request);

            ChannelAuth auth = new ChannelAuth();
            auth.AppId = appid;
            auth.ChannelId = channelId;
            auth.Nonce = response.Nonce;
            auth.Timestamp = (Int64)response.Timestamp;
            auth.ChannelKey = response.ChannelKey;

            return auth;
        }

        static string BuildToken(
            string channelId, string channelKey, 
            string appid, string userId, string session, 
            string nonce, Int64 timestamp)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(channelId).Append(channelKey);
            sb.Append(appid).Append(userId);
            sb.Append(session).Append(nonce).Append(timestamp);

            using (SHA256 hash = SHA256.Create())
            {
                byte[] checksum = hash.ComputeHash(
                    Encoding.ASCII.GetBytes(sb.ToString()));

                sb = new StringBuilder();
                foreach (byte b in checksum)
                {
                    sb.Append(b.ToString("x2"));
                }

                string token = sb.ToString();
                return token;
            }
        }

        static Dictionary<string, ChannelAuth> channels = new Dictionary<string, ChannelAuth>();

        static void Main(string[] args)
        {
            string appid = "", listen = "", accessKeyId = "", accessKeySecret = "", gslb = "";
            string regionId = "cn-hangzhou";

            foreach (string arg in args)
            {
                string key = arg.Split('=')[0], value = arg.Split('=')[1];
                if (key == "--appid")
                {
                    appid = value;
                }
                else if (key == "--listen")
                {
                    listen = value;
                }
                else if (key == "--access-key-id")
                {
                    accessKeyId = value;
                }
                else if (key == "--access-key-secret")
                {
                    accessKeySecret = value;
                }
                else if (key == "--gslb")
                {
                    gslb = value;
                }
            }
            if (appid == "" || listen == "" || accessKeyId == "" || accessKeySecret == "" || gslb == "")
            {
                System.Console.WriteLine("Usage: app.exe <options>");
                System.Console.WriteLine("    --appid             the id of app");
                System.Console.WriteLine("    --listen            listen port");
                System.Console.WriteLine("    --access-key-id     the id of access key");
                System.Console.WriteLine("    --access-key-secret the secret of access key");
                System.Console.WriteLine("    --gslb              the gslb url");
                System.Console.WriteLine("Example:");
                System.Console.WriteLine("    app.exe --listen=8080 --access-key-id=OGAEkdiL62AkwSgs --access-key-secret=4JaIs4SG4dLwPsQSwGAHzeOQKxO6iw --appid=iwo5l81k --gslb=https://rgslb.rtc.aliyuncs.com");
                Environment.Exit(-1);
            }
            System.Console.WriteLine(String.Format(
                "Server listen={0}, appid={1}, akID={2}, akSecret={3}, gslb={4}, regionId={5}",
                listen, appid, accessKeyId, accessKeySecret, gslb, regionId));

            using (HttpListener listener = new HttpListener())
            {
                listener.Prefixes.Add(String.Format("http://localhost:{0}/", listen));
                listener.Start();

                while (true)
                {
                    HttpListenerContext context = listener.GetContext();
                    HandleRequest(context, appid, accessKeyId, accessKeySecret, gslb, regionId);
                }
            }
        }

        static void HandleRequest(HttpListenerContext context, string appid, string accessKeyId, string accessKeySecret, string gslb, string regionId)
        {
            if (context.Request.Headers.Get("Origin") != "")
            {
                context.Response.Headers.Set("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Set("Access-Control-Allow-Methods", "GET,POST,HEAD,PUT,DELETE,OPTIONS");
                context.Response.Headers.Set("Access-Control-Expose-Headers", "Server,Range,Content-Length,Content-Range");
                context.Response.Headers.Set("Access-Control-Allow-Headers", "Origin,Range,Accept-Encoding,Referer,Cache-Control,X-Proxy-Authorization,X-Requested-With,Content-Type");
            }

            if (context.Request.HttpMethod == "OPTIONS")
            {
                responseWrite(context, HttpStatusCode.OK, "");
                return;
            }

            string url = context.Request.RawUrl;
            if (!url.StartsWith("/app/v1/login"))
            {
                responseWrite(context, HttpStatusCode.NotFound, String.Format("Invalid url {0}", url));
                return;
            }

            string channelId = context.Request.QueryString.Get("room");
            string user = context.Request.QueryString.Get("user");
            string channelUrl = string.Format("{0}/{1}", appid, channelId);
            System.Console.WriteLine(String.Format("Request channelId={0}, user={1}, appid={2}", channelId, user, appid));

            ChannelAuth auth = null;
            using (Mutex locker = new Mutex())
            {
                locker.WaitOne();

                if (channels.ContainsKey(channelUrl))
                {
                    auth = channels[channelUrl];
                    return;
                }

                try
                {
                    auth = CreateChannel(appid, channelId, regionId, accessKeyId, accessKeySecret);
                    channels[channelUrl] = auth;
                    System.Console.WriteLine(String.Format(
                        "Create channelId={0}, nonce={1}, timestamp={2}, channelKey={3}",
                        channelId, auth.Nonce, auth.Timestamp, auth.ChannelKey));
                }
                catch (Exception ex)
                {
                    responseWrite(context, HttpStatusCode.InternalServerError, ex.Message);
                    return;
                }
            }

            string userId = Guid.NewGuid().ToString();
            string session = Guid.NewGuid().ToString();

            try
            {
                string token = BuildToken(channelId, auth.ChannelKey, appid, userId, session, auth.Nonce, auth.Timestamp);
                string username = String.Format(
                    "{0}?appid={1}&session={2}&channel={3}&nonce={4}&timestamp={5}",
                    userId, appid, session, channelId, auth.Nonce, auth.Timestamp);

                JObject rturn = new JObject();
                rturn.Add("username", username);
                rturn.Add("password", token);

                JArray rgslbs = new JArray();
                rgslbs.Add(gslb);

                JObject rresponse = new JObject();
                rresponse.Add("appid", appid);
                rresponse.Add("userid", userId);
                rresponse.Add("gslb", rgslbs);
                rresponse.Add("session", session);
                rresponse.Add("token", token);
                rresponse.Add("nonce", auth.Nonce);
                rresponse.Add("timestamp", auth.Timestamp);
                rresponse.Add("turn", rturn);

                JObject ro = new JObject();
                ro.Add("code", 0);
                ro.Add("data", rresponse);

                responseWrite(context, HttpStatusCode.OK, ro.ToString());
            }
            catch (Exception ex)
            {
                responseWrite(context, HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        static void responseWrite(HttpListenerContext context, HttpStatusCode status, string message)
        {
            context.Response.StatusCode = (int)status;

            context.Response.Headers.Set("Content-Type", "application/json");

            byte[] b = Encoding.UTF8.GetBytes(message);
            using (Stream s = context.Response.OutputStream)
            {
                s.Write(b, 0, b.Length);
            }
        }
    }
}
