using System;
using DG.Tweening;
using UnityEngine;

namespace sphereGame.UI
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] private UIFader _gameOverFader;
        private Sequence _fadeInSequence;

        public void playGameOverSequence(Action onFadeInComplete, Action onSequenceComplete)
        {
            if (_fadeInSequence != null)
            {
                _fadeInSequence.Kill();
                _fadeInSequence = null;
            }
            _fadeInSequence = DOTween.Sequence();
            _fadeInSequence
                .Append(_gameOverFader.fadeIn(() => onFadeInComplete?.Invoke()))
                .Append(_gameOverFader.fadeOut(()=> onSequenceComplete?.Invoke()));
        }
    }

}
