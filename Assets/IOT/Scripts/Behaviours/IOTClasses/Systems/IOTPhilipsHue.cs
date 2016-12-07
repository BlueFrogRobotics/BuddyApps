using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS.UI;
namespace BuddyApp.IOT
{
    public class IOTPhilipsHue : IOTSystems
    {
        private List<IOTPhilipsLightHUE> mLights = new List<IOTPhilipsLightHUE>();

        public override void InitializeParams()
        {
            GameObject lSearch = InstanciateParam(ParamType.TEXTFIELD);
            SearchField lSearchComponent = lSearch.GetComponent<SearchField>();

            TextFieldCmd lCmd = new TextFieldCmd();
            lCmd.Parameters.Objects = new object[1];
            lCmd.Parameters.Objects[0] = this;
            lCmd.Parameters.Integers = new int[1];
            lCmd.Parameters.Integers[0] = 0;
            lSearchComponent.UpdateCommands.Add(lCmd);
        }

        public override void Connect()
        {
            AskLightsCount();
        }


        private void AskLightsCount()
        {
            HTTP.Request theRequest = new HTTP.Request("GET", "http://" + Credentials[0] + "/api/" + Credentials[1] + "/lights");
            theRequest.Send((request) =>
            {
                Hashtable result = request.response.Object;
                mLights = new List<IOTPhilipsLightHUE>(result.Count);
                for (int i = 0; i < mLights.Count; ++i)
                {
                    mLights[i].Credentials[0] = Credentials[0];
                    mLights[i].Credentials[1] = Credentials[1];
                    mLights[i].Credentials[2] = Credentials[2];
                    mLights[i].Indice = i;
                }

                if (result == null)
                {
                    return;
                }
            });
        }
        
        public void OnOffForAll(bool iState)
        {
            for(int i = 0; i < mLights.Count; ++i)
                mLights[i].OnOff(iState);
        }
    }
}
