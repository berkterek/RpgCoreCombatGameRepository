using RpgCoreCombatGame.Attributes;
using RpgCoreCombatGame.Controllers;
using RpgCoreCombatGame.Enums;
using UnityEngine;

namespace RpgCoreCombatGame.Combats
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorTypeEnum CursorType => CursorTypeEnum.Combat;

        public bool HandleRaycast(PlayerController player)
        {
            if (!player.GetComponent<Fighter>().CanAttack(this.gameObject))
            {
                return false;
            }

            if (Input.GetMouseButton(0))
            {
                player.GetComponent<Fighter>().Attack(this.gameObject);
            }

            return true;
        }
    }
}
