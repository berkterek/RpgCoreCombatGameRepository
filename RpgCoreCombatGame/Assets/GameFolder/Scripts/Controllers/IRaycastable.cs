using RpgCoreCombatGame.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.Controllers
{
    public interface IRaycastable
    {
        bool HandleRaycast(PlayerController player);
        CursorTypeEnum CursorType { get; }
    }
}

