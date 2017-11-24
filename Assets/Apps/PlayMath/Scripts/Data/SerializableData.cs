using System;
using System.Runtime.Serialization;

namespace BuddyApp.PlayMath{

    [DataContract]
    abstract public class SerializableData : IExtensibleDataObject {

        public SerializableData()
        {
        }

        // Implement IExtensibleData
        protected ExtensionDataObject extensionData_Value;

        public ExtensionDataObject ExtensionData
        {
            get{ return extensionData_Value; }
            set{ extensionData_Value = value; }
        }
    }
}

