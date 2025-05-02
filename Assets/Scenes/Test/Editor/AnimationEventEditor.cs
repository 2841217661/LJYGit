using UnityEditor;
using UnityEngine;

public static class AnimationEventEditor
{
    [MenuItem("Tools/添加动画事件/攻击动画事件(帧)")]
    static void AddAttackAnimationEventByFrame()
    {
        string clipPath = "Assets/Resources/Test.anim";
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);

        if (clip == null)
        {
            Debug.LogError("找不到动画剪辑：" + clipPath);
            return;
        }

        // 设置你想插入的帧数（例如：第 18 帧）
        int targetFrame = 18;

        // 获取帧率
        float frameRate = clip.frameRate; // 默认为 60，如果导入设置没改

        // 帧数转时间
        float eventTime = targetFrame / frameRate;

        // 创建动画事件
        AnimationEvent evt = new AnimationEvent();
        evt.time = eventTime;
        evt.functionName = "Test"; // 替换为你需要触发的方法名

        // 防止重复添加同帧的事件
        AnimationEvent[] events = AnimationUtility.GetAnimationEvents(clip);
        foreach (var e in events)
        {
            if (Mathf.Approximately(e.time, eventTime) && e.functionName == evt.functionName)
            {
                Debug.LogWarning("该帧的相同动画事件已存在，跳过添加。");
                return;
            }
        }

        // 添加事件
        var newEvents = new AnimationEvent[events.Length + 1];
        events.CopyTo(newEvents, 0);
        newEvents[events.Length] = evt;

        AnimationUtility.SetAnimationEvents(clip, newEvents);

        AssetDatabase.SaveAssets();
        Debug.Log($"动画事件已添加成功！帧数: {targetFrame}，时间点: {eventTime:F3} 秒");
    }
}
