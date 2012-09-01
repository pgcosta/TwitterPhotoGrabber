using System;
using System.Web;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using TweetSharp;
using System.Diagnostics;
using System.IO;

namespace YTweeter
{
    class YTweeter
    {
        private TwitterService service;
        private String token;
        private String tokenSecret;

        public YTweeter()
        {
            this.service = new TwitterService("mzgSV0jBVulvNuGMKz8A","wRdizIlRc8f9fW6C0zddxH0EIGepT4Pek6lyr61Zg");
            this.token = String.Empty;
            this.tokenSecret = String.Empty;
        }

        public void Authorize()
        {
            //TwitterService service = new TwitterService("mzgSV0jBVulvNuGMKz8A", "wRdizIlRc8f9fW6C0zddxH0EIGepT4Pek6lyr61Zg");
            OAuthRequestToken requestToken = service.GetRequestToken();

            // Step 2 - Redirect to the OAuth Authorization URL
            Uri uri = service.GetAuthorizationUri(requestToken);
            Process.Start(uri.ToString());

            string verifier = Verifier(); // <-- This is input into your application by your user
            OAuthAccessToken access = service.GetAccessToken(requestToken, verifier);
            this.token = access.Token;
            this.tokenSecret = access.TokenSecret;
            service.AuthenticateWith(access.Token, access.TokenSecret);
        }

        //Returns the number of followers profile pictures downloaded
        public int downloadProfilePictures()
        {
            int count=0;
            String name;
            String imgUrl;
            Image image;            

            WebClient client = new WebClient();
            IEnumerable<TwitterUser> followers = service.ListFollowers();

            foreach(TwitterUser user in followers ) {
                imgUrl = user.ProfileImageUrl;
                name = count.ToString() + ".png";
                //client.DownloadFile(imgUrl,name);
                image = byteToImage(client.DownloadData(imgUrl));
                image.Save(name, ImageFormat.Png);
                count++;
                if (count % 100 == 0)
                {
                    followers = service.ListFollowers();
                }
            }
            Console.WriteLine("\n\n" + count + " profile pictures downloaded.");

            return count;
        }

        Image byteToImage(byte[] raw)
        {
            MemoryStream buffer = new MemoryStream(raw);
            return Image.FromStream(buffer);
        }

        //This methode must specify how should the application PIN be retrieved
        public virtual string Verifier()
        {
            Console.WriteLine("Insert PIN:");
            return Console.ReadLine();
        }

        static void Main()
        {
            YTweeter app = new YTweeter();
            app.Authorize();
            app.downloadProfilePictures();
            Console.ReadLine();
        }
    }
}
