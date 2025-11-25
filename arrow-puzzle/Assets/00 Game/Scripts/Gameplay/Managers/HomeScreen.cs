using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArrowsPuzzle
{
    public class HomeScreen : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _mainButtonsGroup;   // chứa Play + Quit
        [SerializeField] private GameObject _levelSelectPanel;   // panel chọn level

        private void Start()
        {
            if (_levelSelectPanel != null)
                _levelSelectPanel.SetActive(false);

            if (_mainButtonsGroup != null)
                _mainButtonsGroup.SetActive(true);

            // Đảm bảo có level mặc định
            if (!PlayerPrefs.HasKey(Values.GameDataKeys.Level))
            {
                PlayerPrefs.SetInt(Values.GameDataKeys.Level, 1);
                PlayerPrefs.Save();
            }
        }

        // ====== NÚT Ở MÀN HÌNH CHÍNH ======
        public void OnClickPlay()
        {
            // Ấn Play thì ẩn Play/Quit, hiện panel chọn level
            if (_mainButtonsGroup != null)
                _mainButtonsGroup.SetActive(false);

            if (_levelSelectPanel != null)
                _levelSelectPanel.SetActive(true);
        }

        public void OnClickQuit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        // ====== NÚT Ở PANEL CHỌN LEVEL ======
        public void OnClickBackFromLevelSelect()
        {
            if (_levelSelectPanel != null)
                _levelSelectPanel.SetActive(false);

            if (_mainButtonsGroup != null)
                _mainButtonsGroup.SetActive(true);
        }

        public void OnClickSelectLevel(int levelIndex)
        {
            // Lưu level người chơi chọn rồi sang scene Game
            PlayerPrefs.SetInt(Values.GameDataKeys.Level, levelIndex);
            PlayerPrefs.Save();

            // Tạm thời load thẳng sang Game, chưa cần hiệu ứng mây
            SceneManager.LoadScene(Values.Game.GameSceneName);
        }
    }
}
