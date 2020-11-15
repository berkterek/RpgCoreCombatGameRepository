using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgCoreCombatGame.SceneManagements
{
    public class Fader : MonoBehaviour
    {
        //[SerializeField] float fadeSpeed = 1f;
        CanvasGroup _canvasGroup;
        private bool _isFadeIn = false;

        //bu method'un icindeki yapiyi start'dan awake cekmemizin nedeni SaveWrapper start'dan once calismasi icindir awake method'u start'dan once calisir
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        //bu method bizim ekraninmizi bir anda beyazlaticaktir ve bu method'u biz SavingWrapper icinde cagirdik start method'unda
        public void FadeOutImmediate()
        {
            _canvasGroup.alpha = 1f;
        }

        //Portal icinde Transaction method'u tetiklendiginde cagirilir
        //bu method cagirildiginda bizim fader component'imiz FaderCanvasa baglidir ve fade out dedigmiz ise direk image'i anlik gostermek beyaz ekrani getirir
        public IEnumerator FadeOut(float time)
        {
            _isFadeIn = true;
            while (_canvasGroup.alpha < 1)
            {
                _canvasGroup.alpha += Time.deltaTime / time;
                yield return null;
            }
            _isFadeIn = false;
        }

        //Portal icinde Transaction method'u tetiklendiginde cagirilir
        //bu ise fade out ile gelen beyaz ekrani tekrar kapariz 0'a cekeriz
        public IEnumerator FadeIn(float time)
        {
            while (_canvasGroup.alpha >  0.05f && !_isFadeIn)
            {
                _canvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
        }
    }
}
