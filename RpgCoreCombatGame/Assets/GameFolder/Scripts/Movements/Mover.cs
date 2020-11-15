using RpgCoreCombatGame.CoreGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RpgCoreCombatGame.Saving;
using RpgCoreCombatGame.Attributes;

namespace RpgCoreCombatGame.Movements
{
    public class Mover : MonoBehaviour, IAction /*ISaveable*/, Saving2.ISaveable
    {
        [SerializeField] float maxSpeed = 6f;

        NavMeshAgent _navMeshAgent;
        Health _health;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();
        }

        void Update()
        {
            //if (Input.GetMouseButton(0))
            //{
            //    MoveToCursor();
            //}
            //origin kamera pozisyonu
            //direction ise mouse ile tiklanin yerin posizyonu * 100 yapmamizin nedeni ise origin den baslayip direction'a kadar bir cizgi cekmesini istedik * 100 ile uzunlugunu arttirmis olduk cizginin
            //Debug.DrawRay(_lastRay.origin, _lastRay.direction * 100);
            if (_health.IsDeath()) //eger olu ise navMeshAgent iptal ettik
            {
                _navMeshAgent.enabled = false;
            }
        }

        private void FixedUpdate()
        {
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            //Player gameobject'in dunya uzerindeki hizi
            Vector3 velocity = _navMeshAgent.velocity;

            //dunya uzerindeki Player'in degil child elementinin local hizi
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            //ve sadece z'yi aliriz cunku animation'in icinde artan azalan hiz z oldugundan
            float speed = localVelocity.z;
            //Debug.Log(string.Format("NavMeshAgent({0}) LocalVelocity({1}) Speed({2})",velocity,localVelocity,speed));
            //ve parametreye artan azalan hizi animasyona gondeririz
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        //private void MoveToCursor()
        //{
        //ray ışın anlamina gelir
        //RaycastHit ise ışınin carptigi anlamina gelir
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit raycastHit;

        //bool hasHit = Physics.Raycast(ray, out raycastHit);

        //if (hasHit)
        //{
        //    MoveTo(raycastHit.point);
        //}
        //}

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            _navMeshAgent.SetDestination(destination);
            _navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            _navMeshAgent.isStopped = false;
        }

        //IAction iicnden gelir hem ActionScheduler icinde hemde Fighter icinde cagirilir
        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }

        //bu method ISaveable'dan gelir ve bu methodu SaveableEntitiy icinde cagirdik
        public object CaptureState()
        {
            return new SerializableVector3(this.transform.position);
        }

        //bu method ISaveable'dan gelir ve bu methodu SaveableEntitiy icinde cagirdik
        public void RestoreState(object state)
        {
            SerializableVector3 serializableVector3 = state as SerializableVector3;

            if (serializableVector3 != null)
            {
                //if (_navMeshAgent != null)
                //{
                    _navMeshAgent.enabled = false;
                    this.transform.position = serializableVector3.ToVector();
                    _navMeshAgent.enabled = true;
                    GetComponent<ActionScheduler>().CancelCurrentAction();
                //}
            }
        }

        //bu struct tek amaci var oda iki tane SerializableVector3 class'ini icinde tutup position ve rotation bilgisi tutmak datayi save edebilmek icin
        //[System.Serializable]
        //struct MoverSaveData
        //{
        //    public SerializableVector3 position;
        //    public SerializableVector3 rotation;
        //}

        #region ISaveable icindeki Saving dosyasinda olan
        //ISaveable interface'den gelen method
        //butun saveable component'lerin yerini save ettik
        //public object CaptureState()
        //{
        //    //bu Dictionary sayesinde string ref vererek object data'lari atabiliriz ve bu sayede birden fazla yapiyi save etme sansimiz olur burdaki ornekte hem transform.position ve transform.rotation kayit etmis oluyoruz
        //    //1.yol dictionary ile
        //    //Dictionary<string, object> data = new Dictionary<string, object>();

        //    //data["position"] = new SerializableVector3(transform.position);
        //    //data["rotation"] = new SerializableVector3(transform.eulerAngles);

        //    //return data;

        //    //2.yol
        //    MoverSaveData moverSaveData = new MoverSaveData();

        //    //bizim mover gameobject'e bagli yapiinin transform.position ve rotation bilgilerini struck iicnde atabilmek icin bu tarz bir islem yaptik
        //    moverSaveData.position = new SerializableVector3(transform.position);
        //    moverSaveData.rotation = new SerializableVector3(transform.eulerAngles);

        //    return moverSaveData;
        //}

        //ISaveable interface'den gelen method 
        //public void RestoreState(object state)
        //{
        //    //as bize null doner
        //    //(SerializableVector3)object hatirlatma cast ise direk exception firlatir
        //    //SerializableVector3 postion = state as SerializableVector3;

        //    //1.yol dictionary ile
        //    //Dictionary<string, object> data = state as Dictionary<string, object>;

        //    //2.yol struct ile
        //    MoverSaveData data = (MoverSaveData)state;

        //    //if (data != null)
        //    //{
        //    NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        //    navMeshAgent.enabled = false;

        //    //transform.position = ((SerializableVector3)data["postion"]).ToVector();
        //    //transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
        //    //eulerAngles ile Vector3 ile vermemizi saglar transform.rotation ise bize Quaternion ister

        //    transform.position = data.position.ToVector();
        //    transform.eulerAngles = data.rotation.ToVector();


        //    navMeshAgent.enabled = true;
        //    //}
        //} 
        #endregion


    }
}
