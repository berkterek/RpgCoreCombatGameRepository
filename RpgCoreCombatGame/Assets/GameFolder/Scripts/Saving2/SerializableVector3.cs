using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.Saving2
{
    [System.Serializable]
    public class SerializableVector3
    {
        float _x, _y, _z;

        public SerializableVector3(Vector3 vector3)
        {
            _x = vector3.x;
            _y = vector3.y;
            _z = vector3.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(_x, _y, _z);
        }
    }
}
