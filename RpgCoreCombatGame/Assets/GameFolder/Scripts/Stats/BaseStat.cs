using GameDevTV.Utils;
using RpgCoreCombatGame.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.Stats
{
    public class BaseStat : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClassEnum characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect;
        [SerializeField] bool shouldUseModifiers = false;

        public event System.Action OnLevelUpEvent;

        Experience _experience;
        LazyValue<int> _currentLevel;
        

        //bu property Health awake icinde cagirilir
        //public float Heath => progression.GetStat(StatsEnum.Health,characterClass,startingLevel);

        ////bu property Health component icinde RewardExperience method'u icinde cagirilir
        //public float GetExperienceReward => progression.GetStat(StatsEnum.ExperienceReward, characterClass, startingLevel);
        private void Awake()
        {
            _experience = GetComponent<Experience>();
            _currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void OnEnable()
        {
            if (_experience != null)
            {
                _experience.OnExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (_experience != null)
            {
                _experience.OnExperienceGained -= UpdateLevel;
            }
        }

        private void Start()
        {
            _currentLevel.ForceInit();
        }

        private void UpdateLevel()
        {
            int newLevel = (int)CalculateLevel();

            if (newLevel > _currentLevel.Value)
            {
                _currentLevel.Value = newLevel;
                LevelUpEffect();
                OnLevelUpEvent();
            }
        }

        void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect,transform);
        }

        //Health component icinde cagiriliyor
        public float GetStat(StatsEnum stat)
        {
            //float result = (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
            //Debug.Log(result);
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        private float GetBaseStat(StatsEnum stat)
        {
            return progression.GetStat(stat, characterClass, (int)GetLevel());
        }

        private int CalculateLevel()
        {
            if (_experience == null) return startingLevel;
            
            float currentXP = _experience.ExperiencePoint;

            int penultimateLevel = progression.GetLevels(StatsEnum.ExperienceToLevelUp, characterClass);

            for (int i = 1; i <= penultimateLevel; i++)
            {
                float xpToLevelUp = progression.GetStat(StatsEnum.ExperienceToLevelUp, characterClass, i);

                if (currentXP < xpToLevelUp)
                {
                    return i;
                }
            }

            return penultimateLevel + 1;
        }

        private float GetAdditiveModifier(StatsEnum stat)
        {
            if (!shouldUseModifiers) return 0f;

            float total = 0f;
            var components = GetComponents<IModifierProvider>();
            foreach (IModifierProvider provider in components)
            {
                var modifiers = provider.GetAdditiveModifier(stat);
                foreach (float modifier in modifiers)
                {
                    total += modifier;
                }
            }

            return total;
        }

        private float GetPercentageModifier(StatsEnum stat)
        {
            if (!shouldUseModifiers) return 0f;
            float total = 0f;
            var components = GetComponents<IModifierProvider>();
            foreach (IModifierProvider provider in components)
            {
                var modifiers = provider.GetPercentageModifier(stat);
                foreach (float modifier in modifiers)
                {
                    total += modifier;
                }
            }

            return total;
        }

        public int GetLevel()
        {
            return _currentLevel.Value;
        }
    }
}
