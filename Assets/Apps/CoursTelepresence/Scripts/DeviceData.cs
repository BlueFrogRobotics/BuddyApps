﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace BuddyApp.CoursTelepresence
{

    [Serializable]
    public class DeviceData
    {
        public string Uid;
        public string ID;
        public string Etat;
        public string Organisme;
        public string Nom;
        public int idDevice;
        public string Batterie;
        public string Qualite_signal;
    }

    [Serializable]
    public class DevicesCollection
    {
        public DeviceData[] Device;
    }

    [Serializable]
    public class DeviceUserData
    {
        [JsonProperty(PropertyName = "User.idUser")]
        public int User;
        [JsonProperty(PropertyName = "Device.idDevice")]
        public int Device;
    }

    [Serializable]
    public class DeviceUserCollection
    {
        public DeviceUserData[] Device_user;
    }

    [Serializable]
    public class TypeDeviceData
    {
        public string ID;
        public string Type_Device;
    }

    [Serializable]
    public class TypeDeviceCollection
    {
        public TypeDeviceData[] Type_device;
    }
}

