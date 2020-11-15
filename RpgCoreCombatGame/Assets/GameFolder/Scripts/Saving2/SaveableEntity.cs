using RpgCoreCombatGame.CoreGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace RpgCoreCombatGame.Saving2
{
    [ExecuteAlways] //ExecuteAlways ise hem game sahnesi iicnde hemde scene icinde calisir iki durumda da calisir demek
    //[ExecuteInEditMode] //InEditMode ise sadece Scene icinde execute olur yani calisma zamaninda degil
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string uniqueIdentifier = "";
        static Dictionary<string, SaveableEntity> _globalLookUp = new Dictionary<string, SaveableEntity>();

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        //SavingSystem icinde CaptureState iicnde bu method kullanildi
        public object CaptureState()
        {
            //her cagirilan bu component'i paylasan nesne icin bu method SerializableVector3 donucek
            //Debug.Log("Capturing state for " + GetUniqueIdentifier());
            //return new SerializableVector3(transform.position);
            Dictionary<string, object> states = new Dictionary<string, object>();

            foreach (ISaveable item in GetComponents<ISaveable>())
            {
                //ISaveable'a bagli olan miras alan butun ISaveable object'lerini gez demis olduk ornegin Mover ISaveable icinden miras alir Mover bizim string key'imiz oldu ve item.CaptureState ise mover icindeki CaptureState'e dictionary icinde listelicek boylelikle butun mover health gibi butun yapilari tek seferde save etmis olucaz
                states[item.GetType().ToString()] = item.CaptureState();
            }

            return states;
        }

        //SavingSystem icinde RestoreState iicnde bu method kullanildi
        public void RestoreState(object state)
        {
            //burda ise bu component'i tasiyan butun nesneler icin SerializableVector3 doner ve postion.ToVector3 yapip normal vector3 ceviririz
            //Debug.Log("Restore state for " + GetUniqueIdentifier());
            //SerializableVector3 postion = state as SerializableVector3;

            //NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();

            //navMeshAgent.enabled = false;
            //transform.position = postion.ToVector3();
            //navMeshAgent.enabled = true;

            //GetComponent<ActionScheduler>().CancelCurrentAction();

            Dictionary<string, object> states = state as Dictionary<string, object>;

            if (state != null)
            {
                foreach (ISaveable item in GetComponents<ISaveable>())
                {
                    string typeString = item.GetType().ToString();

                    if (states.ContainsKey(typeString))
                    {
                        item.RestoreState(states[typeString]);
                    }
                }
            }
        }

        //update icindeki UnityEditor ref'înden gelen SerializeObject class'in editMode icinde calisir bir problem yaratmaz ama oyunu publish ettigimzde bize hata vericektir bunun icin sadece bu yapi iicinde calismasi iicn bir #if yazmamazi lazimdir
#if UNITY_EDITOR //sadece bu Update method Unity editor icinde calissin demis olduk
        private void Update()
        {
            //burda sadece sunu demis oluyoruz bu kisim derlendigi zaman bu yapi calismasin demis olduk
            if (Application.IsPlaying(gameObject))
            {
                return;
            }

            //eger bu component'i alan gameObject oyun sahnesi uzerinde path'i null veya empty ise return et yani bu asagidaki islemleri yapma demis olduk cunku prefab character veya player veya enemy dosya icinde ise sadece dosya icindeyse ve oyun sahnesi icinde degilse bu islemleri gec demis olduk
            if (string.IsNullOrEmpty(gameObject.scene.path))
            {
                return;
            }


            //bu yapi UnityEditor ref'den gelmekte ve bunun bir kisitlamasi vardir oda bu yapi derlenmez unity erismez ve bize bu class icin hata vericektir
            //bu nesneyi component'i tasiyan gameobject'leri bir serilize et demis olduk
            SerializedObject serializeObject = new SerializedObject(this);

            //bu nesnenin property'sini bul demis olduk
            //property degiskenine atadik
            SerializedProperty property = serializeObject.FindProperty("uniqueIdentifier");

            //ve dedik ki eger stringValue esitse "" bosluga
            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                //gitsin bunun Guid'ini string'e cevirip property'me atasin demis olduk
                property.stringValue = System.Guid.NewGuid().ToString();

                //bunun ustune bu islemi yaptiktan sonra serilize olan object'imi guncellemem lazim
                serializeObject.ApplyModifiedProperties();
            }

            _globalLookUp[property.stringValue] = this;
            //Debug.Log("Editing...");
        }
#endif

        //bu class icinde update bolumde IsNullOrEmpty icinde cagirdik
        private bool IsUnique(string candidate)
        {
            if (!_globalLookUp.ContainsKey(candidate)) //eger girilen unique id eger yoksa 
            {
                return true; //bize true deger dondururuz cunku yeni unique id versin diye
            }

            if (_globalLookUp[candidate] == this)
            {
                return true; //eger girilen unique id varsa ve simdiki bu class'in id'sine esit ise ozaman true don ki bizim bu class new id alsin diye
            }

            if (_globalLookUp[candidate] == null) //static hep yasadigi icin ve baska scene gittigimde de yasarlar ve ayni sahneye geri geldigimde butun karakterler benim id'im alinmis ozaman yeni id almam lazimder ve id'leri degisir bunun onunu gecmek icin bu if yapisini yazdik yani sahneden sahneye giderken dictinary icini temizlemis olutoruz yeni geldiginde tekrar ayni id'leri tekrar dictinary icinde ekler
            {
                _globalLookUp.Remove(candidate);
                return true;
            }

            if (_globalLookUp[candidate].GetUniqueIdentifier() != candidate)
            {
                _globalLookUp.Remove(candidate);
                return true;
            }

            return false; //bize false doner cunku zatan oldgunundan dolayi
        }
    }
}
