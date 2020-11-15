﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health health;
        [SerializeField] RectTransform foreground;
        [SerializeField] Canvas rootCanvas;

        private void Update()
        {
            if(Mathf.Approximately(health.GetFraction(),0) || Mathf.Approximately(health.GetFraction(), 1))
            {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;
            foreground.localScale = new Vector3(health.GetFraction(), 1);
        }
    }
}

