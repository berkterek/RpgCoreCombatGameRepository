using RpgCoreCombatGame.CoreGames;
using RpgCoreCombatGame.Movements;
using RpgCoreCombatGame.Saving2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RpgCoreCombatGame.Stats;
using RpgCoreCombatGame.Enums;
using GameDevTV.Utils;
using System;
using RpgCoreCombatGame.Attributes;

namespace RpgCoreCombatGame.Combats
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {

        [SerializeField] float timeBetweenAttack = 1f;
        [SerializeField] Transform rightHandTransform = null; //left right hand transform yapmamizin nedeni animasyonlarin bazilari sag el bazilari sol el ile yapilmis
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null; //bu bizim inspector'den gelen varsayilan silahimiz
        //[SerializeField] string defaultWeaponName = "Unarmed"; //eger Resources dosyamizida dosya ve scriptable objectlerimize ulasmak istersek ogrenin Attack/Unarmed yazmamiz yeticektir dosya yolunu veriyoruz ve Resources dosyasi birden fazla olablir bundan dolayi scriptable objectleirin isimleri unique olmali

        Health _target;
        float _timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig _currentWeaponConfig = null; // bu ise suanki silahimiz equipWeapon iicnde bu degiskene silah atiyoruz
        LazyValue<Weapon> _currentWeapon;

        public Health Target => _target;

        private void Awake()
        {
            _currentWeaponConfig = defaultWeapon;
            _currentWeapon = new LazyValue<Weapon>(InitialWeapon);
        }

        private void Start()
        {
            _currentWeapon.ForceInit();
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;

            if (_target == null) return;
            if (_target.IsDeath()) return;

            if (Vector3.Distance(this.transform.position, _target.transform.position) <= _currentWeaponConfig.GetWeaponRange())
            {
                GetComponent<Mover>().Cancel();
                AttackAnimation();
            }
            else
            {
                GetComponent<Mover>().MoveTo(_target.transform.position, 1f);
                //AttackAnimation(false);
            }
        }

        private Weapon InitialWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private void AttackAnimation()
        {
            if (_timeSinceLastAttack > timeBetweenAttack)
            {
                //attack oncesi stopAttack animator paratresini resetleri
                BeforeAttackAndStopAttack("attack", "stopAttack");

                transform.LookAt(_target.transform); //dusmana bakmasi icin kullandigmiz method
                _timeSinceLastAttack = 0;
            }
        }

        private void BeforeAttackAndStopAttack(string setTrigger, string resetTrigger)
        {
            Animator attackAnimator = GetComponent<Animator>();
            attackAnimator.ResetTrigger(resetTrigger); //bu kismi bug'un onune gecmek icin yaptik attack yapmadan once stopAttack trigger'ini resetlesin dedik
            attackAnimator.SetTrigger(setTrigger);
        }

        //Attack method'u bizim disaridan dusman hedefin ref'in alan method'umuz ve Attack method'u PlayerController icinde cagirilir
        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            _target = combatTarget.GetComponent<Health>();
        }

        //PlayerController icinde InteractWithCombat() method'unda cagirdik
        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget != null)
            {
                Health healthTest = combatTarget.GetComponent<Health>();
                return healthTest != null && !healthTest.IsDeath();
            }

            return false;
        }

        //IAction'dan gelir ActionScheduler class'i icinde cagirilir
        public void Cancel()
        {
            //stopAttack oncesi attack animator paratresini resetleriz
            BeforeAttackAndStopAttack("stopAttack", "attack");
            _target = null;
            GetComponent<Mover>().Cancel();
        }

        //Animation Hit Event
        void Hit()
        {
            if (_target == null) return;

            float damage = GetComponent<BaseStat>().GetStat(Enums.StatsEnum.Damage);

            if (_currentWeapon.Value != null)
            {
                _currentWeapon.Value.OnHit();
            }

            Debug.Log("Damage:" + damage + " owns: " + transform.name);
            //weapon iicnde yazdigmiz bir method eger projectile varsa bize true doner yoksa false doner
            if (_currentWeaponConfig.HasProjectile())
            {
                _currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, _target, gameObject, damage);
            }
            else
            {
                _target.TakeDamage(gameObject, damage);
            }
        }

        //Animation Shoot Event
        //ikiside ayni isi yaptigindan ayni codelari tekrar tekrar yazmamak icin Shoot icinde hit method'unu cagirdik
        void Shoot()
        {
            Hit();
        }

        //bu method oyun icinde silah gordumuzde o silahi almamiza yariyan method'dur
        public void EquipWeapon(WeaponConfig weapon)
        {
            _currentWeaponConfig = weapon;
            //weaponOverride bizim AnimatorAnimatorController'imizdir ve kilic icin olusuturdumuz bir aniOverrideController'dir bu su isi yapmakta normal animasyonu bos elle yumruk atam ani gibi onu controller icinde ezip kilicla vurma anisine cevirme isi yapar
            _currentWeapon.Value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            //weapon scriptableObject icinden gelen method
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public object CaptureState()
        {
            return _currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifier(StatsEnum stat)
        {
            if (stat == StatsEnum.Damage)
            {
                yield return _currentWeaponConfig.GetWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifier(StatsEnum stat)
        {
            if (stat == StatsEnum.Damage)
            {
                yield return _currentWeaponConfig.GetPercentageBonus();
            }
        }
    }
}