﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.CoreGames
{
    public class CameraFacing : MonoBehaviour
    {
        private void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}

