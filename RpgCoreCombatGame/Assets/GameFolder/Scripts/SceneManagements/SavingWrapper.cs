using RpgCoreCombatGame.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.SceneManagements
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] float fadeInTime = 0.3f;
        [SerializeField] float fadeWaitTime = 0.5f;
        const string _defaultSaveFile = "save";

        //start method'u IEnumerator alabilir yani Coroutine olabilme ozelligine sahiptir
        //bu method calisir calismaz oyunu baslatigiizda direk last load'u vericektir bize
        private IEnumerator Start()
        {
            //once ekrani beyazlattik bir an once
            Fader fader = FindObjectOfType<Fader>();

            fader.FadeOutImmediate();

            //bu method IEnumerator oldugndan yield ile cagirdik
            yield return GetComponent<SavingSystem>().LoadLastScene(_defaultSaveFile);
            yield return new WaitForSeconds(fadeWaitTime); //kendini toparlamasi icin zaman verdik

            //sonra ekranin beyazligini indirdik
            yield return fader.FadeIn(fadeInTime);   
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
        }

        public void Load()
        {
            //call to saving system
            GetComponent<SavingSystem>().Load(_defaultSaveFile);
        }

        public void Save()
        {
            //call to saving system
            GetComponent<SavingSystem>().Save(_defaultSaveFile);
        }
    }
}
