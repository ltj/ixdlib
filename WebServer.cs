using System;
using Microsoft.SPOT;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace IxDLib {

    /// <summary>
    /// Event handler for WebServer OnContextReceived events,
    /// ie. receiving an HTTP request.
    /// </summary>
    /// <param name="sender">WebServer instance</param>
    /// <param name="context">Request context</param>
    public delegate void ContextReceivedEventHandler(object sender, HttpListenerContext context);

    /// <summary>
    /// Tiny web server on a thread (not so tiny actually - the System.Http assembly is huge!)
    /// v0.1 by Lars Toft Jacobsen, ITU, IxDLab
    /// CC-BY-SA
    /// </summary>
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
