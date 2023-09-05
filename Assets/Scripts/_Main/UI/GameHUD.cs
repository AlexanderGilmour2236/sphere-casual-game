using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace sphereGame.UI
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] private UIFader _gameOverFader;
        [SerializeField] private TextMeshProUGUI _gameOverText;
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
                .AppendInterval(_gameOverFader.gameOverDuration)
                .Append(_gameOverFader.fadeOut(()=> onSequenceComplete?.Invoke()));
        }

        public void setGameOverText(string gameOverText)
        {
            _gameOverText.text = gameOverText;
        }
    }

}
