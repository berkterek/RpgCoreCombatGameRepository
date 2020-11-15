using RpgCoreCombatGame.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
//Playables kutuphanesi sayesinde bu timeline icin kullandiigz PlayableDirector gibi class'lara component'lara erismemizi saglar

namespace RpgCoreCombatGame.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool _isPassOneTime = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && !_isPassOneTime)
            {
                GetComponent<PlayableDirector>().Play();
                _isPassOneTime = true;
            }
        }
    }
}