using RpgCoreCombatGame.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RpgCoreCombatGame.SceneManagements
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] int sceneToLoad = -1; //default deger alis oyun icinden portaldan alicaktir
        [SerializeField] Transform spawnPoint; //child elementi oldugu icin direk Inspector'dan atadigimizda problem olmicaktir
        [SerializeField] DestinationIdentifier destination; //bunu da inspector'dan her portal icin sectiricez ve birbirine baglicaz
        [SerializeField] float faderTime = 2f;

        //enum'lari destination'lari birbirinden ayirmak icin bu sekil yazdik
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        //Portal'dan gectgimizde hangi index numarasini verirsek o scene bizim karsimiza cikicaktir
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }

        //eger bir parent icinde ise yaa onu'da yok etme diyicez yada burdaki gibi bir method yazicaz do not destroy icine yazdimgiz gameobject'i once transform.parent = null'a cekeriz cunku dont destroy dedigmizde direk parent'i aricak bulamiyinca sacmalicak bundan dolayi parent'i null'a cekeriz sonra DontDestroy deriz
        private void DontDestroyOL()
        {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }

        private IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set on inspector");
                yield break;
            }

            DontDestroyOL();

            Fader fader = FindObjectOfType<Fader>();
            Saving2.SavingWrapper savingWrapper = FindObjectOfType<Saving2.SavingWrapper>();
            
            //playerController'i portaldan girip cikarken enable = false'a cekeriz cunku race condition olmasin diye yani yasi durumu ve code'lar birbirine girmesin diye
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;

            savingWrapper.Save(); //diger boyuta scene gecmeden once checkPoing gibi save et

            yield return fader.FadeOut(faderTime);

            //Save Current Level
            //Saving2.SavingWrapper savingWrapper = FindObjectOfType<Saving2.SavingWrapper>();

            //savingWrapper.Save();

            //DontDestroyOL();
            //DontDestroyOnLoad(this);
            //burda yaptigmiz islem sudur bu method yeni sahne hazir olana kadar bize donmicektir hazir olduktan sonra yeni sahne gelicektir ama onun disinda ki normal Coroutine mantgii devam eder sadece sonraki is hazir olunca yield return icinde bunu belirttik
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;
            //Load Current Level
            //savingWrapper.Load();
            savingWrapper.Load(); //seni bolume gectigimde Load yap

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            //savingWrapper.Save();
            savingWrapper.Save(); //ve yeni bolumde load'dan sonra ilk checkpoint'i save et demis olduk

            yield return fader.FadeIn(faderTime);

            newPlayerController.enabled = true;
            Destroy(this.gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");

            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.spawnPoint.position; //bu yapi ile NavMeshAgent ile ilgili hata alablriz cunku bizim yurume islemrizmi NavMeshAgent yapmakta bunun onune gecmeinin iki yolu var ilk position ve rotation vermeden once NavMeshAgent enable false position ve rotaion verdikten sonra NavMeshAgent enable true cekebilriz ve ikinci ve daha duzgun yol ise

            //ikinci yol NavMeshAgent icin
            //player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position); //verdigmiz alana teleport ol demis olduk

            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal item in FindObjectsOfType<Portal>())
            {
                if (item == this)
                {
                    continue;
                }
                else if (item.destination == destination)
                {
                    return item;
                }
            }

            Debug.Log("Null");
            return null;
        }
    }
}
