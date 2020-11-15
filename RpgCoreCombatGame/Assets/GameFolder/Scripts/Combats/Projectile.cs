using RpgCoreCombatGame.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RpgCoreCombatGame.Combats
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float arrowSpeed = 3f;
        [SerializeField] bool isHoming = true; //isHoming true ise hedefi takip edicektir degilse duz gidicektir
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2f;
        [SerializeField] UnityEvent onHit;

        Health _target = null;
        GameObject _instigator = null;
        float _damage = 0;

        private void Start()
        {
            transform.LookAt(GetAimLocation()); // bu ise hedefe bakmamiza yariyor update icinde olursa direk bu hedefi takip edicektir
        }

        void Update()
        {
            if (_target == null) return;

            //burdaki mantik yukarida yazdigmiz gibidir yani true ise hedefi takip eder degilse duz gider
            if (isHoming && !_target.IsDeath()) transform.LookAt(GetAimLocation());
            

            transform.Translate(Vector3.forward * arrowSpeed * Time.deltaTime); //bu yapi okun duz gitmesine yariyor
        }

        //weapon component'i iicnde LaunchProjectile method'unda cagirilir disaridan target ve damage bilgisini alir
        public void SetTarget(Health target, float damage,GameObject instigator)
        {
            _target = target;
            _damage = damage;
            _instigator = instigator;

            Destroy(this.gameObject, maxLifeTime);
        }

        //eger hedefinizim merkezi egitimdeki gibi ayaklarindaysa hedef yani ok ayaklara gidicektir bunu code ile onlemenin yolu ise asagidaki gibi hedefin capsulecollider'i height'i arttirmaktir
        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = _target.GetComponent<CapsuleCollider>();

            if (targetCapsule == null)
            {
                return _target.transform.position;
            }

            return _target.transform.position + Vector3.up * targetCapsule.height / 4;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != _target) return;
            if (_target.IsDeath()) return;

            _target.TakeDamage(_instigator,_damage);
            onHit.Invoke();
            arrowSpeed = 0;

            if (hitEffect != null)
            {
                GameObject newEffect = Instantiate(hitEffect,GetAimLocation(),transform.rotation);
            }

            foreach (GameObject item in destroyOnHit)
            {
                Destroy(item);
            }

            Destroy(this.gameObject, lifeAfterImpact);
        }
    }
}

