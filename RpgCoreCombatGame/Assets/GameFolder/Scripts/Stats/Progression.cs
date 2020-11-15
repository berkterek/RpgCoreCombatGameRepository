using RpgCoreCombatGame.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.Stats
{
    [CreateAssetMenu(fileName ="Progression",menuName ="Stats/New Progression",order =0)]
    public class Progression : ScriptableObject
    {
        //bu field bizim inspector uzerinde elle bilgileri girmemize yariyan field'imizdir
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        //bu dictionary icinde iki value tarafinda da dictionary kullandik _lookupTable field'ini bir kere build ederiz inspector'dan gelen bilgilere gore sonra key'lerinden direk value degerlerini cagirirz
        Dictionary<CharacterClassEnum, Dictionary<StatsEnum, float[]>> _lookupTable = null;

        public float GetStat(StatsEnum stat, CharacterClassEnum characterClass, int level)
        {
            BuildLookup();

            Dictionary<StatsEnum, float[]> levelInfos = _lookupTable[characterClass];

            float[] levels = levelInfos[stat];

            if (levels.Length < level) return 0f;

            return levels[level - 1];
        }

        //BuildLookup method tek sefer calisir once null degilse return eder eger null ise once instance islemi yapariz ve instance tan sonra _lookupTable icini doldururz
        private void BuildLookup()
        {
            if (_lookupTable != null) return;

            _lookupTable = new Dictionary<CharacterClassEnum, Dictionary<StatsEnum, float[]>>();

            //characterClasses icinde doneriz cunku inspector uzerinde bilgisleri saklayan list yapimizdir
            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                var statLookupTable = new Dictionary<StatsEnum, float[]>();

                //character bilgilerine gore stats ve level bilgilerini dictionary icine ekleriz
                foreach (ProgressionStat progressionStat in progressionClass.Stats)
                {
                    statLookupTable[progressionStat.Stat] = progressionStat.Levels; 
                }

                _lookupTable[progressionClass.CharacterClass] = statLookupTable;
            }
        }

        public int GetLevels(StatsEnum stat, CharacterClassEnum characterClass)
        {
            BuildLookup();

            float[] levels = _lookupTable[characterClass][stat];
            return levels.Length;
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            [SerializeField] CharacterClassEnum characterClass;
            [SerializeField] ProgressionStat[] stats;

            public CharacterClassEnum CharacterClass => characterClass;
            public ProgressionStat[] Stats => stats;
        }

        [System.Serializable]
        class ProgressionStat
        {
            [SerializeField] StatsEnum stat;
            [SerializeField] float[] levels;

            public StatsEnum Stat => stat;
            public float[] Levels => levels;
        }
    }
}

