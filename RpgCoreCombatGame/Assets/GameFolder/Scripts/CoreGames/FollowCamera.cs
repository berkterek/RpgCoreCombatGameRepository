using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RpgCoreCombatGame.Controllers;

namespace RpgCoreCombatGame.CoreGames
{
    //bu class bu Mover Component icinde gecmiyorsa calismicak anlamina gelmektedir
    [RequireComponent(typeof(PlayerController))]
    public class FollowCamera : MonoBehaviour
    {
        Transform _target;

        private void Awake()
        {
            _target = FindObjectOfType<PlayerController>().transform;
        }

        //bu kismi LateUpdate yapmamizin nedeni once oyuncuunun haraket etmesini en son kameranin takip etmesini istedigimizden bu kismi LateUpdate olarak degistirdik
        void LateUpdate()
        {
            //bu class'a bagli GameObject'i child elemniti GameCamere yaptik ve bu GameObject'in transform.position'ini y 1 ve kalani Mover'a bagli oldugu GameObject'in transform'unun ustune atadik boylelikle GameCamera parent ile haraket eder
            this.transform.position = new Vector3(_target.position.x, 1, _target.position.z);
        }
    }
}
