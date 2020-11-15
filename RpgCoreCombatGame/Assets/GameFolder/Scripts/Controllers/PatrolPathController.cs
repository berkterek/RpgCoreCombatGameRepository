using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.Controllers
{
    public class PatrolPathController : MonoBehaviour
    {
        const float _waypointGizmosRadius = 0.35f;

        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWaypoint(i), _waypointGizmosRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        public int GetNextIndex(int i)
        {
            //eger i + 1 transform.childCount'dan esit ise bizim return 0 demis olduk
            if (i + 1 == transform.childCount)
            {
                return 0;
            }
            return i + 1;
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}
