using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour, IInteractable
{
    public enum Target { Tutorial, Camp, Battle, Dev}
    public Target target = Target.Camp;
    public string targetSpawnId = "Default";

    [TextArea] public string promptText = "E - 撤离";

    void Reset() 
    { 
        GetComponent<Collider>().isTrigger = true; 
    }

    public string   Prompt        => promptText;
    public Transform PromptAnchor => transform;

    public void Interact(GameObject actor)
    {
        if (AppFlowDriver.I != null)
        {
            switch (target)
            {
                case Target.Tutorial: AppFlowDriver.I.GoToTutorial(targetSpawnId); break;
                case Target.Camp:     AppFlowDriver.I.GoToCamp(targetSpawnId);     break;
                case Target.Battle:   AppFlowDriver.I.GoToBattle(targetSpawnId);   break;
                case Target.Dev:     AppFlowDriver.I.GoToDevScene(targetSpawnId);  break;
            }
            return; // 如果在上面已经切换，返回 -> 避免出现下面的Dev切换，导致场景切换两次出现重叠（场景重复，影子加深等）
        }

        // 开发模式：DevScene中没有AppFlowDriver，直接切换场景
        var sceneName = target switch {
            Target.Tutorial => "Tutorial",
            Target.Camp     => "Hub_Camp",
            _               => "Battle_Map01"
        };
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
