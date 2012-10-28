using System;
using Microsoft.SPOT;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace IxDLib {

    public delegate void ContextReceivedEventHandler(object sender, HttpListenerContext context);

    public class WebServer {

        private HttpListener server;
        private Thread connection;

        public event ContextReceivedEventHandler OnContextReceived;

        /// <summary>
        /// WebServer constructor
        /// </summary>
        /// <param name="prefix">Protocal prefix. Either 'http' or 'https'</param>
        /// <param name="port">Port number</param>
        public WebServer(string prefix, int port) {
            server = new HttpListener(prefix, port);
            server.Start();
            connection = new Thread(Context);
            connection.Start();

        }

        // Set prefix, standard port numbers (80 or 443)
        public WebServer(string prefix) : this(prefix, -1) { }

        // http on port 80
        public WebServer() : this("http", -1) { }

        /// <summary>
        /// Context listener thread
        /// </summary>
        protected void Context() {
            while (true) {
                try {
                    Debug.Print("Waiting for requests");
                    HttpListenerContext context = server.GetContext();
                    // invoke event
                    if (OnContextReceived != null) OnContextReceived(this, context);
                    Debug.Print(context.Request.HttpMethod + " " + context.Request.Url.OriginalString);
                }
                catch (SocketException e) {
                    Debug.Print(e.ToString());
                }
            }
        }

    }
}
