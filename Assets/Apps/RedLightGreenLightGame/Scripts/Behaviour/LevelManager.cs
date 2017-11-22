using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using System.IO;

namespace BuddyApp.RedLightGreenLightGame
{
    public class LevelManager : MonoBehaviour
    {
        public LevelData LevelData { get; private set; }

        private List<LevelData> mListLevels;

        // Use this for initialization
        void Start()
        {
            mListLevels = new List<LevelData>();

            if (Directory.GetFiles(BYOS.Instance.Resources.GetPathToRaw("Levels")) != null)
            {
                foreach (string lLevelfile in Directory.GetFiles(BYOS.Instance.Resources.GetPathToRaw("Levels")))
                {
                    if (!lLevelfile.EndsWith("xml"))
                        continue;

                    LevelData lLevel = Utils.UnserializeXML<LevelData>(lLevelfile);
                    Debug.Log("target: speed: " + lLevel.Target.Speed + " size: " + lLevel.Target.Size + " move: " + lLevel.Target.Movement);
                    mListLevels.Add(lLevel);
                }
            }

            mListLevels.Sort(delegate (LevelData x, LevelData y)
            {
                if (x == null && y == null) return 0;
                else if (x.Level>y.Level) return 1;
                else if (y.Level>x.Level) return -1;
                else return 0;
            });

            LevelData = mListLevels[0];
            
        }

        public void LevelUp()
        {
            int lLevelActual = LevelData.Level;
            if (lLevelActual < 3)
                LevelData = mListLevels[lLevelActual + 1];
            else
            {
                LevelData.Level++;
                LevelData.WaitingTime -= LevelData.WaitingTime / 10.0f;
                LevelData.Target.Speed += 3;
                LevelData.Target.Size /= 2;
                //TODO: modify detection time
            }
        }

        public void Reset()
        {
            LevelData = mListLevels[0];
        }
    }
}