using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//bu class'in amaci direk singleton yapmak yerine serializeField icindeki secili object'i yok edimemesini saglamak singleton gibi olur ama static kullanmayiz ve heryerden erisimini kapatmis oluruz
namespace RpgCoreCombatGame.CoreGames
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistenObjectPrefab;

        //default static field'imizi false yapariz yani hic olusturulmadiysa bu classs direk false olucaktir olustuktan sonra true'ya cekilicektir
        static bool _hasSpawner = false;

        private void Awake()
        {
            if (_hasSpawner) return; //eger true ise direk return ol

            SpawnPersistentObjects();

            _hasSpawner = true; //ilk nesne olsutuktan sonra static oldugundan dolayi yeni olusucak instance veya instantiate islemleri icin butun bu class'i kullanicak nesneler icin _hasSpawner true olucak cunku static field'imdir
        }

        //SerializeField icinde verilen nesneyi direk reference'i al ve olustur diyen class'i yarattik ve yok etme demmis olduk
        private void SpawnPersistentObjects()
        {
            if (persistenObjectPrefab != null)
            {
                GameObject persistenObject = Instantiate(persistenObjectPrefab);
                DontDestroyOnLoad(persistenObject);
            }
        }
    }
}
