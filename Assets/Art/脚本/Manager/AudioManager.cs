using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioType
{
    SFX, //��Ч
    BGM, //��������
    Sound, //����
    UI, //UI��Ч
}

public class AudioManager : MonoSingleton<AudioManager>
{
    //������Ч��Ҫ�����������ḻ��ÿһ����Ч��Ҫʹ�õ�����AudioSource
    //����Ϊ�˽�ʡ������ʹ����Ƶ�����������AudioSource�Ĵ���
    private Queue<AudioSource> sfxAudioPool = new Queue<AudioSource>(); //��Ƶ�����
    [SerializeField] private Transform sfxAudioParent; //��Ч������������
    [SerializeField] private int sfxInitialSize = 10; //��Ч����ش�С

    [SerializeField] private AudioSource bgmSource; //���ֲ�����
    [SerializeField] private AudioSource soundSource; //���Բ�����
    [SerializeField] private AudioSource uiSource; //ui��Ч������

    private List<GameObject> sfxAudioObject; //TODO:�������ڲ��ŵ���Ч(����Ч���������ı�ʱ����ǰ���ڲ��ŵ���ЧҲӦ�øı�����)
    private Dictionary<string, AudioClip> sfxCacheAudio; //�����Ѵ򿪵���Ч 
    private Dictionary<string, AudioClip> bgmCacheAudio; //�����Ѵ򿪵����� 
    private Dictionary<string, AudioClip> soundCacheAudio; //�����Ѵ򿪵����� 
    private Dictionary<string, AudioClip> uiCacheAudio; //�����Ѵ򿪵�UI��Ч 


    protected override void Init()
    {
        base.Init();

        DontDestroyOnLoad(gameObject);

        // ��ʼ��Ԥ������Դ
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


    //��Ӷ�Ӧ��Ƶ����ش�С�����岢ʵ������������
    private AudioSource CreateNewAudioSource()
    {
        GameObject go = new GameObject("SFXPooledAudioSource");
        go.transform.SetParent(sfxAudioParent);
        AudioSource source = go.AddComponent<AudioSource>();
        go.SetActive(false);
        return source;
    }

    //����������������Ƶ����Ҫȷ����Ƶ�ļ���Resources�ļ���
    public AudioClip LoadAudio(string path)
    {
        return (AudioClip)Resources.Load(path);
    }

    //������������ȡ��Ƶ�������仺�����ֵ��У������ظ�����
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
                Debug.LogError("�޸�����");
                return null;
        }
    }


    /// <summary>
    /// ������Ч
    /// </summary>
    /// <param name="_path">��Ч·��</param>
    /// <param name="_audioType">��Ч����</param>
    /// <param name="_minTone">��С����</param>
    /// <param name="_maxTone">�������</param>
    /// <param name="_minDis">��С������Χ</param>
    /// <param name="_maxDis">��������Χ</param>
    public void PlaySound(string _path, AudioType _audioType, Vector3 _playPoint, float _minTone = 0.8f, float _maxTone = 1.2f, float _minDis = 10f, float _maxDis = 20f)
    {
        //PlayOneShot���Ե��Ӳ���
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
                Debug.LogError("��������");
                break;
        }
    }

    /// <summary>
    /// ������Ч
    /// </summary>
    /// <param name="_audioClip">��Ч</param>
    /// <param name="_minTone">��С����</param>
    /// <param name="_maxTone">�������</param>
    /// <param name="_playPoint">��Ч����λ��</param>
    /// <param name="_minDis">������С����</param>
    /// <param name="_maxDis">����������</param>
    private void PlaySFX(AudioClip _audioClip, Vector3 _playPoint, float _minTone, float _maxTone,  float _minDis, float _maxDis)
    {
        AudioSource source;

        // ��������п��õ� AudioSource ��ȡ����ʹ��
        if (sfxAudioPool.Count > 0)
        {
            source = sfxAudioPool.Dequeue();
        }
        else
        {
            // ������ѿգ��ʹ���һ���µ�(��̬����)
            source = CreateNewAudioSource();
        }

        //������������ò���λ��
        source.gameObject.SetActive(true);
        source.transform.position = _playPoint; 

        source.clip = _audioClip;

        source.pitch = Random.Range(_minTone, _maxTone); //����ı��������ḻ��Ч
        source.volume = 1f; // Ĭ������
        source.spatialBlend = 1.0f; //3D��Ч
        //���ÿ�����Χ
        source.minDistance = _minDis;
        source.maxDistance = _maxDis;

        source.Play();

        // ����Э�̣�����Ч������ͻ��յ������
        StartCoroutine(ReturnToPoolAfterPlaying(source));
    }

    //������Ϻ���յ������
    private System.Collections.IEnumerator ReturnToPoolAfterPlaying(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        source.clip = null;
        source.gameObject.SetActive(false);
        sfxAudioPool.Enqueue(source);
    }



    /// <summary>
    /// �ı�ָ�����͵�������С
    /// </summary>
    /// <param name="_audioType">����</param>
    /// <param name="_voluem">Ŀ��ֵ</param>
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
    /// ����ȫ��������С
    /// </summary>
    /// <param name="_voluem">Ŀ��ֵ</param>
    public void SetGlobalVoluem(float _voluem)
    {
        _voluem = Mathf.Clamp01(_voluem);

        AudioListener.volume = _voluem;
    }
}