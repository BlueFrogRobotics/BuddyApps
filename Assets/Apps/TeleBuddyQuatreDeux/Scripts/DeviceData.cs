using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace BuddyApp.TeleBuddyQuatreDeux
{

    [Serializable]
    public class DeviceData
    {
        public string Etat;
        public string Position_GPS;
        public string Batterie;
        [JsonProperty(PropertyName = "Type_device.idType_Device")]
        public string IdType_device;
        public int idDevice;
        public string Qualite_signal;
        public string Nom;
        public string Uid;
        [JsonProperty(PropertyName = "Organisme.Reference")]
        public string OrganismeRef;
        public string Organisme;
        public string Type_device;
        [JsonProperty(PropertyName = "Groupes_Users.nom_groupe")]
        public string Nom_Groupe_User;
        public string ID;
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
        public string Modified_Time;
        public string Organisme;
        public string ID;
        public string Nom;
        public string Identifiant;
        public string Password;
        public string Planning;
        public bool NeedPlanning;
        public string RTMToken;
        public string RTCToken;
        public string AppID;

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
        public int? DeviceId;
        public string Eleve;
        public string Device;
        public string Date_Debut;
        [JsonProperty(PropertyName = "Device.Uid")]
        public string DeviceUID;
        public int? idPlanning;
        [JsonProperty(PropertyName = "Eleve.idUser")]
        public int? EleveIdUser;
        public string ID;
        [JsonProperty(PropertyName = "Prof.idUser")]
        public int? ProfId;
        public string Prof;
    }

    [Serializable]
    public class PlanningList
    {
        public Planning[] Planning;
    }

    [Serializable]
    public class DeviceUserLiaison
    {
        public string User;

        [JsonProperty(PropertyName = "User.Prenom")]
        public string UserPrenom;

        [JsonProperty(PropertyName = "Device.Organisme")]
        public string DeviceOrganisme;

        [JsonProperty(PropertyName = "User.Type_user")]
        public string UserType_user;

        [JsonProperty(PropertyName = "Device.Organisme_Reference")]
        public string DeviceOrganisme_Reference;

        [JsonProperty(PropertyName = "User.idUser")]
        public int? UserIdUser;

        [JsonProperty(PropertyName = "Device.Organisme_Ville")]
        public string DeviceOrganisme_Ville;

        [JsonProperty(PropertyName = "User.Nom")]
        public string UserNom;

        //[JsonProperty(PropertyName = "Device.Etat")]
        //public string DeviceEtat;

        [JsonProperty(PropertyName = "Device.idDevice")]
        public int? DeviceIdDevice;

        [JsonProperty(PropertyName = "Device.Organisme_Type_organisme")]
        public string DeviceOrganisme_Type_organisme;

        public string ID;

        //public string Groupes_Users;

        [JsonProperty(PropertyName = "User.Organisme")]
        public string UserOrganisme;

        public int? idDevice_User;

        public string Modified_User;

        [JsonProperty(PropertyName = "Device.Position_GPS")]
        public string DevicePosition_GPS;

        public string Modified_Time;

        [JsonProperty(PropertyName = "Device.AppID")]
        public string DeviceAppID;

        public string Device;

        [JsonProperty(PropertyName = "Device.Type_device")]
        public string DeviceType_device;

        [JsonProperty(PropertyName = "Device.Uid")]
        public string DeviceUid;

        [JsonProperty(PropertyName = "Device.Organisme_Nom")]
        public string DeviceOrganisme_Nom;

        [JsonProperty(PropertyName = "Device.Qualite_signal")]
        public string DeviceQualite_signal;

        [JsonProperty(PropertyName = "Device.Batterie")]
        public string DeviceBatterie;

        [JsonProperty(PropertyName = "User.Identifiant")]
        public string UserIdentifiant;

        [JsonProperty(PropertyName = "Device.Device_RTC")]
        public string DeviceRTC;

        [JsonProperty(PropertyName = "Planning.idPlanning")]
        public string PlanningidPlanning;

        [JsonProperty(PropertyName = "Device.Organisme_Organisme_Parent")]
        public string DeviceOrganisme_Organisme_Parent;

        [JsonProperty(PropertyName = "Device.Nom")]
        public string DeviceNom;

        [JsonProperty(PropertyName = "Device.Need_planning")]
        public bool DeviceNeedPlanning;

        //public string Groupes_fabricants;

        [JsonProperty(PropertyName = "Device.Device_RTM")]
        public string DeviceRTM;

        [JsonProperty(PropertyName = "Device.Organisme_idOrganisme1")]
        public string DeviceOrganisme_idOrganisme1;


    }

    [Serializable]
    public class DeviceUserLiaisonList
    {
        public DeviceUserLiaison[] Device_user;
    }
}

