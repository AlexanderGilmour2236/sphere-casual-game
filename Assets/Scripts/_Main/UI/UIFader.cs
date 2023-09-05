using System;
using DG.Tweening;
using UnityEngine;

namespace sphereGame.UI
{
    public class UIFader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _faderCanvasGroup;
        [SerializeField] private float _fadeInDuration;
        [SerializeField] private float _gameOverDuration;

        private Tween _faderTween;

        public Tween fadeIn(Action onComplete)
        {
            killFadeTween();
            _faderTween = _faderCanvasGroup.DOFade(1, _fadeInDuration).OnComplete(onComplete.Invoke);
            return _faderTween;
        }

        public Tween fadeOut(Action onComplete)
        {
            killFadeTween();
            _faderTween = _faderCanvasGroup.DOFade(0, _fadeInDuration).OnComplete(onComplete.Invoke);
            return _faderTween;
        }

        private void killFadeTween()
        {
            if (_faderTween != null)
            {
                _faderTween.Kill();
                _faderTween = null;
            }
        }

        public float gameOverDuration
        {
            get { return _gameOverDuration; }
        }
    }
}