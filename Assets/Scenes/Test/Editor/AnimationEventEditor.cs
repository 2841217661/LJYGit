using UnityEditor;
using UnityEngine;

public static class AnimationEventEditor
{
    [MenuItem("Tools/��Ӷ����¼�/���������¼�(֡)")]
    static void AddAttackAnimationEventByFrame()
    {
        string clipPath = "Assets/Resources/Test.anim";
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);

        if (clip == null)
        {
            Debug.LogError("�Ҳ�������������" + clipPath);
            return;
        }

        // ������������֡�������磺�� 18 ֡��
        int targetFrame = 18;

        // ��ȡ֡��
        float frameRate = clip.frameRate; // Ĭ��Ϊ 60�������������û��

        // ֡��תʱ��
        float eventTime = targetFrame / frameRate;

        // ���������¼�
        AnimationEvent evt = new AnimationEvent();
        evt.time = eventTime;
        evt.functionName = "Test"; // �滻Ϊ����Ҫ�����ķ�����

        // ��ֹ�ظ����ͬ֡���¼�
        AnimationEvent[] events = AnimationUtility.GetAnimationEvents(clip);
        foreach (var e in events)
        {
            if (Mathf.Approximately(e.time, eventTime) && e.functionName == evt.functionName)
            {
                Debug.LogWarning("��֡����ͬ�����¼��Ѵ��ڣ�������ӡ�");
                return;
            }
        }

        // ����¼�
        var newEvents = new AnimationEvent[events.Length + 1];
        events.CopyTo(newEvents, 0);
        newEvents[events.Length] = evt;

        AnimationUtility.SetAnimationEvents(clip, newEvents);

        AssetDatabase.SaveAssets();
        Debug.Log($"�����¼�����ӳɹ���֡��: {targetFrame}��ʱ���: {eventTime:F3} ��");
    }
}
