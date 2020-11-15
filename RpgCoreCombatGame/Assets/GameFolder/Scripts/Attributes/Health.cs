using GameDevTV.Utils;
using RpgCoreCombatGame.CoreGames;
using RpgCoreCombatGame.Saving2;
using RpgCoreCombatGame.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace RpgCoreCombatGame.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regenerationPercentage = 70f;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        //bu class'in bir tek amaci var UnityEvent<T> direk inspector icinde generic olarak gozukmedigi icin boyle bir yonteme bas vuruldu bir subclass olusturuludu ve UnityEvent<T> den miras aldi ve inspector icinde bu subclass'i vermis olduk cunku paramere olarak float yani damage'i event icinde listener olan class'a gecirmek istiyoruz
        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
            
        }

        bool _isDeath = false;
        BaseStat _baseStat;

        LazyValue<float> _healthPoints;

        private void Awake()
        {
            _baseStat = GetComponent<BaseStat>();
            _healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private void OnEnable()
        {
            _baseStat.OnLevelUpEvent += RegenerateHealth;
        }

        private void OnDisable()
        {
            _baseStat.OnLevelUpEvent -= RegenerateHealth;
        }

        private void Start()
        {
            EnableNavMeshAgent();
            _healthPoints.ForceInit();
        }

        private float GetInitialHealth()
        {
            //if (_isDeath)
            //{
            //    _healthPoints.Value = 0f;
            //}
            //else
            //{
            //    _healthPoints.Value = _baseStat.GetStat(Enums.StatsEnum.Health); //basestat baslangic durumu anlami demektir butun oyuncu ve enemy butun oyuncularin level ve saglik bilgilerini tasir health oyun basladiginda basestat'den alir
            //}

            return _baseStat.GetStat(Enums.StatsEnum.Health);
        }

        private void RegenerateHealth()
        {
            float regenerateHealth = _baseStat.GetStat(Enums.StatsEnum.Health) * (regenerationPercentage / 100);
            _healthPoints.Value = Mathf.Max(_healthPoints.Value, regenerateHealth);
        }

        private void EnableNavMeshAgent()
        {
            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();

            if (navMeshAgent != null)
            {
                if (navMeshAgent.enabled == false)
                {
                    navMeshAgent.enabled = true;
                }
            }
        }

        //TakeDamage method'u Figther Component icinde Hit Animation method'u icinde cagiriliyor
        public void TakeDamage(GameObject instigator, float damage)
        {
            _healthPoints.Value = Mathf.Max(_healthPoints.Value - damage, 0);
            Debug.Log("Health:" + _healthPoints.Value + " Owns: " + transform.name);

            takeDamage.Invoke(damage);

            if (_healthPoints.Value <= 0 && !_isDeath)
            {
                onDie.Invoke();
                TriggerDeathAndAnimation();
                RewardExperience(instigator);
            }
        }

        //karakterlerin can bilgisini 100'de olarak canvas icindeki text gonderen method'dur
        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return (_healthPoints.Value / GetMaxHealthPoint());
        }

        private void TriggerDeathAndAnimation()
        {
            _isDeath = true;
            GetComponent<Animator>().SetTrigger("death");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        //bu method Figther.cs icinde Update method'unda cagiriliyor
        public bool IsDeath()
        {
            return _isDeath;
        }

        //Saving2 icinden gelen ISaveable method'u
        public object CaptureState()
        {
            return _healthPoints.Value;
        }

        //WeaponPickup icinde cagirlir eger heal yerden alirsak bu method tetiklenir
        public void Heal(float restoreHeal)
        {
            _healthPoints.Value = Mathf.Min(_healthPoints.Value + restoreHeal, GetMaxHealthPoint());
        }

        private float GetMaxHealthPoint()
        {
            return _baseStat.GetStat(Enums.StatsEnum.Health);
        }

        //Saving2 icinden gelen ISaveable method'u
        public void RestoreState(object state)
        {
            _healthPoints.Value = (float)state;

            if (_healthPoints.Value <= 0)
            {
                TriggerDeathAndAnimation();
            }
            else
            {
                _isDeath = false;
            }
        }

        private void RewardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();

            if (experience == null) return;

            experience.GainExperience(_baseStat.GetStat(Enums.StatsEnum.ExperienceReward));
        }

        #region Bu iki method Saving icinden ISavaable'dan gelmistir
        //bu method ISaveable interface'dne gelir
        //public object CaptureState()
        //{
        //    return healthPoints;
        //}

        //bu method ISaveable interface'dne gelir
        //public void RestoreState(object state)
        //{
        //    healthPoints = (float)state;

        //    if (healthPoints <= 0)
        //    {
        //        TriggerDeathAndAnimation();
        //    }
        //    else
        //    {   
        //        _isDeath = false;
        //    }
        //} 
        #endregion
    }
}
