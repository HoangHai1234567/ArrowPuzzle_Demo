using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace ArrowsPuzzle
{
    public class LevelManager : MonoBehaviour
    {
        #region Serialize properties

        [SerializeField] private LevelCollection _levelCollection;

        #endregion


        public void GetCurrentLevel(Action<Level> callback)
        {
#if UNITY_EDITOR
            int test = PlayerPrefs.GetInt(Values.GameDataKeys.Test, 0);
            
            if (test != 0)
            {
                PlayerPrefs.DeleteKey(Values.GameDataKeys.Test);
                var text = PlayerPrefs.GetString(Values.GameDataKeys.TestLevel, String.Empty);
                if (!string.IsNullOrEmpty(text))
                {
                    Level level = new Level("Test","Test Level");
                    level.LoadFromText(text);


                    callback?.Invoke(level);
                }
                else
                {
                    var levelAsset = _levelCollection.GetLevel(0);
                    LoadLevelAsset(levelAsset,callback);
                }
            }
            else
            {
                int level = PlayerPrefs.GetInt(Values.GameDataKeys.Level, 1);
                int levelIndex = (level - 1) % _levelCollection.Count + 1;

                var levelAsset = _levelCollection.GetLevel(levelIndex);
                LoadLevelAsset(levelAsset, callback);
            }
#else
            int level = PlayerPrefs.GetInt(Values.GameDataKeys.Level, 1);
            int levelIndex = (level-1) % _levelCollection.Count+1;

            var levelAsset = _levelCollection.GetLevel(levelIndex);
            LoadLevelAsset(levelAsset, callback);
#endif
        }

        private void LoadLevelAsset(LevelAssetData levelAssetData,Action<Level> callback)
        {
            var opt = Addressables.LoadAssetAsync<TextAsset>(levelAssetData.LevelAsset);
            if (opt.IsValid())
            {
                opt.Completed += handle =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Level level = new Level(levelAssetData.Name, levelAssetData.Description);
                        level.LoadFromText(handle.Result.text);

                        callback?.Invoke(level);
                    }
                    else
                    {
                        callback?.Invoke(null);
                    }
                };
            }
            //levelAssetData.LevelAsset.LoadAssetAsync<TextAsset>().Completed += handle =>
            //{
            //    if (handle.Status == AsyncOperationStatus.Succeeded)
            //    {
            //        Level level = new Level(levelAssetData.Name,levelAssetData.Description);
            //        level.LoadFromText(handle.Result.text);

            //        callback?.Invoke(level);
            //    }
            //    else
            //    {
            //        callback?.Invoke(null);
            //    }
            //};
        }
    }
}
