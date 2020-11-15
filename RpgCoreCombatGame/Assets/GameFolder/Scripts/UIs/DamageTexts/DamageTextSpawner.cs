using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.UIs.DamageTexts
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText prefab;

        public void Spawn(float damageAmount)
        {
            DamageText instance = Instantiate<DamageText>(prefab,transform);
            instance.SetValue(damageAmount);
        }
    }
}

