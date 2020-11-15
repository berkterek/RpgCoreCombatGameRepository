using RpgCoreCombatGame.Saving2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.Stats
{
    public class Experience : MonoBehaviour,ISaveable
    {
        [SerializeField] float experiencePoint = 0f;

        public event System.Action OnExperienceGained;

        public float ExperiencePoint => experiencePoint;

        public void GainExperience(float experience)
        {
            experiencePoint += experience;

            OnExperienceGained();

            RestoreState(experiencePoint);
        }

        public object CaptureState()
        {
            return experiencePoint;
        }

        public void RestoreState(object state)
        {
            experiencePoint = (float)state;
        }
    }
}
