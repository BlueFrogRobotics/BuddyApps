using UnityEngine;
using System.Collections;
namespace BuddyApp.IOT
{
    public class IOTPhilipsHue : IOTSystems
    {
        public override void initializeParams()
        {
            GameObject lButton = instanciateParam(ParamType.BUTTON);
        }
    }
}
