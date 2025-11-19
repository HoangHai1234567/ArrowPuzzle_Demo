using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ArrowsPuzzle
{
    public class UIManager : MonoBehaviour
    {
        #region Assets

        [SerializeField] private AssetReference _uiLevelCompleteAsset;

        [SerializeField] private AssetReference _uiGameOverAsset;

        #endregion

        #region UI

        private UILevelComplete _uiLevelComplete = null;

        private UIGameOver _uiGameOver = null;


        #endregion

        #region Unity messages
        private void Start()
        {
            EventManager.UIEvents.OnRequestShowUILevelCompleted += UIEvents_OnRequestShowUILevelCompleted;
            EventManager.UIEvents.OnRequestShowUIGameOver += UIEvents_OnRequestShowUIGameOver;
        }

        #endregion

        #region Event handlers

        private void UIEvents_OnRequestShowUIGameOver(float delay, bool withAnim)
        {
            if (_uiGameOver != null)
            {
                StartCoroutine(_Show(delay));
            }
            else
            {
                CreatePanel(_uiGameOverAsset, (panel) =>
                {
                    _uiGameOver = panel.GetComponent<UIGameOver>();
                    _uiGameOver.OnReplay += EventManager.GamePlay.OnGameReplayEvent;
                    _uiGameOver.gameObject.SetActive(false);
                    StartCoroutine(_Show(delay));
                });
            }

            IEnumerator _Show(float delay)
            {
                yield return new WaitForSeconds(delay);
                _uiGameOver.Show();
            }
        }

        private void UIEvents_OnRequestShowUILevelCompleted(float delay, bool withAnim)
        {
            if (_uiLevelComplete != null)
            {
                StartCoroutine(_Show(delay));
            }
            else
            {
                CreatePanel(_uiLevelCompleteAsset, (panel) =>
                {
                    _uiLevelComplete = panel.GetComponent<UILevelComplete>();
                    _uiLevelComplete.OnNextLevel += EventManager.GamePlay.OnNextLevelEvent;
                    _uiLevelComplete.OnReplay += EventManager.GamePlay.OnGameReplayEvent;
                    _uiLevelComplete.gameObject.SetActive(false);
                    StartCoroutine(_Show(delay));
                });
            }

            IEnumerator _Show(float delay)
            {
                yield return new WaitForSeconds(delay);
                _uiLevelComplete.Show();
            }
        }
        #endregion

        void CreatePanel(AssetReference asset, Action<GameObject> callback)
        {
            var handle = Addressables.InstantiateAsync(asset);
            handle.Completed += operationHandle =>
            {
                if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    handle.Result.transform.SetParent(transform);
                    callback?.Invoke(handle.Result);
                }
            };
        }
    }
}
