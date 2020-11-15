using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RpgCoreCombatGame.Movements;
using RpgCoreCombatGame.Enums;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using RpgCoreCombatGame.Attributes;

//namespace vermemzin nedeni baska klasorlerden ayirmak icin ve ulasilabilmesi yada kullanilabilmesi icin baska class'lar tarafindan once dosya yolunun verilmesi lazimdir
namespace RpgCoreCombatGame.Controllers
{
    //playercontroller icinde biz oyuncunun ana karaterin butun controller'lairnin burda tutariz fighter veya mover gibi diger component'lari ilgili isleri olustururur controller icinde cagiririz ana merkez bu ksiimdir
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float maxNavMeshDistance = 1f;
        [SerializeField] float maxNavPathLength = 40f;
        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float radiusDistance = 1f;

        Health _health;

        void Start()
        {
            _health = GetComponent<Health>();
        }

        void FixedUpdate()
        {
            if (InteractWithUI()) return;

            if (_health.IsDeath())
            {
                SetCursor(CursorTypeEnum.None);
                return; //eger player olduyse controller iptal et
            }

            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;

            SetCursor(CursorTypeEnum.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                if (raycastables != null)
                {
                    foreach (IRaycastable raycastable in raycastables)
                    {
                        if (raycastable.HandleRaycast(this))
                        {
                            SetCursor(raycastable.CursorType);
                            return true;
                        }
                    }
                }

            }

            return false;
        }

        //ray'in en once carpani alimis olduk isin ilk kime carpiyorsa bize onun bilgisini donmesini saglayan method
        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), radiusDistance);
            float[] distances = new float[hits.Length];

            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            System.Array.Sort(distances, hits);

            return hits;
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorTypeEnum.UserInterface);
                return true;
            }

            return false;

            //IsPointerOverGameObject method'u burda game object dedigi aslinda UI'dir
            //bool isUI = EventSystem.current.IsPointerOverGameObject();
            //Debug.Log(isUI);
            //return isUI;
        }

        bool InteractWithMovement()
        {
            Vector3 target;
            bool isHit = RaycastNavMesh(out target);

            if (isHit)
            {
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                }

                SetCursor(CursorTypeEnum.Movement);
                return true;
            }

            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            bool isHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!isHit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point,out navMeshHit,maxNavMeshDistance,NavMesh.AllAreas);

            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;

            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavPathLength) return false;

            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0f;
            if (path.corners.Length > 2) return total;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }

        private void SetCursor(CursorTypeEnum cursorType)
        {
            CursorMapping mapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private CursorMapping GetCursorMapping(CursorTypeEnum cursorType)
        {
            foreach (CursorMapping item in cursorMappings)
            {
                if (item.cursorType == cursorType) return item;
            }

            return cursorMappings[0];
        }

        [System.Serializable]
        struct CursorMapping
        {
            public CursorTypeEnum cursorType;
            public Texture2D texture;
            public Vector2 hotspot;
        }
    }
}
