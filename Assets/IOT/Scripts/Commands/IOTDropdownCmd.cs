using BuddyOS.Command;
using BuddyOS;
using System;
using UnityEngine.UI;
namespace BuddyApp.IOT
{
    public class IOTDropdownCmd : ACommand
    {
        public IOTDropdownCmd(object iObject)
        {
            Parameters = new CommandParam();
            Parameters.Objects = new object[1] { iObject };
        }

        protected override void ExecuteImpl()
        {
            object[] lVal = (object[])Parameters.Objects[0];
            IOTNewDevice lVal0 = (IOTNewDevice)lVal[0];
            string lVal1 = (string)lVal[1];
            lVal0.IOTObject = (IOTObjects)Activator.CreateInstance(Type.GetType(lVal1));

            if (lVal0.IOTObject is IOTSystems)
            {
                lVal0.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetComponent<Image>().sprite = BYOS.Instance.SpriteManager.GetSprite(lVal0.IOTObject.SpriteName, "AtlasIOT");
                lVal0.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = lVal0.IOTObject.Name;
            }
            else if (lVal0.IOTObject is IOTDevices)
            {
                lVal0.transform.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetComponent<Image>().sprite = BYOS.Instance.SpriteManager.GetSprite(lVal0.IOTObject.SpriteName, "AtlasIOT");
                lVal0.transform.GetChild(0).GetChild(1).GetChild(1).GetChild(2).GetComponent<Text>().text = lVal0.IOTObject.Name;
            }

            lVal0.CleanParams(false);

            lVal0.FillParamClasses();
            lVal0.InitiliazeParameters();
        }
    }
}
