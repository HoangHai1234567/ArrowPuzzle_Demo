using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArrowsPuzzle
{
    public class UILevelComplete : UIBasePanel
    {
        [SerializeField] private TextMeshProUGUI _textLevel;

        [SerializeField] private Button _buttonReplay;

        [SerializeField] private Button _buttonNext;

        public event Action OnReplay;

        public event Action OnNextLevel;

        void Start()
        {
            int level = PlayerPrefs.GetInt(Values.GameDataKeys.Level, 1);
            _textLevel.text = $"Level {level}";

            _buttonNext.onClick.AddListener(() =>
            {
                NextLevel();
            });

            _buttonReplay.onClick.AddListener(() =>
            {
                Replay();
            });
        }

        void NextLevel()
        {
            OnNextLevel?.Invoke();
        }

        void Replay()
        {
            OnReplay?.Invoke();
        }
    }
}
