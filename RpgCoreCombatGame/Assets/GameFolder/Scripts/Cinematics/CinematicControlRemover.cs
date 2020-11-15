using RpgCoreCombatGame.Controllers;
using RpgCoreCombatGame.CoreGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RpgCoreCombatGame.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        GameObject _player;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
        }

        void OnEnable()
        {
            PlayableDirector playableDirector = GetComponent<PlayableDirector>();
            //bu iki method'u PlayerDirector class'inin event'lerine atadik
            playableDirector.played += DisableControl;
            playableDirector.stopped += EnableControl;
        }

        private void OnDisable()
        {
            PlayableDirector playableDirector = GetComponent<PlayableDirector>();
            //bu iki method'u PlayerDirector class'inin event'lerine atadik
            playableDirector.played -= DisableControl;
            playableDirector.stopped -= EnableControl;
        }

        void DisableControl(PlayableDirector playableDirector)
        {
            _player.GetComponent<ActionScheduler>().CancelCurrentAction(); //eger bir attack yada baska bir islem yapiyorsak yuremek gibi onu anlik durdururuz
            _player.GetComponent<PlayerController>().enabled = false; //ve baska player ile bir islem olmamasi icin bir sureligine enable'ini false cekeriz
        }

        void EnableControl(PlayableDirector playableDirector)
        {
            _player.GetComponent<PlayerController>().enabled = true; //animasyon bittiginde tekrar PlayerController Component'ini enable true cekeriz
        }
    }
}

