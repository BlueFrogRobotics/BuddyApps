using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BuddyApp.ExperienceCenter
{
	public class TcpServer : MonoBehaviour
	{
		public static bool clientConnected ;
		private static Socket mHandler;
		private bool mStateSent = false;
		private AnimatorManager mAnimatorManager;

		private bool mEventClient;

		// State object for reading client data asynchronously
		private class StateObject
		{
			// Client  socket.
			public Socket workSocket = null;
			// Size of receive buffer.
			public const int bufferSize = 1024;
			// Receive buffer.
			public byte[] buffer = new byte[bufferSize];
			// Received data string.
			public StringBuilder Sb = new StringBuilder ();
		}
			
		// Use this for initialization
		public void Init ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			clientConnected = false;
			StartCoroutine(Listening());
			InvokeRepeating ("SendStateRequest", 0.5f, 3.0f);
		}
			
		private string GetIPAddress ()
		{
			IPHostEntry host;
			string localIP = "";
			host = Dns.GetHostEntry (Dns.GetHostName ());
			foreach (IPAddress ip in host.AddressList) {
				if (ip.AddressFamily == AddressFamily.InterNetwork) {
					localIP = ip.ToString ();
				}

			}
			return localIP;
		}
			

		private IEnumerator Listening ()
		{
			// host running the application.
			IPAddress[] ipArray = Dns.GetHostAddresses (GetIPAddress ());
			IPEndPoint localEndPoint = new IPEndPoint (ipArray [0], 3000);

			Debug.Log ("[TCP SERVER] Server address and port : " + localEndPoint.ToString ());
			// Create a TCP/IP socket.
			Socket listener = new Socket (ipArray [0].AddressFamily,
				                  SocketType.Stream, ProtocolType.Tcp);
			listener.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.ReuseAddress, true);
			// Bind the socket to the local endpoint and 
			// listen for incoming connections.
				listener.Bind (localEndPoint);
				listener.Listen (10);

				// Start listening for connections.
				while (true) {
					mEventClient = false;
					listener.BeginAccept (
						new AsyncCallback (AcceptCallback),
						listener);
				
					yield return new WaitUntil(() => mEventClient );
				}
		}


		public void AcceptCallback (IAsyncResult ar)
		{
			mEventClient = true;
			if (!clientConnected) {
				// Get the socket that handles the client request.
				Socket listener = (Socket)ar.AsyncState;
				Socket handler = listener.EndAccept (ar);
				mHandler = handler;
				// Create the state object.
				StateObject state = new StateObject ();
				state.workSocket = handler;
				handler.BeginReceive (state.buffer, 0, StateObject.bufferSize, 0,
					new AsyncCallback (ReadCallback), state);

				IPEndPoint clientIP = handler.RemoteEndPoint as IPEndPoint;
				Debug.Log ("[TCP SERVER] Client: " + clientIP.Address + ":" + clientIP.Port + " connected");
				clientConnected = true;
				mAnimatorManager.clientConnected = true;
			} else {
				Socket listener = (Socket)ar.AsyncState;
				Socket handler = listener.EndAccept (ar);
				string message = "Server busy";
				// Convert the string data to byte data using ASCII encoding.
				byte[] byteData = Encoding.ASCII.GetBytes (message);
				// Begin sending the data to the remote device.
				handler.BeginSend (byteData, 0, byteData.Length, 0,
					new AsyncCallback (SendCallback), handler);
				IPEndPoint clientIP = handler.RemoteEndPoint as IPEndPoint;
				Debug.LogWarning ("[TCP SERVER] Client: " + clientIP.Address + ":" + clientIP.Port + " wants to connect, but server is busy");
				handler.Shutdown (SocketShutdown.Both);
				handler.Close ();
			}

		}

		public void ReadCallback (IAsyncResult ar)
		{
			String content = String.Empty;

			// Retrieve the state object and the handler socket
			// from the asynchronous state object.
			StateObject state = (StateObject)ar.AsyncState;
			Socket handler = state.workSocket;

			if (handler != null && !handler.Poll (1, SelectMode.SelectRead) && handler.Connected && handler.Available == 0) {
				// Read data from the client socket. 
				int bytesRead = handler.EndReceive (ar);

				if (bytesRead > 0) {
					switch (state.buffer [0]) {
					case (byte) (Mode.StateResponse):
						{
							Debug.Log ("[TCP SERVER] Got State Response ");
							break;
						}

					case (byte) (Mode.CommandRequest): 
						{
							ActivateCommand (handler, state.buffer [1]);
							Debug.Log ("[TCP SERVER] Cmd " + (Command)(state.buffer [1]) + " is received");
							break;
						}
					default:
						{
							Debug.LogWarning ("[TCP SERVER] Unrecognized message from the client");
							break;
						}
					}
					
					handler.BeginReceive (state.buffer, 0, StateObject.bufferSize, 0,
						new AsyncCallback (ReadCallback), state);
				}
			} else {
				if (handler.Connected) {
					// Client are offline will be detected here
					IPEndPoint clientIP = handler.RemoteEndPoint as IPEndPoint;
					Debug.Log ("[TCP SERVER] Client: " + clientIP.Address + ":" + clientIP.Port + " disconnected");
					clientConnected = false;
					mAnimatorManager.clientConnected = false;
					mStateSent = false;
				}
			}
		}

		private void SendStateRequest ()
		{
			if (/*!mStateSent &&*/ clientConnected) {
				byte[] byteData = new byte[] { (byte)(Mode.StateRequest), (byte)(BuddyState.HighBattery) };
				// Begin sending the data to the remote device.
				mHandler.BeginSend (byteData, 0, byteData.Length, 0,
					new AsyncCallback (SendCallback), mHandler);
				mStateSent = true;
			}
		}

		private void SendCmdResponse (Socket handler, Command cmd)
		{
			byte[] byteData = new byte[] { (byte)Mode.CommandResponse, (byte)(cmd) };
			handler.BeginSend (byteData, 0, byteData.Length, 0,
				new AsyncCallback (SendCallback), handler);
	
		}

		private void ActivateCommand (Socket handler, byte cmd)
		{
			SendCmdResponse (handler, (Command)cmd);
			mAnimatorManager.ActivateCmd (cmd);
		}

		private void SendCallback (IAsyncResult ar)
		{
			try {
				// Retrieve the socket from the state object.
				Socket handler = (Socket)ar.AsyncState;

				// Complete sending the data to the remote device.
				//int bytesSent = handler.EndSend (ar);
				//Debug.Log ("Sent " + bytesSent + " bytes to client.");

			} catch (Exception e) {
				Debug.Log (e.ToString ());
			}
		}


		public void StopServer ()
		{
			if (mHandler != null && mHandler.Connected)
			{
				Debug.LogWarning("Disconnecting all client!");
				mHandler.Shutdown (SocketShutdown.Both);
				mHandler.Close ();
			}
				
		}

	}

}