using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RpgCoreCombatGame.Stats
{
    public class DisplayLevel : MonoBehaviour
    {
        Text _levelText;
        BaseStat _baseStat;

        private void Awake()
        {
            _levelText = GetComponent<Text>();
            _baseStat = GameObject.FindWithTag("Player").GetComponent<BaseStat>();
        }

        private void Update()
        {
            _levelText.text = _baseStat.GetLevel().ToString();
        }
    }
}

