using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

public interface IAudioUtil
{
    public static void PlayAudio(EAudioType f_AudioType)
    {
        GTools.AudioMgr.PlayAudio(f_AudioType);
    }
    public static async UniTask<AudioClip> LoadAudioAsync(EAssetKey f_AssetKey)
    {
        if (!GTools.TableMgr.TryGetAssetPath(f_AssetKey, out var path))
        {
            return null;
        }
        var asset = await Resources.LoadAsync<AudioClip>(path);
        var audio = asset as AudioClip;
        return audio;
    }
    
}
public class AudioMgr : MonoSingleton<AudioMgr>
{
    [SerializeField]
    private AudioSource m_AudioSource = null;
    [SerializeField]
    private Dictionary<EAudioType, AudioClip> m_AudioClipList = new();
    [Button]
    public void PlayAudio(EAudioType f_AudioType)
    {
        //if (!GTools.TableMgr.TryGetAudioInfo(f_AudioType, out var audioInfo))
        //{
        //    return;
        //}
        //var audio = await IAudioUtil.LoadAudioAsync(audioInfo.AudioAssetKey);
        ////audio.
        ///

        if (!m_AudioClipList.TryGetValue(f_AudioType, out var audioClip))
        {
            return;
        }
        //m_AudioSource.clip = audioClip;
        m_AudioSource.PlayOneShot(audioClip);
    }
    public void PlayBackground(EAudioType f_AudioType)
    {
        if (!m_AudioClipList.TryGetValue(f_AudioType, out var audioClip))
        {
            return;
        }
        //m_AudioSource.clip = audioClip;
        m_AudioSource.Stop();
        m_AudioSource.clip = audioClip;
        m_AudioSource.Play();
    }

    protected override async void Awake()
    {
        base.Awake();
        m_AudioClipList.Clear();
        UniTask[] tasks = new UniTask[(int)EAudioType.EnumCount];
        for (int i = 0; i < (int)EAudioType.EnumCount; i++)
        {
            var audioType = (EAudioType)i;
            tasks[i] = UniTask.Create(async () =>
            {
                if (!GTools.TableMgr.TryGetAudioInfo(audioType, out var audioInfo))
                {
                    return;
                }
                var audio = await IAudioUtil.LoadAudioAsync(audioInfo.AudioAssetKey);
                m_AudioClipList.Add(audioType, audio);
            });
        }
        await UniTask.WhenAll(tasks);

        PlayBackground(EAudioType.Scene_GameEntrance);
    }

}
