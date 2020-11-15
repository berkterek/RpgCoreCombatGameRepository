using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.Saving2
{
    public interface ISaveable
    {
        object CaptureState();
        void RestoreState(object state);
    }
}
