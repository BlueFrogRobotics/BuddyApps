using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Globalization;
using System.Threading;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates; 

namespace BuddyApp.Somfy
{
    public class HTTPException : Exception
    {
        public HTTPException(string message) : base(message)
        {
        }
    }

    public enum RequestState
    {
        WAITING, READING, DONE
    }

    public class Request
    {
        public static bool LogAllRequests = false;
        public static bool VerboseLogging = false;
        public static string unityVersion = Application.unityVersion;
        public static string operatingSystem = SystemInfo.operatingSystem; 

        public CookieJar cookieJar = CookieJar.Instance;
        public string method = "GET";
        public string protocol = "HTTP/1.1";
        public byte[] bytes;
        public Uri uri;
        public static byte[] EOL = { (byte)'\r', (byte)'\n' };
        public Response response = null;
        public bool isDone = false;
        public int maximumRetryCount = 8;
        public bool acceptGzip = true;
        public bool useCache = false;
        public Exception exception = null;
        public RequestState state = RequestState.WAITING;
        public long responseTime = 0; // in milliseconds
        public bool synchronous = false;

        public Action<Request> completedCallback = null;

        private Dictionary<string, List<string>> mHeaders = new Dictionary<string, List<string>>();
        private static Dictionary<string, string> mETags = new Dictionary<string, string>();

        public Request(string iMethod, string iURI)
        {
            this.method = iMethod;
            this.uri = new Uri(iURI);
        }

        public Request(string iMethod, string iURI, bool iUseCache)
        {
            this.method = iMethod;
            this.uri = new Uri(iURI);
            this.useCache = iUseCache;
        }

        public Request(string iMethod, string iURI, byte[] iBytes)
        {
            this.method = iMethod;
            this.uri = new Uri(iURI);
            this.bytes = iBytes;
        }

        /*public Request( string method, string uri, WWWForm form )
          {
              this.method = method;
              this.uri = new Uri (uri);
              this.bytes = form.data;
              foreach (KeyValuePair<string,string> entry in form.headers )
              {
                  this.AddHeader( (string)entry.Key, (string)entry.Value );
              }
          }*/

        public Request(string iMethod, string iURI, Hashtable iData)
        {
            this.method = iMethod;
            this.uri = new Uri(iURI);
            this.bytes = Encoding.UTF8.GetBytes(JSON.JsonEncode(iData));
            this.AddHeader("Content-Type", "application/json");
        }

        public void AddHeader(string iName, string iValue)
        {
            iName = iName.ToLower().Trim();
            iValue = iValue.Trim();
            if (!mHeaders.ContainsKey(iName))
                mHeaders[iName] = new List<string>();
            mHeaders[iName].Add(iValue);
        }

        public string GetHeader(string iName)
        {
            iName = iName.ToLower().Trim();
            if (!mHeaders.ContainsKey(iName))
                return "";
            return mHeaders[iName][0];
        }

        public List<string> GetHeaders()
        {
            List<string> oResult = new List<string>();
            foreach (string lName in mHeaders.Keys)
            {
                foreach (string value in mHeaders[lName])
                {
                    oResult.Add(lName + ": " + value);
                }
            }

            return oResult;
        }

        public List<string> GetHeaders(string iName)
        {
            iName = iName.ToLower().Trim();
            if (!mHeaders.ContainsKey(iName))
                mHeaders[iName] = new List<string>();
            return mHeaders[iName];
        }

        public void SetHeader(string lName, string lValue)
        {
            lName = lName.ToLower().Trim();
            lValue = lValue.Trim();
            if (!mHeaders.ContainsKey(lName))
                mHeaders[lName] = new List<string>();
            mHeaders[lName].Clear();
            mHeaders[lName].TrimExcess();
            mHeaders[lName].Add(lValue);
        }

