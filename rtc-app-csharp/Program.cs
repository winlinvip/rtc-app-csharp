using System;
using System.Net;

namespace rtc_app_csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            string appid = "", listen = "", accessKeyId = "", accessKeySecret = "", gslb = "";
            for (int i = 0; i < args.Length - 1; i++)
            {
                string key = args[i];
                if (key == "--appid")
                {
                    appid = args[++i];
                }
                else if (key == "--listen")
                {
                    listen = args[++i];
                }
                else if (key == "--access-key-id")
                {
                    accessKeyId = args[++i];
                }
                else if (key == "--access-key-secret")
                {
                    accessKeySecret = args[++i];
                }
                else if (key == "--gslb")
                {
                    gslb = args[++i];
                }
            }
            if (appid == "" || listen == "" || accessKeyId == "" || accessKeySecret == "" || gslb == "")
            {
                System.Console.WriteLine(String.Format("Usage: <options> %s", args[0]));
                Environment.Exit(-1);
            }
            System.Console.WriteLine(String.Format(
                "Server listen=%s, appid=%s, akID=%s, akSecret=%s, gslb=%s",
                listen, appid, accessKeyId, accessKeySecret, gslb));

            using (HttpListener listener = new HttpListener())
            {
                listener.Start();
            }
        }
    }
}
