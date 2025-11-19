using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArrowsPuzzle
{
    public class UIGameOver : UIBasePanel
    {
        [SerializeField] private TextMeshProUGUI _textLevel;

        [SerializeField] private Button _buttonReplay;

        public event Action OnReplay;

        void Start()
        {
            int level = PlayerPrefs.GetInt(Values.GameDataKeys.Level, 1);
            _textLevel.text = $"Level {level}";

            _buttonReplay.onClick.AddListener(() =>
            {
                Replay();
            });
        }

        void Replay()
        {
            OnReplay?.Invoke();
        }
    }
}
