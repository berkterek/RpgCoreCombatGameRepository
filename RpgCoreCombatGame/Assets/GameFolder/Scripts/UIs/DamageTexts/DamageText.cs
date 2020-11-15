using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RpgCoreCombatGame.UIs.DamageTexts
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] Text damageText;

        public void DestroyTakeDamageObject()
        {
            Destroy(this.gameObject);
        }

        public void EnableThisObject()
        {
            this.gameObject.SetActive(false);
        }

        public void SetValue(float amount)
        {
            //damageText.text = System.String.Format("{0:F2}",amount);
            damageText.text = amount.ToString("F2");
        }
    }
}

