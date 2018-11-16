using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FIUChat.Enums;
using FIUChat.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
using System.Text;

namespace FIU_Chat.Controllers
{
    [Produces("application/json")]
    public class ClassesController : Controller
    {
        private AuthenticateUser authenticateUser = new AuthenticateUser();
        private static Socket serverSocket = null;

        // Thread signal.  
        private static ManualResetEvent allDone = new ManualResetEvent(false);

        public async Task<IActionResult> DisplayClass()
        {
            var request = Request;
            var headers = request.Headers;

            StringValues token;
            if (headers.TryGetValue("Authorization", out token))
            {
                var result = this.authenticateUser.ValidateToken(token);
                if (result.Result == AuthenticateResult.Success)
                {
                    var successfulToken = Json(new
                    {
                        success = true
                    });

                    if (serverSocket == null)
                    {
                        Thread socketThread = new Thread(() => ClassesController.acceptClients());
                        socketThread.Start();
                    }

                    return Ok(successfulToken);
                }
                else
                {
                    return RedirectToAction("Index", "Account");
                }
            }

            return RedirectToAction("Index", "Account");
        }

#region Server Socket
        private static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        private static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the   
                    // client. Display it on the console.  
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                    // Echo the data back to the client.  
                    Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void acceptClients()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            if (serverSocket == null)
                serverSocket = new Socket(ipAddress.AddressFamily,
                                          SocketType.Stream, ProtocolType.Tcp);
            try
            {
                serverSocket.Bind(localEndPoint);
                serverSocket.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");
                    serverSocket.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        serverSocket);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
#endregion

        private async Task<Dictionary<string, List<Dictionary<string, string>>>> GetUserClasses(string token)
        {
            return await this.authenticateUser.GetUserDictionaryFromToken(token);
        }
    }

    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }
}