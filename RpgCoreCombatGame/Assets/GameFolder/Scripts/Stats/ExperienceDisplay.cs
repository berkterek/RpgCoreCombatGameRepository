using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RpgCoreCombatGame.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Text _experienceText;
        Experience _experience;

        private void Awake()
        {
            _experienceText = GetComponent<Text>();
            _experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            _experienceText.text = _experience.ExperiencePoint.ToString();
        }
    }
}

