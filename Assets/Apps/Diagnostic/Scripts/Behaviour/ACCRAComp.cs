using BlueQuark;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BuddyApp.Diagnostic
{
    public static class ACCRAComp
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void OnTrigger(Action iOnTrigger)
        {
            Buddy.Vocal.OnTrigger.Clear();
            Buddy.Vocal.OnTrigger.Add((iInput) => {
                if (iOnTrigger != null)
                    iOnTrigger();
            });
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Clear()
        {
            Buddy.Vocal.OnTrigger.Clear();
        }
    }
}
