using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EssentialManagers.Scripts
{
    public class GameStarter : MonoBehaviour, IPointerDownHandler
    {
        public bool autoStart;
        private bool _isGameStarted;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.1f);
            if (!autoStart) yield break;
            _isGameStarted = true;
            GameManager.instance.StartGame();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isGameStarted) return;
            _isGameStarted = true;
            GameManager.instance.StartGame();
        }
    }
}