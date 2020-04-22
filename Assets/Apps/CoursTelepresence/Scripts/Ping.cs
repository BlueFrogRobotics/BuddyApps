using UnityEngine;
using System.Net.NetworkInformation;
using System.Text;

namespace BuddyApp.CoursTelepresence
{
    public class Ping : MonoBehaviour
    {

        private System.Net.NetworkInformation.Ping mPingSender;
        private PingOptions mPingOptions;
        private string mDataToSend;
        private byte[] mBuffer;
        private int mTimeOut;
        private PingReply mPingReply;

        [SerializeField]
        private float RefreshPing;
        private float mTimer;

        [SerializeField]
        private string AdressToConnect;

        // Use this for initialization
        void Start()
        {
            mTimer = 0F;
            mPingOptions = new PingOptions();
            mPingSender = new System.Net.NetworkInformation.Ping();
            // Create a 32 bytes array
            mDataToSend = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            mTimeOut = 120;

            // Adress for the test, here is the ip from Google
            AdressToConnect = "8.8.8.8";

            //Don't fragment the packet in multiple packets
            mPingOptions.DontFragment = true;
            mBuffer = Encoding.ASCII.GetBytes(mDataToSend);
            mPingReply = mPingSender.Send(AdressToConnect, mTimeOut, mBuffer, mPingOptions);
            RequestStatus();
        }

        private void Update()
        {
            mTimer += Time.deltaTime;
            if(mTimer > RefreshPing)
            {
                mTimer = 0F;
                mPingReply = mPingSender.Send(AdressToConnect, mTimeOut, mBuffer, mPingOptions);
                RequestStatus();
            }
        }

        /// <summary>
        /// All the status 
        /// </summary>
        private void RequestStatus()
        {
            switch (mPingReply.Status)
            {
                case IPStatus.Success:
                    Debug.LogWarning(mPingReply.RoundtripTime + " ms");
                    CoursTelepresenceData.Instance.Ping = mPingReply.RoundtripTime + " ms";
                    break;
                case IPStatus.DestinationHostUnreachable:
                    Debug.LogWarning("The ICMP echo request failed because the destination computer is not reachable. IPSTATUS ERROR : 11003");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.DestinationNetworkUnreachable:
                    Debug.LogWarning("The ICMP echo request failed because the network that contains the destination computer is not reachable. IPSTATUS ERROR : 11002");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.BadDestination:
                    Debug.LogWarning("The ICMP echo request failed because the destination IP address cannot receive ICMP echo requests or should never appear in the destination address field of any IP datagram. For example, calling Send and specifying IP address \"000.0.0.0\" returns this status.. IPSTATUS ERROR : 11018");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.BadHeader:
                    Debug.LogWarning("The ICMP echo request failed because the header is invalid. IPSTATUS ERROR : 11042");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.BadOption:
                    Debug.LogWarning("The ICMP echo request failed because it contains an invalid option. IPSTATUS ERROR : 11007");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.BadRoute:
                    Debug.LogWarning("The ICMP echo request failed because there is no valid route between the source and destination computers. IPSTATUS ERROR : 11012");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.DestinationPortUnreachable:
                    Debug.LogWarning("The ICMP echo request failed because the port on the destination computer is not available. IPSTATUS ERROR : 11005");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.DestinationProhibited:
                    Debug.LogWarning("The ICMPv6 echo request failed because contact with the destination computer is administratively prohibited. This value applies only to IPv6. IPSTATUS ERROR : 11004");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                //case IPStatus.DestinationProtocolUnreachable:
                //    Debug.LogWarning("The ICMP echo request failed because the destination computer that is specified in an ICMP echo message is not reachable, because it does not support the packet's protocol. This value applies only to IPv4. This value is described in IETF RFC 1812 as Communication Administratively Prohibited. IPSTATUS ERROR : 11004");
                //    break;
                case IPStatus.DestinationScopeMismatch:
                    Debug.LogWarning("The ICMP echo request failed because the source address and destination address that are specified in an ICMP echo message are not in the same scope. This is typically caused by a router forwarding a packet using an interface that is outside the scope of the source address. Address scopes (link-local, site-local, and global scope) determine where on the network an address is valid. IPSTATUS ERROR : 11045");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.DestinationUnreachable:
                    Debug.LogWarning("The ICMP echo request failed because the destination computer that is specified in an ICMP echo message is not reachable; the exact cause of problem is unknown. IPSTATUS ERROR : 11040");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.HardwareError:
                    Debug.LogWarning("The ICMP echo request failed because of a hardware error. IPSTATUS ERROR : 11008");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.IcmpError:
                    Debug.LogWarning("The ICMP echo request failed because of an ICMP protocol error. IPSTATUS ERROR : 11044");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.NoResources:
                    Debug.LogWarning("The ICMP echo request failed because of insufficient network resources. IPSTATUS ERROR : 11006");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.PacketTooBig:
                    Debug.LogWarning("The ICMP echo request failed because the packet containing the request is larger than the maximum transmission unit (MTU) of a node (router or gateway) located between the source and destination. The MTU defines the maximum size of a transmittable packet. IPSTATUS ERROR : 11009");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.ParameterProblem:
                    Debug.LogWarning("The ICMP echo request failed because a node (router or gateway) encountered problems while processing the packet header. This is the status if, for example, the header contains invalid field data or an unrecognized option. IPSTATUS ERROR : 11015");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.SourceQuench:
                    Debug.LogWarning("The ICMP echo request failed because the packet was discarded. This occurs when the source computer's output queue has insufficient storage space, or when packets arrive at the destination too quickly to be processed. IPSTATUS ERROR : 11016");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.TimedOut:
                    Debug.LogWarning("The ICMP echo Reply was not received within the allotted time. The default time allowed for replies is 5 seconds. You can change this value using the Send or SendAsync methods that take a timeout parameter. IPSTATUS ERROR : 11010");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.TimeExceeded:
                    Debug.LogWarning("The ICMP echo request failed because its Time to Live (TTL) value reached zero, causing the forwarding node (router or gateway) to discard the packet. IPSTATUS ERROR : 11041");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.TtlExpired:
                    Debug.LogWarning("The ICMP echo request failed because its Time to Live (TTL) value reached zero, causing the forwarding node (router or gateway) to discard the packet. IPSTATUS ERROR : 11013");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.TtlReassemblyTimeExceeded:
                    Debug.LogWarning("The ICMP echo request failed because the packet was divided into fragments for transmission and all of the fragments were not received within the time allotted for reassembly. RFC 2460 specifies 60 seconds as the time limit within which all packet fragments must be received. IPSTATUS ERROR : 11014");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.Unknown:
                    Debug.LogWarning("The ICMP echo request failed for an unknown reason. IPSTATUS ERROR : -1");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                case IPStatus.UnrecognizedNextHeader:
                    Debug.LogWarning("The ICMP echo request failed because the Next Header field does not contain a recognized value. The Next Header field indicates the extension header type (if present) or the protocol above the IP layer, for example, TCP or UDP. IPSTATUS ERROR : 11043");
                    CoursTelepresenceData.Instance.Ping = "Error";
                    break;
                default:
                    Debug.LogWarning("Default case ping");
                    break;
            }
        }
    }
}


