using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net.NetworkInformation;

namespace BuddyApp.ExperienceCenter
{
	public class TcpServer : MonoBehaviour
	{
		public static bool clientConnected;
		private static bool mEventClient;

		private static Socket mHandler;

		private static AnimatorManager mAnimatorManager;

		public static bool mStop = false;

		private class StateObject
		{
			public Socket workSocket = null;
			public const int bufferSize = 1024;
			public byte[] buffer = new byte[bufferSize];
			public StringBuilder Sb = new StringBuilder ();
		}

		public void Init ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			clientConnected = false;
			StartCoroutine (Listening ());
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
			yield return new WaitUntil (() => !mStop);
			IPAddress[] ipArray = Dns.GetHostAddresses (GetIPAddress ());
			IPEndPoint localEndPoint = new IPEndPoint (ipArray [0], 3000);

			Debug.Log ("[EXCENTER] [TCP SERVER] Server address and port : " + localEndPoint.ToString ());
			ExperienceCenterData.Instance.IPAddress = localEndPoint.ToString ();

			// Create a TCP/IP socket if the port is released
			Socket listener = new Socket (ipArray [0].AddressFamily,
				                  SocketType.Stream, ProtocolType.Tcp);

			// Bind the socket to the local endpoint and 
			// listen for incoming connections.
			listener.Bind (localEndPoint);
			listener.Listen (10);

			// Start listening for connections.
			Debug.Log ("[EXCENTER][TCP SERVER] Start listening");
			ExperienceCenterData.Instance.StatusTcp = "Waiting for connection";
			while (true) {
				mEventClient = false;
				listener.BeginAccept (
					new AsyncCallback (AcceptCallback),
					listener);
				yield return new WaitUntil (() => mEventClient || mStop);
				if (mStop) {
					break;
				}
			}

			listener.Close ();
			Debug.LogWarning ("[EXCENTER][TCP SERVER] Stop listening");
			mStop = false;
		}


		public static void AcceptCallback (IAsyncResult ar)
		{
			mEventClient = true;
			if (!clientConnected) {
				// Get the socket that handles the client request.
				Socket listener = (Socket)ar.AsyncState;
				mHandler = listener.EndAccept (ar);
				// Create the state object.
				StateObject state = new StateObject ();
				state.workSocket = mHandler;
				mHandler.BeginReceive (state.buffer, 0, StateObject.bufferSize, 0,
					new AsyncCallback (ReadCallback), state);

				IPEndPoint clientIP = mHandler.RemoteEndPoint as IPEndPoint;
				Debug.Log ("[EXCENTER] [TCP SERVER] Client: " + clientIP.Address + ":" + clientIP.Port + " connected");
				clientConnected = true;
				ExperienceCenterData.Instance.StatusTcp = "Client connected";
				mAnimatorManager.ConnectionTrigger ();
			} else {
				Socket listener = (Socket)ar.AsyncState;
				Socket handler = listener.EndAccept (ar);
				SendServerBusy (handler);
				IPEndPoint clientIP = handler.RemoteEndPoint as IPEndPoint;
				Debug.LogWarning ("[EXCENTER] [TCP SERVER] Client: " + clientIP.Address + ":" + clientIP.Port + " wants to connect, but server is busy");
				handler.Shutdown (SocketShutdown.Both);
				handler.Close ();
			}

		}

		public static void ReadCallback (IAsyncResult ar)
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
//					case (byte) (Mode.StateAck):
//						{
//							Debug.Log ("[TCP SERVER] Got State Acknowledge ");
//							break;
//						}

					case (byte) (Mode.CommandReq): 
						{
							ActivateCommand (handler, state.buffer [1]);
							Debug.Log ("[EXCENTER] [TCP SERVER] Command Req" + (Command)(state.buffer [1]) + " is received");
							break;
						}
					
					case (byte) (Mode.StateReq): 
						{
							CheckState (handler, state.buffer [1]);
							Debug.Log ("[EXCENTER] [TCP SERVER] State Req " + (StateReq)(state.buffer [1]) + " is received");
							break;
						}
					default:
						{   
							SendMsgUndef (handler);
							Debug.LogWarning ("[EXCENTER] [TCP SERVER] Unrecognized message mode from the client");
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
					Debug.Log ("[EXCENTER] [TCP SERVER] Client: " + clientIP.Address + ":" + clientIP.Port + " disconnected");
					ExperienceCenterData.Instance.StatusTcp = "Waiting for connection";
					clientConnected = false;
					mAnimatorManager.ConnectionTrigger ();
				}
			}
		}

	
		private static void SendServerBusy (Socket handler)
		{
			byte[] byteData = new byte[] { (byte)Mode.ServerBusy, 0x01 };
			handler.BeginSend (byteData, 0, byteData.Length, 0,
				new AsyncCallback (SendCallback), handler);

		}

		private static void SendMsgUndef (Socket handler)
		{
			byte[] byteData = new byte[] { (byte)Mode.ServerBusy, (byte)State.Undefined };
			handler.BeginSend (byteData, 0, byteData.Length, 0,
				new AsyncCallback (SendCallback), handler);

		}

		private static void ActivateCommand (Socket handler, byte cmd)
		{
			SendCommandAck (handler, (Command)cmd);
			mAnimatorManager.ActivateCmd (cmd);
		}

		private static void SendCommandAck (Socket handler, Command cmd)
		{
			byte[] byteData = new byte[] { (byte)Mode.CommandAck, (byte)(cmd) };
			handler.BeginSend (byteData, 0, byteData.Length, 0,
				new AsyncCallback (SendCallback), handler);

		}

		private static void CheckState (Socket handler, byte stateReq)
		{
			State state = mAnimatorManager.CheckState (stateReq);
			SendStateAck (handler, state);

		}

		private static void SendStateAck (Socket handler, State state)
		{
			if (state != State.Undefined) {
				byte[] byteData = new byte[] { (byte)(Mode.StateAck), (byte)state };
				// Begin sending the data to the remote device.
				handler.BeginSend (byteData, 0, byteData.Length, 0,
					new AsyncCallback (SendCallback), mHandler);
			} else {
				Debug.LogWarning ("[EXCENTER] [TCP SERVER] Unrecognized state request from the client");
				SendMsgUndef (handler);
			}
		}

		private static void SendCallback (IAsyncResult ar)
		{
			try {
				Socket handler = (Socket)ar.AsyncState;
				handler.EndSend (ar);

			} catch (Exception e) {
				Debug.LogError (e.ToString ());
			}
		}


		public void StopServer ()
		{
			if (mHandler != null && mHandler.Connected) {
				Debug.LogWarning ("[EXCENTER][TCP SERVER] Disconnecting all client !");
				clientConnected = false;
				ExperienceCenterData.Instance.StatusTcp = "Offline";
				ExperienceCenterData.Instance.IPAddress = "-";
				mAnimatorManager.ConnectionTrigger ();
				mHandler.Shutdown (SocketShutdown.Both);
				mHandler.Close ();
			}
			mStop = true;
		}

	}
}