        private void GetResponse()
        {
            System.Diagnostics.Stopwatch lCurCall = new System.Diagnostics.Stopwatch();
            lCurCall.Start();
            try
            {
                int lRetry = 0;
                while (++lRetry < maximumRetryCount)
                {
                    if (useCache)
                    {
                        string etag = "";
                        if (mETags.TryGetValue(uri.AbsoluteUri, out etag))
                        {
                            SetHeader("If-None-Match", etag);
                        }
                    }

                    SetHeader("Host", uri.Host);

                    var client = new TcpClient();
                    client.Connect(uri.Host, uri.Port);
                    using (var stream = client.GetStream())
                    {
                        var ostream = stream as Stream;
                        if (uri.Scheme.ToLower() == "https")
                        {
                            ostream = new SslStream(stream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate));
                            try
                            {
                                var ssl = ostream as SslStream;
                                ssl.AuthenticateAsClient(uri.Host);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError("Exception: " + e.Message);
                                return;
                            }
                        }
                        WriteToStream(ostream);
                        response = new Response();
                        response.Request = this;
                        state = RequestState.READING;
                        response.ReadFromStream(ostream);
                    }
                    client.Close();

                    switch (response.Status)
                    {
                        case 307:
                        case 302:
                        case 301:
                            uri = new Uri(response.GetHeader("Location"));
                            continue;
                        default:
                            lRetry = maximumRetryCount;
                            break;
                    }
                }
                if (useCache)
                {
                    string etag = response.GetHeader("etag");
                    if (etag.Length > 0)
                        mETags[uri.AbsoluteUri] = etag;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Unhandled Exception, aborting request.");
                Console.WriteLine(e);
                exception = e;
                response = null;
            }
            state = RequestState.DONE;
            isDone = true;
            responseTime = lCurCall.ElapsedMilliseconds;

            if (completedCallback != null)
            {
                if (synchronous)
                {
                    completedCallback(this);
                }
                else
                {
                    // we have to use this dispatcher to avoid executing the callback inside this worker thread
                    ResponseCallbackDispatcher.Instance.Requests.Enqueue(this);
                }
            }

            if (LogAllRequests)
            {
#if !UNITY_EDITOR
                System.Console.WriteLine("NET: " + InfoString(VerboseLogging));
#else
                if ( response != null && response.Status >= 200 && response.Status < 300 )
                {
                    Debug.Log( InfoString( VerboseLogging ) );
                }
                else if ( response != null && response.Status >= 400 )
                {
                    Debug.LogError( InfoString( VerboseLogging ) );
                }
                else
                {
                    Debug.LogWarning( InfoString( VerboseLogging ) );
                }
#endif
            }
        }


        private void GetResponse(string iUrl)
        {
            System.Diagnostics.Stopwatch lCurCall = new System.Diagnostics.Stopwatch();
            lCurCall.Start();
            try
            {
                int lRetry = 0;
                while (++lRetry < maximumRetryCount)
                {
                    if (useCache)
                    {
                        string etag = "";
                        if (mETags.TryGetValue(iUrl, out etag))
                        {
                            SetHeader("If-None-Match", etag);
                        }
                    }

                    SetHeader("Host", uri.Host);

                    var client = new TcpClient();
                    client.Connect(uri.Host, uri.Port);
                    using (var stream = client.GetStream())
                    {
                        var ostream = stream as Stream;
                        if (uri.Scheme.ToLower() == "https")
                        {
                            ostream = new SslStream(stream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate));
                            try
                            {
                                var ssl = ostream as SslStream;
                                ssl.AuthenticateAsClient(uri.Host);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError("Exception: " + e.Message);
                                return;
                            }
                        }
                        WriteToStream(ostream);
                        response = new Response();
                        response.Request = this;
                        state = RequestState.READING;
                        response.ReadFromStream(ostream);
                    }
                    client.Close();

                    switch (response.Status)
                    {
                        case 307:
                        case 302:
                        case 301:
                            uri = new Uri(response.GetHeader("Location"));
                            continue;
                        default:
                            lRetry = maximumRetryCount;
                            break;
                    }
                }
                if (useCache)
                {
                    string etag = response.GetHeader("etag");
                    if (etag.Length > 0)
                        mETags[iUrl] = etag;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Unhandled Exception, aborting request.");
                Console.WriteLine(e);
                exception = e;
                response = null;
            }
            state = RequestState.DONE;
            isDone = true;
            responseTime = lCurCall.ElapsedMilliseconds;

            if (completedCallback != null)
            {
                if (synchronous)
                {
                    completedCallback(this);
                }
                else
                {
                    // we have to use this dispatcher to avoid executing the callback inside this worker thread
                    ResponseCallbackDispatcher.Instance.Requests.Enqueue(this);
                }
            }

            if (LogAllRequests)
            {
#if !UNITY_EDITOR
                System.Console.WriteLine("NET: " + InfoString(VerboseLogging));
#else
                if ( response != null && response.Status >= 200 && response.Status < 300 )
                {
                    Debug.Log( InfoString( VerboseLogging ) );
                }
                else if ( response != null && response.Status >= 400 )
                {
                    Debug.LogError( InfoString( VerboseLogging ) );
                }
                else
                {
                    Debug.LogWarning( InfoString( VerboseLogging ) );
                }
#endif
            }
        }

