using RpgCoreCombatGame.Attributes;
using RpgCoreCombatGame.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.Combats
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController weaponOverride = null;
        [SerializeField] Weapon weaponPrefab = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 10f;
        [SerializeField] float percentageBonus = 0f;
        [SerializeField] HandEnum handEnum;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {

            DestroyOldWeapon(rightHand,leftHand);
            Weapon weapon = null;

            if (weaponPrefab != null)
            {
                Transform handTransform = GetHandTransform(rightHand, leftHand);
                weapon = Instantiate(weaponPrefab, handTransform);
                weapon.gameObject.name = weaponName; //elimizdeki silahin adini degistirdik cunku silah degistirme(yok etme destroy) yapmak istedigimzide bu isimden yakalariz
            }

            //AnimatorOverrideController RunTimeAnimatorController'dan miras alir
            //AnimatorOverrideController animatorOverride = weaponOverride.runtimeAnimatorController as AnimatorOverrideController;

            if (weaponOverride != null)
            {
                animator.runtimeAnimatorController = weaponOverride;
            }
            //else if (animatorOverride != null)
            //{
            //    animator.runtimeAnimatorController = animatorOverride.runtimeAnimatorController;
            //}

            return weapon;
        }

        //eski silagimizi yok etmemize yariyan method'dur sag el ve sol elin transfom bilgisini aliriz
        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName); //Find method'u child elementinin adindan yakalamaiza yariyan method'dur ve eger sag el dolu ise yani silah varsa onu yok edicek method asasagidaki destroy method'dur yoksa ayni islemi find ile left hand'e yapariz

            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }

            if (oldWeapon == null) return;

            oldWeapon.name = "Destroying"; //yeni silahta karisiklik olmasin diye silayin yok etmedne once adini degistirirz

            Destroy(oldWeapon.gameObject);
        }

        private Transform GetHandTransform(Transform rightHand, Transform leftHand)
        {
            if (handEnum == HandEnum.RightHand)
            {
                return rightHand;
            }
            else
            {
                return leftHand;
            }
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        //bu method sayesinde bizim hangi elden cikicagini bilir ve target olarakta health alir ve projectile.Set direk set ederiz
        public void LaunchProjectile(Transform rigthHand, Transform leftHand, Health health, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetHandTransform(rigthHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(health, calculatedDamage, instigator);
        }

        public float GetWeaponDamage()
        {
            return weaponDamage;
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }
    }
}
