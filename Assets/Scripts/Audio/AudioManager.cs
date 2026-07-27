using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class AudioClip_
    {
        public string name;
        public AudioClip clip;
        public float volume = 1f;
        public bool loop = false;
    }
    
    [SerializeField] private List<AudioClip_> audioClips = new();
    [SerializeField] private int maxAudioSources = 10;
    [SerializeField] private float masterVolume = 1f;
    
    private List<AudioSource> audioSourcePool = new();
    private Dictionary<string, AudioClip_> audioLibrary = new();
    private static AudioManager instance;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeAudioPool();
        BuildAudioLibrary();
    }
    
    private void InitializeAudioPool()
    {
        for (int i = 0; i < maxAudioSources; i++)
        {
            GameObject audioObj = new($"AudioSource_{i}");
            audioObj.transform.SetParent(transform);
            
            AudioSource source = audioObj.AddComponent<AudioSource>();
            audioSourcePool.Add(source);
        }
        
        Debug.Log($"[AudioManager] Initialized {maxAudioSources} audio sources");
    }
    
    private void BuildAudioLibrary()
    {
        audioLibrary.Clear();
        
        foreach (var clip in audioClips)
        {
            if (clip.clip != null)
            {
                audioLibrary[clip.name] = clip;
            }
        }
        
        Debug.Log($"[AudioManager] Loaded {audioLibrary.Count} audio clips");
    }
    
    public void PlaySFX(string clipName)
    {
        if (!audioLibrary.TryGetValue(clipName, out var clipData))
        {
            Debug.LogWarning($"[AudioManager] Audio clip '{clipName}' not found");
            return;
        }
        
        AudioSource source = GetAvailableAudioSource();
        if (source == null) return;
        
        source.clip = clipData.clip;
        source.volume = clipData.volume * masterVolume;
        source.spatialBlend = 0f;
        source.Play();
    }
    
    public void Play3DSFX(string clipName, Vector3 position)
    {
        if (!audioLibrary.TryGetValue(clipName, out var clipData))
            return;
        
        AudioSource source = GetAvailableAudioSource();
        if (source == null) return;
        
        source.transform.position = position;
        source.clip = clipData.clip;
        source.volume = clipData.volume * masterVolume;
        source.spatialBlend = 1f;
        source.Play();
    }
    
    public void PlayMusic(string clipName)
    {
        if (!audioLibrary.TryGetValue(clipName, out var clipData))
            return;
        
        AudioSource source = GetAvailableAudioSource();
        if (source == null) return;
        
        source.clip = clipData.clip;
        source.volume = clipData.volume * masterVolume * 0.7f;
        source.loop = true;
        source.Play();
    }
    
    public void StopAll()
    {
        foreach (var source in audioSourcePool)
        {
            source.Stop();
        }
    }
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
    }
    
    private AudioSource GetAvailableAudioSource()
    {
        foreach (var source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        
        Debug.LogWarning("[AudioManager] No available audio sources");
        return null;
    }
    
    public static AudioManager Instance => instance;
}