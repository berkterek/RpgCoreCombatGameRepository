using RpgCoreCombatGame.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.Stats
{
    public interface IModifierProvider
    {
        IEnumerable<float> GetAdditiveModifier(StatsEnum stat);
        IEnumerable<float> GetPercentageModifier(StatsEnum stat);
    }
}

