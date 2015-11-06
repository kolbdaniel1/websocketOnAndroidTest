using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace App1
{
    [Activity(Label = "App1", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        WebsocketService websocket;
 

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button connect = FindViewById<Button>(Resource.Id.conn);
            Button send1 = FindViewById<Button>(Resource.Id.send);
            Button sendMultiple = FindViewById<Button>(Resource.Id.mul);
            websocket = new WebsocketService();

        }

        private void mulmj(object sender, EventArgs e)
        {
            for (int i = 0; i < 1000; i++) { 
            websocket.Send("");
        }
        }

        private void send(object sender, EventArgs e)
        {
            websocket.Send("asdf");
        }

        private void conn(object sender, EventArgs e)
        {
             websocket.ConnectWithWebsocket();
        }
    }
}

