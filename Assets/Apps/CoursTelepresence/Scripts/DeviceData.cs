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
        public string Eleve;
        public string Device;
        public string Date_Debut;
        [JsonProperty(PropertyName = "Device.Uid")]
        public string DeviceUID;
        public int idPlanning;
        [JsonProperty(PropertyName = "Eleve.idUser")]
        public int EleveIdUser;
        public string ID;
        [JsonProperty(PropertyName = "Prof.idUser")]
        public int ProfId;
        public string Prof;
    }

    [Serializable]
    public class PlanningList
    {
        public Planning[] Planning;
    }

    public class DeviceUserLiaison
    {
        public string User;
        [JsonProperty(PropertyName = "Device.Reference")]
        public string DeviceReference;
        [JsonProperty(PropertyName = "User.Prenom")]
        public string UserPrenom;
        [JsonProperty(PropertyName = "Device.Organisme")]
        public string DeviceOrganisme;
        [JsonProperty(PropertyName = "User.Type_user")]
        public string UserType_user;
        [JsonProperty(PropertyName = "User.idUser")]
        public int UserIdUser;
        [JsonProperty(PropertyName = "User.Nom")]
        public string UserNom;
        [JsonProperty(PropertyName = "Device.Etat")]
        public string DeviceEtat;
        [JsonProperty(PropertyName = "Device.idDevice")]
        public int DeviceIdDevice;
        [JsonProperty(PropertyName = "Device.Nom1")]
        public string DeviceNom1;
        public string Groupes_Users;
        [JsonProperty(PropertyName = "Device.Ville")]
        public string DeviceVille;
        public string ID;
        [JsonProperty(PropertyName = "User.Organisme")]
        public string UserOrganisme;
        public int idDevice_User;
        [JsonProperty(PropertyName = "Device.Position_GPS")]
        public string DevicePosition_GPS;
        [JsonProperty(PropertyName = "Device.Type_organisme")]
        public string DeviceType_organisme;
        public string Device;
        [JsonProperty(PropertyName = "Device.Type_device")]
        public string DeviceType_device;
        [JsonProperty(PropertyName = "Device.Uid")]
        public string DeviceUid;
        [JsonProperty(PropertyName = "Device.idOrganisme")]
        public string DeviceIdOrganisme;
        [JsonProperty(PropertyName = "Device.Qualite_signal")]
        public string DeviceQualite_signal;
        [JsonProperty(PropertyName = "Device.Batterie")]
        public string DeviceBatterie;
        [JsonProperty(PropertyName = "User.Identifiant")]
        public string UserIdentifiant;
        [JsonProperty(PropertyName = "Planning.idPlanning")]
        public string PlanningIdPlanning;
        [JsonProperty(PropertyName = "Device.Nom")]
        public string DeviceNom;
        public string Groupes_fabricants;
    }

    public class DeviceUserLiaisonList
    {
        public DeviceUserLiaison[] Device_user;
    }
}

