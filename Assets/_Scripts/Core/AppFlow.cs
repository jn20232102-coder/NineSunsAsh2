using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AppFlow
{
    // 场景 - 需要加入Build Settings
    public string titleScene = "Title";
    public string tutorialScene = "Tutorial";
    public string campScene = "Hub_Camp";
    public string battleScene = "Battle_Map01";
    public string devScene = "DevScene"; // 开发阶段


    // 玩家在场景间保留
    public bool keepPlayerBetweenScenes = true;
    public GameObject playerPrefab; 

    string _currentContent;
    GameObject _player;
    bool _busy;
    public bool Busy => _busy;

    // 对外携程接口
    public IEnumerator GoToTitle(string spawnedID="Default") => CoSwitch(titleScene, spawnedID);
    public IEnumerator GoToTutorial(string spawnId="Default") => CoSwitch(tutorialScene, spawnId);
    public IEnumerator GoToCamp    (string spawnId="Default") => CoSwitch(campScene, spawnId);
    public IEnumerator GoToBattle  (string spawnId="Default") => CoSwitch(battleScene, spawnId);
    
    public IEnumerator GoToDevScene(string spawnId="Default") => CoSwitch(devScene, spawnId); // 开发阶段

    public IEnumerator CoSwitch(string targetScene, string spawnID)
    {
        if (_busy)
        {
            yield break;
        }
        _busy = true;

        // 1) 卸载除 Bootstrap 和 target 以外的所有场景
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (!s.isLoaded) continue;
            if (s.name == "Bootstrap") continue;
            if (s.name == targetScene) continue;
            yield return SceneManager.UnloadSceneAsync(s);
        }

        // 加载目标场景(Additive)
        if (!SceneManager.GetSceneByName(targetScene).isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
        }

        var newScene = SceneManager.GetSceneByName(targetScene);
        SceneManager.SetActiveScene(newScene);

        // 刷新环境
        DynamicGI.UpdateEnvironment();
        EnforceSingleSun(newScene);
        
        // 放置、传送玩家
        if (keepPlayerBetweenScenes)
        {
            if (_player == null)
            {
                _player = Object.Instantiate(playerPrefab);
                Object.DontDestroyOnLoad(_player);
            }

            // 查找SpawnPoint
            var sps = Object.FindObjectsOfType<SpawnPoint>(true);
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            foreach (var sp in sps)
            {
                if (sp.id == spawnID || (spawnID == "Default" && sp.isDefault))
                {
                    pos = sp.transform.position;
                    rot = sp.transform.rotation;
                    break;
                }
            }

            _player.transform.SetPositionAndRotation(pos, rot);
        }
        _busy = false;
    }

    static void EnforceSingleSun(Scene active) // 处理阴影叠加问题
    {
        var suns = Object.FindObjectsOfType<Light>(true).Where(l => l.type == LightType.Directional);
        bool kept = false;
        foreach (var l in suns)
        {
            bool inActive = l.gameObject.scene == active;
            l.enabled = inActive && !kept;   // 只保留激活场景里的第一盏
            if (l.enabled) kept = true;
        }
    }
}
