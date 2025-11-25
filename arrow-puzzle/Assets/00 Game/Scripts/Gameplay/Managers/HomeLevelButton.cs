using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArrowsPuzzle
{
    public class HomeLevelButton : MonoBehaviour
    {
        [SerializeField] private int _levelIndex = 1;   // set trong Inspector

        // Gắn hàm này vào OnClick của Button
        public void OnClickSelect()
        {
            // Lưu level người chơi chọn
            PlayerPrefs.SetInt(Values.GameDataKeys.Level, _levelIndex);
            PlayerPrefs.Save();

            if (SceneTransition.Instance != null)
            {
                SceneTransition.Instance.LoadSceneWithTransition(Values.Game.GameSceneName);
            }
            else
            {
                // Nếu không có SceneTransition thì load thẳng
                SceneManager.LoadScene(Values.Game.GameSceneName);
            }
        }
    }
}
