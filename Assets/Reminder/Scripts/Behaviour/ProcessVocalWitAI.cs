using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using BuddyApp.Reminder.SimpleJSON;

namespace BuddyApp.Reminder
{
    public class ProcessVocalWitAI : IProcessVocal
    {
        public Command ExtractParameters(string iCommand)
        {
            Command lCommand = new Command();
            string lAnswer = NLP_Processing.ProcessWrittenText(iCommand);
            JSONNode NodeAnswer = JSON.Parse(lAnswer);
            Debug.Log("answer avec wit ai: " + lAnswer);
            Debug.Log("le intent: " + NodeAnswer["entities"]["intent"][0]["value"].ToString().Trim());

            if(NodeAnswer["entities"]["intent"][0]["value"].ToString().Contains("add"))
            {
                lCommand.Intent = Intent.ADD;
                Debug.Log("add lol mdr");
            }
            else if (NodeAnswer["entities"]["intent"][0]["value"].ToString().Contains("print"))
            {
                lCommand.Intent = Intent.PRINT;
                Debug.Log("add lol mdr");
            }

            if (NodeAnswer["entities"]["reminder"][0]["value"]!=null)
            {
                lCommand.Content = NodeAnswer["entities"]["reminder"][0]["value"];
                lCommand.Title = NodeAnswer["entities"]["reminder"][0]["value"];
            }

            if (NodeAnswer["entities"]["datetime"][0]["value"] != null)
            {
                DateTime lDate = DateTime.ParseExact(NodeAnswer["entities"]["datetime"][0]["value"], "yyyy-MM-dd'T'HH:mm:ss.fffzzz", CultureInfo.InvariantCulture,
                                       DateTimeStyles.AssumeUniversal |
                                       DateTimeStyles.AdjustToUniversal);
                Debug.Log("!!!!!!!!!!!!!!!!!date de now: " + lDate.ToString());
                lCommand.RemindDate = lDate;
                Debug.Log("date de now: " + lDate.ToString());
            }

            //lCommand.Intent = NodeAnswer["entities"]["intent"][0]["value"];
            //lCommand.Product = NodeAnswer["entities"]["product"][0]["value"];
            //lCommand.Quantity = NodeAnswer["entities"]["number"][0]["value"].AsInt;
            //lCommand.Unit = NodeAnswer["entities"]["unit"][0]["value"];
            //lCommand.Prep = NodeAnswer["entities"]["prep"][0]["value"];

            return lCommand;
        }
    }
}