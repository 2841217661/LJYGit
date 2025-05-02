using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ч��������ʵ����Ч�Ķ���ظ��û��ƣ�֧��ͬһ��Ч��β������ţ�
/// ��·������أ�����ͬһ prefab ʵ������λ�û�״̬���š�
/// ʹ�� Resources ������Ч prefab��
/// </summary>
public class EffectManager : MonoSingleton<EffectManager>
{
    [SerializeField] private int effectInitialSize = 10;   // ÿ����Ч��ʼ�ش�С����δ�õ���������չ��
    [SerializeField] private int effectMaxPoolSize = 30;   // ÿ����Ч������������

    private Dictionary<string, GameObject> cacheEffect;                      // ������ص���Ч prefab�������ظ� Resources.Load
    private Dictionary<string, Queue<GameObject>> effectPools;               // ÿ����Ч���� path ���֣���Ӧ�Ķ����

    /// <summary>
    /// ��ʼ���������� MonoSingleton �����е���
    /// </summary>
    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(gameObject); // ���ֳ����л�������

        cacheEffect = new Dictionary<string, GameObject>();
        effectPools = new Dictionary<string, Queue<GameObject>>();
    }

    /// <summary>
    /// ������Ч prefab���� Resources �ļ��У�
    /// </summary>
    public GameObject LoadEffectGameObject(string path)
    {
        return (GameObject)Resources.Load(path);
    }

    /// <summary>
    /// ��ȡ��������Ч prefab�������ظ�����
    /// </summary>
    public GameObject GetEffectGameObject(string path)
    {
        if (!cacheEffect.ContainsKey(path))
        {
            GameObject prefab = LoadEffectGameObject(path);
            if (prefab == null)
            {
                Debug.LogError("Effect prefab not found: " + path);
                return null;
            }
            cacheEffect[path] = prefab;
        }

        return cacheEffect[path];
    }

    /// <summary>
    /// ������Ч��ʵ������ӳ���ȡ����
    /// </summary>
    /// <param name="path">��Ч prefab �� Resources ·��</param>
    /// <param name="position">����λ��</param>
    /// <param name="rotation">������ת�����糯���ң�</param>
    public void InitEffectGameObject(string path, Vector3 position, Quaternion rotation)
    {
        GameObject effect = GetEffectFromPool(path);
        if (effect == null)
        {
            Debug.LogError("Failed to instantiate effect for path: " + path);
            return;
        }

        // ������Чλ�á�����͸���
        effect.transform.position = position;
        effect.transform.rotation = rotation;
        effect.transform.SetParent(transform); // ����Чͳһ���� EffectManager �ڵ��£����ڹ���

        // ����Э�̣��ȴ�����ϵͳ������Ϻ��Զ�����
        StartCoroutine(WaitEffectRecycle(path, effect));
    }

    /// <summary>
    /// �Ӷ�����л�ȡ��Ч���󣨰� path ���ࣩ
    /// </summary>
    private GameObject GetEffectFromPool(string path)
    {
        if (!effectPools.ContainsKey(path))
        {
            effectPools[path] = new Queue<GameObject>(); // �״�ʹ�ô� path��������Ӧ����
        }

        Queue<GameObject> pool = effectPools[path];

        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();  // �ӳ���ȡ������
            obj.SetActive(true);              // �������
            return obj;
        }
        else
        {
            // ����û�п��ö��󣬴�����ʵ��
            GameObject prefab = GetEffectGameObject(path);
            if (prefab == null)
                return null;

            return Instantiate(prefab);
        }
    }

    /// <summary>
    /// ������Ч���󵽶�Ӧ�Ķ���أ��� path ���ࣩ
    /// </summary>
    private void RecycleEffect(string path, GameObject effect)
    {
        if (!effectPools.ContainsKey(path))
        {
            effectPools[path] = new Queue<GameObject>();
        }

        Queue<GameObject> pool = effectPools[path];

        effect.SetActive(false); // �رն�����ʾ

        if (pool.Count >= effectMaxPoolSize)
        {
            // ��������������ٶ��󣨽�ʡ�ڴ棩
            Destroy(effect);
        }
        else
        {
            // ��δ�������ն����´θ���
            pool.Enqueue(effect);
        }
    }

    /// <summary>
    /// �ȴ��������Ӳ�������Զ�����
    /// </summary>
    private IEnumerator WaitEffectRecycle(string path, GameObject effect)
    {
        ParticleSystem[] psArray = effect.GetComponentsInChildren<ParticleSystem>();
        bool playing = true;

        while (playing)
        {
            playing = false;
            foreach (var ps in psArray)
            {
                if (ps.IsAlive(true)) // �ж�����ϵͳ�Ƿ��ڲ���
                {
                    playing = true;
                    break;
                }
            }
            yield return null; // ÿ֡���һ��
        }

        RecycleEffect(path, effect); // ������Ϻ���ն���
    }
}
