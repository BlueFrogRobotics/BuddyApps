using Buddy;
using Buddy.Command;
using Buddy.UI;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Boot
{
    public class BootBehaviour : MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5F);
            new StartAppCmd("o201104109524").Execute();
        }
    }
}