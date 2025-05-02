using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特效管理器：实现特效的对象池复用机制，支持同一特效多次并发播放，
/// 按路径分类池，避免同一 prefab 实例发生位置或状态干扰。
/// 使用 Resources 加载特效 prefab。
/// </summary>
public class EffectManager : MonoSingleton<EffectManager>
{
    [SerializeField] private int effectInitialSize = 10;   // 每种特效初始池大小（暂未用到，但可扩展）
    [SerializeField] private int effectMaxPoolSize = 30;   // 每种特效最大池容量限制

    private Dictionary<string, GameObject> cacheEffect;                      // 缓存加载的特效 prefab，避免重复 Resources.Load
    private Dictionary<string, Queue<GameObject>> effectPools;               // 每种特效（以 path 区分）对应的对象池

    /// <summary>
    /// 初始化方法，在 MonoSingleton 基类中调用
    /// </summary>
    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(gameObject); // 保持场景切换不销毁

        cacheEffect = new Dictionary<string, GameObject>();
        effectPools = new Dictionary<string, Queue<GameObject>>();
    }

    /// <summary>
    /// 加载特效 prefab（从 Resources 文件夹）
    /// </summary>
    public GameObject LoadEffectGameObject(string path)
    {
        return (GameObject)Resources.Load(path);
    }

    /// <summary>
    /// 获取并缓存特效 prefab，避免重复加载
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
    /// 播放特效（实例化或从池中取出）
    /// </summary>
    /// <param name="path">特效 prefab 的 Resources 路径</param>
    /// <param name="position">播放位置</param>
    /// <param name="rotation">播放旋转（例如朝左朝右）</param>
    public void InitEffectGameObject(string path, Vector3 position, Quaternion rotation)
    {
        GameObject effect = GetEffectFromPool(path);
        if (effect == null)
        {
            Debug.LogError("Failed to instantiate effect for path: " + path);
            return;
        }

        // 设置特效位置、朝向和父级
        effect.transform.position = position;
        effect.transform.rotation = rotation;
        effect.transform.SetParent(transform); // 将特效统一挂在 EffectManager 节点下，便于管理

        // 启动协程，等待粒子系统播放完毕后自动回收
        StartCoroutine(WaitEffectRecycle(path, effect));
    }

    /// <summary>
    /// 从对象池中获取特效对象（按 path 分类）
    /// </summary>
    private GameObject GetEffectFromPool(string path)
    {
        if (!effectPools.ContainsKey(path))
        {
            effectPools[path] = new Queue<GameObject>(); // 首次使用此 path，创建对应队列
        }

        Queue<GameObject> pool = effectPools[path];

        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();  // 从池中取出对象
            obj.SetActive(true);              // 激活对象
            return obj;
        }
        else
        {
            // 池中没有可用对象，创建新实例
            GameObject prefab = GetEffectGameObject(path);
            if (prefab == null)
                return null;

            return Instantiate(prefab);
        }
    }

    /// <summary>
    /// 回收特效对象到对应的对象池（按 path 分类）
    /// </summary>
    private void RecycleEffect(string path, GameObject effect)
    {
        if (!effectPools.ContainsKey(path))
        {
            effectPools[path] = new Queue<GameObject>();
        }

        Queue<GameObject> pool = effectPools[path];

        effect.SetActive(false); // 关闭对象显示

        if (pool.Count >= effectMaxPoolSize)
        {
            // 如果池已满，销毁对象（节省内存）
            Destroy(effect);
        }
        else
        {
            // 池未满，回收对象供下次复用
            pool.Enqueue(effect);
        }
    }

    /// <summary>
    /// 等待所有粒子播放完后自动回收
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
                if (ps.IsAlive(true)) // 判断粒子系统是否还在播放
                {
                    playing = true;
                    break;
                }
            }
            yield return null; // 每帧检查一次
        }

        RecycleEffect(path, effect); // 播放完毕后回收对象
    }
}
