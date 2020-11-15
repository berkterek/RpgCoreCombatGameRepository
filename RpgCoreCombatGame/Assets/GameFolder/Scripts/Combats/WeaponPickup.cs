using RpgCoreCombatGame.Attributes;
using RpgCoreCombatGame.Controllers;
using RpgCoreCombatGame.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.Combats
{
    /// <summary>
    /// RootPickup gameobject component'idir
    /// </summary>
    public class WeaponPickup : MonoBehaviour,IRaycastable
    {
        [SerializeField] WeaponConfig weapon;
        [SerializeField] float healtToRestore = 0f;
        [SerializeField] float respawnTime = 5f;

        public CursorTypeEnum CursorType => CursorTypeEnum.WeaponPickup;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject subject)
        {
            if (weapon != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            }

            if (healtToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healtToRestore);
            }

            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowHidePickup(false);
            yield return new WaitForSeconds(seconds);
            ShowHidePickup(true);
        }

        void ShowHidePickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool HandleRaycast(PlayerController player)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(player.gameObject);
            }

            return true;
        }
    }
}

