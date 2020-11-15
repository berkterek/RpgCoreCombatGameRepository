using RpgCoreCombatGame.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RpgCoreCombatGame.Combats
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Text _healthValueText;
        Fighter _fighter;

        private void Awake()
        {
            _fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            _healthValueText = GetComponent<Text>();
        }

        private void Update()
        {
            if(_fighter.Target == null)
            {
                _healthValueText.text = "N/A";
            }
            else
            {
                Health health = _fighter.Target;
                _healthValueText.text = health.GetPercentage().ToString("0")+"%";
            }
        }
    }
}

