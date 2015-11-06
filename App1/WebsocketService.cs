using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebSocket.Portable.Interfaces;
using WebSocket.Portable;
using dralloMultiPlayer.Messages;

namespace App1
{
    public class WebsocketService
    {
        private const string webSocketURI = "ws://drallodmmprototype.azurewebsites.net/dmmSocketConnector.ashx";

        public event Action Closed;
        public event Action Error;
        public event Action<string> Received;
        private const string challengeId = "45435-2435-245-2345-234";
        private const string deviceId = "123123123";
        private const string userName = "colbinator";
        WebSocketClient connection;

        public WebsocketService()
        {
            connection = new WebSocketClient();
            connection.Closed += OnWebsocketClosed;
            connection.Error += OnWebsocketError;
            connection.MessageReceived += OnWebsocketReceived;
            connection.AutoSendPongResponse = false;
            
        }


        public void ConnectWithWebsocket()
        {
            try
            {
               

                connection.OpenAsync(webSocketURI);
            }
            catch (Exception e)
            {

                Debug.WriteLine("could not connect: " + e.Message);
            }
        }

        void OnWebsocketClosed()
        {
            Debug.WriteLine("Closed Websocket");
            Closed();
        }


        void OnWebsocketError(Exception e)
        {
            Debug.WriteLine("Websocket Error" + e.Message);
            Error();
        }

        public async Task Send(string msg)
        {

            try
            {
                EchoWithTimestamp echo = new EchoWithTimestamp(msg);
                var msgFrame = new MessageFrame("echo-request", echo);
                string message = JsonConvert.SerializeObject(msgFrame);

                Debug.WriteLine("SEND" + message);
                await connection.SendAsync(message);
            }
            catch (Exception e)
            {
                Debug.WriteLine("exception in Send(): " + e.Message);
            }

        }

        public async Task Register()
        {
            var registerMsg = new RegisterMessage();
            registerMsg.challengeId = challengeId;
            registerMsg.deviceId = deviceId;
            var registerMessage = new MessageFrame("register", registerMsg);

            string message = JsonConvert.SerializeObject(registerMessage);

            await connection.SendAsync(message);
            Debug.WriteLine("sent register message: " + message);
        }

        public async Task Deregister()
        {
            var deregisterMsg = new DeregisterMessage();
            deregisterMsg.challengeId = challengeId;
            deregisterMsg.deviceId = deviceId;
            var deregisterMessage = new MessageFrame("deregister", deregisterMsg);

            string message = JsonConvert.SerializeObject(deregisterMessage);

            await connection.SendAsync(message);

            Debug.WriteLine("sent deregister message");
        }

        public async Task Join()
        {
            var joinMsg = new JoinMessage();
            joinMsg.challengeId = challengeId;
            joinMsg.userName = userName;
            var joinMessage = new MessageFrame("join", joinMsg);

            string message = JsonConvert.SerializeObject(joinMessage);
            await connection.SendAsync(message);

            Debug.WriteLine("sent join message");
        }

        int numberOfReceivedMessages = 0;
        void OnWebsocketReceived(IWebSocketMessage message)
        {

            string msgstr = message.ToString();
            Debug.WriteLine("recevid message:" + msgstr);

            var messageFrame = JsonConvert.DeserializeObject<MessageFrame>(msgstr);
            object typedMessage = new object();

            switch (messageFrame.messageType)
            {
                case "invite":
                    typedMessage = JsonConvert.DeserializeObject<JoinMessage>(messageFrame.data.ToString());
                    break;
                case "echo-request":
                    typedMessage = JsonConvert.DeserializeObject<EchoWithTimestamp>(messageFrame.data.ToString());
                    break;
                case "echo-reply":
                    typedMessage = JsonConvert.DeserializeObject<EchoWithTimestamp>(messageFrame.data.ToString());
                    break;
                default:
                    throw new Exception("invalid message type");
            }

            Debug.WriteLine(typedMessage.ToString() + " received ");
            //numberOfReceivedMessages++;
            //Received(msgstr);
        }
    }
}