        public virtual void Send(string iUrl, Action<Request> iCallback = null)
        {

            if (!synchronous && iCallback != null && ResponseCallbackDispatcher.Instance == null)
            {
                ResponseCallbackDispatcher.Init();
            }

            completedCallback = iCallback;

            isDone = false;
            state = RequestState.WAITING;
            if (acceptGzip)
            {
                SetHeader("Accept-Encoding", "gzip");
            }

            if (this.cookieJar != null)
            {
                List<Cookie> cookies = this.cookieJar.GetCookies(new CookieAccessInfo(uri.Host, iUrl));
                string cookieString = this.GetHeader("cookie");
                for (int cookieIndex = 0; cookieIndex < cookies.Count; ++cookieIndex)
                {
                    if (cookieString.Length > 0 && cookieString[cookieString.Length - 1] != ';')
                    {
                        cookieString += ';';
                    }
                    cookieString += cookies[cookieIndex].name + '=' + cookies[cookieIndex].value + ';';
                }
                SetHeader("cookie", cookieString);
            }

            if (bytes != null && bytes.Length > 0 && GetHeader("Content-Length") == "")
            {
                SetHeader("Content-Length", bytes.Length.ToString());
            }

            if (GetHeader("User-Agent") == "")
            {
                try
                {
                    SetHeader("User-Agent", "UnityWeb/1.0 (Unity " + Request.unityVersion + "; " + Request.operatingSystem + ")");
                }
                catch (Exception)
                {
                    SetHeader("User-Agent", "UnityWeb/1.0");
                }
            }

            if (GetHeader("Connection") == "")
            {
                SetHeader("Connection", "close");
            }

            // Basic Authorization
            if (!String.IsNullOrEmpty(uri.UserInfo))
            {
                SetHeader("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(uri.UserInfo)));
            }

            if (synchronous)
            {
                GetResponse(iUrl);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object t) {
                    GetResponse(iUrl);
                }));
            }
        }

        public virtual void Send(Action<Request> iCallback = null)
        {

            if (!synchronous && iCallback != null && ResponseCallbackDispatcher.Instance == null)
            {
                ResponseCallbackDispatcher.Init();
            }

            completedCallback = iCallback;

            isDone = false;
            state = RequestState.WAITING;
            if (acceptGzip)
            {
                SetHeader("Accept-Encoding", "gzip");
            }

            if (this.cookieJar != null)
            {
                List<Cookie> cookies = this.cookieJar.GetCookies(new CookieAccessInfo(uri.Host, uri.AbsolutePath));
                string cookieString = this.GetHeader("cookie");
                for (int cookieIndex = 0; cookieIndex < cookies.Count; ++cookieIndex)
                {
                    if (cookieString.Length > 0 && cookieString[cookieString.Length - 1] != ';')
                    {
                        cookieString += ';';
                    }
                    cookieString += cookies[cookieIndex].name + '=' + cookies[cookieIndex].value + ';';
                }
                SetHeader("cookie", cookieString);
            }

            if (bytes != null && bytes.Length > 0 && GetHeader("Content-Length") == "")
            {
                SetHeader("Content-Length", bytes.Length.ToString());
            }

            if (GetHeader("User-Agent") == "")
            {
                try
                {
                    SetHeader("User-Agent", "UnityWeb/1.0 (Unity " + Request.unityVersion + "; " + Request.operatingSystem + ")");
                }
                catch (Exception)
                {
                    SetHeader("User-Agent", "UnityWeb/1.0");
                }
            }

            if (GetHeader("Connection") == "")
            {
                SetHeader("Connection", "close");
            }

            // Basic Authorization
            if (!String.IsNullOrEmpty(uri.UserInfo))
            {
                SetHeader("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(uri.UserInfo)));
            }

            if (synchronous)
            {
                GetResponse();
            }
            else
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object t) {
                    GetResponse();
                }));
            }
        }

        public string Text
        {
            set { bytes = Encoding.UTF8.GetBytes(value); }
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
#if !UNITY_EDITOR
            System.Console.WriteLine("NET: SSL Cert: " + sslPolicyErrors.ToString());
#else
            Debug.LogWarning("SSL Cert Error: " + sslPolicyErrors.ToString ());
#endif
            return true;
        }

        void WriteToStream(Stream outputStream)
        {
            var stream = new BinaryWriter(outputStream);
            stream.Write(ASCIIEncoding.ASCII.GetBytes(method.ToUpper() + " " + uri.PathAndQuery + " " + protocol));
            stream.Write(EOL);

            foreach (string name in mHeaders.Keys)
            {
                foreach (string value in mHeaders[name])
                {
                    stream.Write(ASCIIEncoding.ASCII.GetBytes(name));
                    stream.Write(':');
                    stream.Write(ASCIIEncoding.ASCII.GetBytes(value));
                    stream.Write(EOL);
                }
            }

            stream.Write(EOL);

            if (bytes != null && bytes.Length > 0)
            {
                stream.Write(bytes);
            }
        }

        private static string[] sizes = { "B", "KB", "MB", "GB" };
        public string InfoString(bool verbose)
        {
            string status = isDone && response != null ? response.Status.ToString() : "---";
            string message = isDone && response != null ? response.Message : "Unknown";
            double size = isDone && response != null && response.Bytes != null ? response.Bytes.Length : 0.0f;

            int order = 0;
            while (size >= 1024.0f && order + 1 < sizes.Length)
            {
                ++order;
                size /= 1024.0f;
            }

            string sizeString = String.Format("{0:0.##}{1}", size, sizes[order]);

            string result = uri.ToString() + " [ " + method.ToUpper() + " ] [ " + status + " " + message + " ] [ " + sizeString + " ] [ " + responseTime + "ms ]";

            if (verbose && response != null)
            {
                result += "\n\nRequest Headers:\n\n" + String.Join("\n", GetHeaders().ToArray());
                result += "\n\nResponse Headers:\n\n" + String.Join("\n", response.GetHeaders().ToArray());

                if (response.Text != null)
                {
                    result += "\n\nResponse Body:\n" + response.Text;
                }
            }

            return result;
        }
    }
}
