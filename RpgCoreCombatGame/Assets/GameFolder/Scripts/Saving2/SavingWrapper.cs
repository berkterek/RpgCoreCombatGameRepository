using RpgCoreCombatGame.SceneManagements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Saving2 kendimizin yazdigi Saving ise direk udemy icinden aldigmiz
namespace RpgCoreCombatGame.Saving2
{
    //SavingWrapper sarici kaydetme gibi dusunelim
    //SavingWrapper ise oyun icinde bir S'e basarsa bu SavingSystem icinde olan Save veya Load methodu tetiklicek 
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] float faderTime = 1f;

        const string DEFAULT_SAVE_FILE = "save";

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }

        //SavingSystem.LoadLastScene Coroutine oldundan direk cagiramayiz yazilir hata vermez ama debug oldugunda calismadigini goruruz coroutine'ler IEnumerator icinde cagirilir yield return ile yazlir veya StartCoroutine() method'u ile yazmamaiz lazimdir
        private IEnumerator LoadLastScene()
        {
            //son kaldigimiz scene iicnde load olamasini istiyoruz
            yield return GetComponent<SavingSystem>().LoadLastScene(DEFAULT_SAVE_FILE);

            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(faderTime);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Delete();
            }
        }

        private void Delete()
        {
            GetComponent<SavingSystem>().Delete(DEFAULT_SAVE_FILE);
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(DEFAULT_SAVE_FILE);
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(DEFAULT_SAVE_FILE);
        }
    }
}
