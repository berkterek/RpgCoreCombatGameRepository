using GameDevTV.Utils;
using RpgCoreCombatGame.Attributes;
using RpgCoreCombatGame.Combats;
using RpgCoreCombatGame.CoreGames;
using RpgCoreCombatGame.Movements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.Controllers
{
    //artificial intelligence 
    public class AiController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] float aggrevatCoolDownTime = 5f;
        [SerializeField] PatrolPathController partrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 3f;
        [Range(0, 1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        [SerializeField] float shoutDistance = 5f;

        GameObject _player;
        Health _health;
        Fighter _fighter;
        Mover _mover;

        LazyValue<Vector3> _guardPosition;
        float _timeSinceLastSawPlayer = Mathf.Infinity;
        float _timeSinceArrivedWaypoint = Mathf.Infinity;
        float _timeSinceAggrevated = Mathf.Infinity;
        int _currentWaypointIndex = 0;

        private void Awake()
        {
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _player = GameObject.FindWithTag("Player");
            _guardPosition = new LazyValue<Vector3>(StartPositionInitial);
        }

        private void Start()
        {
            _guardPosition.ForceInit();
        }

        private void Update()
        {
            if (_health.IsDeath()) return; //eger dusman olduyse controller iptal et

            if (IsAggrevated() && _fighter.CanAttack(_player))
            {
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer < suspicionTime)
            {
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0;
            _fighter.Attack(_player);

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);

            foreach (RaycastHit hit in hits)
            {
                AiController enemy = hit.collider.GetComponent<AiController>();

                if (enemy != null)
                {
                    enemy.Aggrevate();
                }
            }
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedWaypoint += Time.deltaTime;
            _timeSinceAggrevated += Time.deltaTime;
        }

        public void Aggrevate()
        {
            _timeSinceAggrevated = 0f;
        }

        private Vector3 StartPositionInitial()
        {
            return this.transform.position;
        }

        private void PatrolBehaviour()
        {
            //guardPosition ilk yerini temsil eder ve nextPositon'a atariz
            Vector3 nextPosition = _guardPosition.Value;

            if (partrolPath != null)
            {
                if (AtWaypoint())
                {
                    _timeSinceArrivedWaypoint = 0;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            if (_timeSinceArrivedWaypoint > waypointDwellTime)
            {
                _mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }

        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = partrolPath.GetNextIndex(_currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return partrolPath.GetWaypoint(_currentWaypointIndex);
        }

        private bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(_player.transform.position, this.transform.position);
            return distanceToPlayer < chaseDistance || _timeSinceAggrevated < aggrevatCoolDownTime;
        }

        //called by unity
        //bu method cagirildiginda ise butun nesneler icinde direk gosterrir nesneyi secmeme gerek kalmaz
        private void OnDrawGizmos()
        {
            OnDrawGizmosSelected();
        }

        //bu method bizim Gizmos cizmemizi saglar secili olan nesne icinde gosterir
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.transform.position, chaseDistance);
        }
    }
}
