using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.CoreGames
{
    public class ActionScheduler : MonoBehaviour
    {
        //currentAction simdiki ve en sonuncu harektetmiz action'inmizdir biz bu islemi iptal edip yeni action'ini inject ederiz
        IAction _currentAction;

        public void StartAction(IAction action)
        {
            if (_currentAction == action) //_current action ve disardan gelen action ayni ise hic bir islem yapma demis olduk
            {
                return;
            }

            if (_currentAction != null) //eger current action bos degilse current action'i iptal et demis olduk
            {
                _currentAction.Cancel(); //IAction icinde fighter yada Mover gelicektir burda iki arasinda bir kopru kurduk ve Cancel method'nu burda cagiriyoruz
                //Debug.Log("Cancel current actin => " + _currentAction.GetType().ToString());
            }

            _currentAction = action;
            //Debug.Log("Add new action => " + action.ToString());
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}
