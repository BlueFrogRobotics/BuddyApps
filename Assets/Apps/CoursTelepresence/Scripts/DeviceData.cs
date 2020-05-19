using System.Collections;
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
        public string Etat;
        public string Position_GPS;
        public string Batterie;
        [JsonProperty(PropertyName = "Organisme.Nom")]
        public string OrganismeName;
        public string Organisme;
        public string Type_device;
        public string ID;
        public int idDevice;
        public string Qualite_signal;
        public string Nom;
        [JsonProperty(PropertyName = "Organisme.idOrganisme")]
        public string OrganismeID;
    }

    [Serializable]
    public class DevicesCollection
    {
        public DeviceData[] Device;
    }

    [Serializable]
    public class DeviceUser
    {
        [JsonProperty(PropertyName = "Device.idDevice")]
        public int DeviceId;
        public string User;
        public string Device;
        public string ID;
        [JsonProperty(PropertyName = "User.idUser")]
        public int UserId;
    }

    [Serializable]
    public class DeviceUserCollection
    {
        public DeviceUser[] Device_user;
    }

    [Serializable]
    public class User
    {
        public string Prenom;
        public int idUser;
        public DateTime Modified_Time;
        public string Organisme;
        public string ID;
        public string Nom;
        public string Identifiant;
        public string Password;

    }
    [Serializable]
    public class UserList
    {
        public User[] User;

    }

    [Serializable]
    public class Planning
    {
        public string Date_Fin;
        [JsonProperty(PropertyName = "Device.idDevice")]
        public int DeviceId;
        public string User;
        public string Device;
        public string Date_Debut;
        public int idPlanning;
        public string ID;
        [JsonProperty(PropertyName = "User.idUser")]
        public int UserId;
        public string Prof;
    }

    [Serializable]
    public class PlanningList
    {
        public Planning[] Planning;
    }
}

