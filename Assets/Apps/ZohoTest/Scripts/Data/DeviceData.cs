using BlueQuark;
using UnityEngine;
using Newtonsoft.Json;

namespace BuddyApp.ZohoTest
{
    [System.Serializable]
    public class DeviceData
    {
        public string Uid;
        public string ID;
        public string Etat;
        public string Organisme;
        public string Nom;
        public int idDevice;
    }

    [System.Serializable]
    public class DevicesCollection
    {
        public DeviceData[] Device;
    }

    [System.Serializable]
    public class DeviceUserData
    {
        [JsonProperty(PropertyName = "User.idUser")]
        public int User;
        [JsonProperty(PropertyName = "Device.idDevice")]
        public int Device;
    }

    [System.Serializable]
    public class DeviceUserCollection
    {
        public DeviceUserData[] Device_user;
    }

    [System.Serializable]
    public class TypeDeviceData
    {
        public string ID;
        public string Type_Device;
    }

    [System.Serializable]
    public class TypeDeviceCollection
    {
        public TypeDeviceData[] Type_device;
    }
}
