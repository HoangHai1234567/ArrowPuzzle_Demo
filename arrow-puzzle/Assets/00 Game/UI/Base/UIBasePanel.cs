using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ArrowsPuzzle
{
    public class UIBasePanel : MonoBehaviour
    {
        [SerializeField] protected GameObject container;
        [SerializeField] protected Image imageBackground;

        #region Events

        public event Action OnShow;

        public event Action OnHide;

        #endregion

        public virtual void Show(bool withAnim = true, Action callback = null)
        {
            gameObject.SetActive(true);
            imageBackground.gameObject.SetActive(true);
            OnShowEvent();
            if (withAnim)
            {
                container.gameObject.SetActive(true);
                ShowAnim(callback);
            }
            else
            {
                container.SetActive(true);
                callback?.Invoke();
            }
        }

        public virtual void Hide(bool withAnim, Action callback)
        {
            OnHideEvent();
            if (withAnim)
            {
                HideAnim(() =>
                {
                    imageBackground.gameObject.SetActive(false);
                    gameObject.SetActive(false);
                    callback?.Invoke();
                });
            }
            else
            {
                imageBackground.gameObject.SetActive(false);
                gameObject.SetActive(false);
                container.SetActive(false);
                callback?.Invoke();
            }
        }

        protected virtual void ShowAnim(Action callback)
        {
            container.transform.localScale = Vector3.zero;
            container.transform.DOScale(Vector3.one, 0.3f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    callback?.Invoke();
                });
        }

        protected virtual void HideAnim(Action callback)
        {
            container.transform.localScale = Vector3.one;
            container.transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    callback?.Invoke();
                });
        }

        protected void OnShowEvent()
        {
            OnShow?.Invoke();
        }

        protected void OnHideEvent()
        {
            OnHide?.Invoke();
        }
    }
}
