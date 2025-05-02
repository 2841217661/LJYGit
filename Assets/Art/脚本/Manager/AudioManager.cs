using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioType
{
    SFX, //音效
    BGM, //背景音乐
    Sound, //语言
    UI, //UI音效
}

public class AudioManager : MonoSingleton<AudioManager>
{
    //由于音效需要调节音调来丰富，每一个音效需要使用单独的AudioSource
    //但是为了节省开销，使用音频对象池来减少AudioSource的创建
    private Queue<AudioSource> sfxAudioPool = new Queue<AudioSource>(); //音频对象池
    [SerializeField] private Transform sfxAudioParent; //音效播放器父物体
    [SerializeField] private int sfxInitialSize = 10; //音效对象池大小

    [SerializeField] private AudioSource bgmSource; //音乐播放器
    [SerializeField] private AudioSource soundSource; //语言播放器
    [SerializeField] private AudioSource uiSource; //ui音效播放器

    private List<GameObject> sfxAudioObject; //TODO:缓存正在播放的音效(当音效的音量被改变时，当前正在播放的音效也应该改变音量)
    private Dictionary<string, AudioClip> sfxCacheAudio; //缓存已打开的音效 
    private Dictionary<string, AudioClip> bgmCacheAudio; //缓存已打开的音乐 
    private Dictionary<string, AudioClip> soundCacheAudio; //缓存已打开的声音 
    private Dictionary<string, AudioClip> uiCacheAudio; //缓存已打开的UI音效 


    protected override void Init()
    {
        base.Init();

        DontDestroyOnLoad(gameObject);

        // 初始化预生成音源
        for (int i = 0; i < sfxInitialSize; i++)
        {
            AudioSource source = CreateNewAudioSource();
            sfxAudioPool.Enqueue(source);
        }

        sfxCacheAudio = new Dictionary<string, AudioClip>();
        bgmCacheAudio = new Dictionary<string, AudioClip>();
        soundCacheAudio = new Dictionary<string, AudioClip>();
        uiCacheAudio = new Dictionary<string, AudioClip>();
    }


    //添加对应音频对象池大小的物体并实例加入对象池中
    private AudioSource CreateNewAudioSource()
    {
        GameObject go = new GameObject("SFXPooledAudioSource");
        go.transform.SetParent(sfxAudioParent);
        AudioSource source = go.AddComponent<AudioSource>();
        go.SetActive(false);
        return source;
    }

    //辅助函数：加载音频，需要确保音频文件在Resources文件下
    public AudioClip LoadAudio(string path)
    {
        return (AudioClip)Resources.Load(path);
    }

    //辅助函数：获取音频，并将其缓存在字典中，避免重复加载
    public AudioClip GetAudio(string path, AudioType audioType)
    {
        switch (audioType)
        {
            case AudioType.SFX:
                if (!sfxCacheAudio.ContainsKey(path))
                {
                    sfxCacheAudio[path] = LoadAudio(path);
                }
                return sfxCacheAudio[path];
            case AudioType.BGM:
                if (!bgmCacheAudio.ContainsKey(path))
                {
                    bgmCacheAudio[path] = LoadAudio(path);
                }
                return bgmCacheAudio[path];
            case AudioType.Sound:
                if (!soundCacheAudio.ContainsKey(path))
                {
                    soundCacheAudio[path] = LoadAudio(path);
                }
                return soundCacheAudio[path];
            case AudioType.UI:
                if (!uiCacheAudio.ContainsKey(path))
                {
                    uiCacheAudio[path] = LoadAudio(path);
                }
                return uiCacheAudio[path];
            default:
                Debug.LogError("无该类型");
                return null;
        }
    }


    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="_path">音效路径</param>
    /// <param name="_audioType">音效类型</param>
    /// <param name="_minTone">最小声调</param>
    /// <param name="_maxTone">最大声调</param>
    /// <param name="_minDis">最小可听范围</param>
    /// <param name="_maxDis">最大可听范围</param>
    public void PlaySound(string _path, AudioType _audioType, Vector3 _playPoint, float _minTone = 0.8f, float _maxTone = 1.2f, float _minDis = 10f, float _maxDis = 20f)
    {
        //PlayOneShot可以叠加播放
        switch (_audioType)
        {
            case AudioType.SFX:
                PlaySFX(GetAudio(_path, _audioType), _playPoint,_minTone, _maxTone,_minDis,_maxDis);

                break;
            case AudioType.BGM:
                bgmSource.PlayOneShot(GetAudio(_path, _audioType));
                break;
            case AudioType.Sound:
                soundSource.PlayOneShot(GetAudio(_path, _audioType));
                break;
            case AudioType.UI:
                //soundSource.PlayOneShot(GetAudio(_path, _audioType));
                break;
            default:
                Debug.LogError("错误类型");
                break;
        }
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="_audioClip">音效</param>
    /// <param name="_minTone">最小音调</param>
    /// <param name="_maxTone">最大音调</param>
    /// <param name="_playPoint">音效播放位置</param>
    /// <param name="_minDis">可听最小距离</param>
    /// <param name="_maxDis">可听最大距离</param>
    private void PlaySFX(AudioClip _audioClip, Vector3 _playPoint, float _minTone, float _maxTone,  float _minDis, float _maxDis)
    {
        AudioSource source;

        // 如果池中有可用的 AudioSource 就取出来使用
        if (sfxAudioPool.Count > 0)
        {
            source = sfxAudioPool.Dequeue();
        }
        else
        {
            // 如果池已空，就创建一个新的(动态扩容)
            source = CreateNewAudioSource();
        }

        //激活播放器并设置播放位置
        source.gameObject.SetActive(true);
        source.transform.position = _playPoint; 

        source.clip = _audioClip;

        source.pitch = Random.Range(_minTone, _maxTone); //随机改变音调，丰富音效
        source.volume = 1f; // 默认音量
        source.spatialBlend = 1.0f; //3D音效
        //设置可听范围
        source.minDistance = _minDis;
        source.maxDistance = _maxDis;

        source.Play();

        // 开启协程：等音效播放完就回收到对象池
        StartCoroutine(ReturnToPoolAfterPlaying(source));
    }

    //播放完毕后回收到对象池
    private System.Collections.IEnumerator ReturnToPoolAfterPlaying(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        source.clip = null;
        source.gameObject.SetActive(false);
        sfxAudioPool.Enqueue(source);
    }



    /// <summary>
    /// 改变指定类型的音量大小
    /// </summary>
    /// <param name="_audioType">类型</param>
    /// <param name="_voluem">目标值</param>
    public void SetSoundValue(AudioType _audioType, float _voluem)
    {
        _voluem = Mathf.Clamp01(_voluem);

        switch (_audioType)
        {
            case AudioType.SFX:
                
                break;
            case AudioType.BGM:
                bgmSource.volume = _voluem;
                break;
            case AudioType.Sound:
                soundSource.volume = _voluem;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 设置全局音量大小
    /// </summary>
    /// <param name="_voluem">目标值</param>
    public void SetGlobalVoluem(float _voluem)
    {
        _voluem = Mathf.Clamp01(_voluem);

        AudioListener.volume = _voluem;
    }
}