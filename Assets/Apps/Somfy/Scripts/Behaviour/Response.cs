using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using Ionic.Zlib;

namespace BuddyApp.Somfy
{
    public class Response
    {
        private readonly Dictionary<string, List<string>> mHeaders;
        private Request mRequest;
        private int mStatus;
        private string mMessage;
        private byte[] mBytes;

        public Request Request { get { return mRequest; } set { mRequest = value; } }

        public int Status { get { return mStatus; } }

        public string Message { get { return mMessage; } }

        public byte[] Bytes { get { return mBytes; } }

        public string Text
        {
            get
            {
                if (Bytes == null)
                    return "";
                return UTF8Encoding.UTF8.GetString(Bytes);
            }
        }

        public string Asset
        {
            get
            {
                throw new NotSupportedException("This can't be done, yet.");
            }
        }

        public Hashtable Object
        {
            get
            {
                if (Bytes == null) {
                    return null;
                }

                bool lResult = false;
                Hashtable oObj = (Hashtable)JSON.JsonDecode(this.Text, ref lResult);
                if (!lResult) {
                    oObj = null;
                }

                return oObj;
            }
        }

        public ArrayList Array
        {
            get
            {
                if (Bytes == null)
                    return null;

                bool lResult = false;
                ArrayList oArray = (ArrayList)JSON.JsonDecode(this.Text, ref lResult);
                if (!lResult) {
                    oArray = null;
                }

                return oArray;
            }
        }

        public Response()
        {
            mMessage = "OK";
            mStatus = 200;
            mHeaders = new Dictionary<string, List<string>>();
            //ReadFromStream (stream);
        }

        public List<string> GetHeaders()
        {
            List<string> oResult = new List<string>();
            foreach (string lName in mHeaders.Keys) {
                foreach (string value in mHeaders[lName]) {
                    oResult.Add(lName + ": " + value);
                }
            }

            return oResult;
        }

        public List<string> GetHeaders(string iName)
        {
            iName = iName.ToLower().Trim();
            if (!mHeaders.ContainsKey(iName))
                return new List<string>();
            return mHeaders[iName];
        }

        public string GetHeader(string iName)
        {
            iName = iName.ToLower().Trim();
            if (!mHeaders.ContainsKey(iName))
                return string.Empty;
            return mHeaders[iName][mHeaders[iName].Count - 1];
        }

        public void ReadFromStream(Stream iInputStream)
        {
            //var inputStream = new BinaryReader(inputStream);
            string[] lTop = ReadLine(iInputStream).Split(new char[] { ' ' });

            if (!int.TryParse(lTop[1], out mStatus))
                throw new HTTPException("Bad Status Code");

            // MemoryStream is a disposable
            // http://stackoverflow.com/questions/234059/is-a-memory-leak-created-if-a-memorystream-in-net-is-not-closed
            using (MemoryStream lOutput = new MemoryStream()) {
                mMessage = string.Join(" ", lTop, 2, lTop.Length - 2);
                mHeaders.Clear();

                while (true) {
                    // Collect Headers
                    string[] lParts = ReadKeyValue(iInputStream);
                    if (lParts == null)
                        break;
                    AddHeader(lParts[0], lParts[1]);
                }

                if (Request.cookieJar != null) {
                    List<string> cookies = GetHeaders("set-cookie");
                    for (int cookieIndex = 0; cookieIndex < cookies.Count; ++cookieIndex) {
                        string cookieString = cookies[cookieIndex];
                        if (cookieString.IndexOf("domain=", StringComparison.CurrentCultureIgnoreCase) == -1) {
                            cookieString += "; domain=" + Request.uri.Host;
                        }

                        if (cookieString.IndexOf("path=", StringComparison.CurrentCultureIgnoreCase) == -1) {
                            cookieString += "; path=" + Request.uri.AbsolutePath;
                        }

                        Request.cookieJar.SetCookie(new Cookie(cookieString));
                    }
                }

                if (GetHeader("transfer-encoding") == "chunked") {
                    while (true) {
                        // Collect Body
                        int lLength = int.Parse(ReadLine(iInputStream), NumberStyles.AllowHexSpecifier);

                        if (lLength == 0) {
                            break;
                        }

                        for (int i = 0; i < lLength; i++) {
                            lOutput.WriteByte((byte)iInputStream.ReadByte());
                        }

                        //forget the CRLF.
                        iInputStream.ReadByte();
                        iInputStream.ReadByte();
                    }

                    while (true) {
                        //Collect Trailers
                        string[] lParts = ReadKeyValue(iInputStream);
                        if (lParts == null)
                            break;
                        AddHeader(lParts[0], lParts[1]);
                    }

                } else {
                    // Read Body
                    int lContentLength = 0;

                    try {
                        lContentLength = int.Parse(GetHeader("content-length"));
                    } catch {
                        lContentLength = 0;
                    }

                    int lB;
                    while ((lContentLength == 0 || lOutput.Length < lContentLength)
                              && (lB = iInputStream.ReadByte()) != -1) {
                        lOutput.WriteByte((byte)lB);
                    }

                    if (lContentLength > 0 && lOutput.Length != lContentLength) {
                        throw new HTTPException("Response length does not match content length");
                    }
                }

                if (GetHeader("content-encoding").Contains("gzip")) {
                    mBytes = UnZip(lOutput);
                } else {
                    mBytes = lOutput.ToArray();
                }
            }
        }

        private void AddHeader(string iName, string iValue)
        {
            iName = iName.ToLower().Trim();
            iValue = iValue.Trim();
            if (!mHeaders.ContainsKey(iName))
                mHeaders[iName] = new List<string>();
            mHeaders[iName].Add(iValue);
        }

        private string ReadLine(Stream iStream)
        {
            List<byte> lLine = new List<byte>();
            while (true) {
                int lChar = iStream.ReadByte();
                if (lChar == -1) {
                    throw new HTTPException("Unterminated Stream Encountered.");
                }
                if ((byte)lChar == Request.EOL[1])
                    break;
                lLine.Add((byte)lChar);
            }
            return ASCIIEncoding.ASCII.GetString(lLine.ToArray()).Trim();
        }

        private string[] ReadKeyValue(Stream iStream)
        {
            string lLine = ReadLine(iStream);
            if (lLine == "")
                return null;
            else {
                var split = lLine.IndexOf(':');
                if (split == -1)
                    return null;
                string[] lParts = new string[2];
                lParts[0] = lLine.Substring(0, split).Trim();
                lParts[1] = lLine.Substring(split + 1).Trim();
                return lParts;
            }
        }

        private byte[] UnZip(MemoryStream iOutput)
        {
            MemoryStream oCms = new MemoryStream();
            iOutput.Seek(0, SeekOrigin.Begin);
            using (var gz = new GZipStream(iOutput, CompressionMode.Decompress)) {
                var buf = new byte[1024];
                int byteCount = 0;
                while ((byteCount = gz.Read(buf, 0, buf.Length)) > 0) {
                    oCms.Write(buf, 0, byteCount);
                }
            }
            return oCms.ToArray();
        }
    }
}
