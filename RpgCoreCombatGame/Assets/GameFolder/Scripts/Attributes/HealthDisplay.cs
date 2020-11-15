using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RpgCoreCombatGame.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Text _healthValueText;
        Health _health;

        private void Awake()
        {
            _health = GameObject.FindWithTag("Player").GetComponent<Health>();
            _healthValueText = GetComponent<Text>();
        }

        private void Update()
        {
            _healthValueText.text = _health.GetPercentage().ToString("0") + "%";
        }
    }
}

