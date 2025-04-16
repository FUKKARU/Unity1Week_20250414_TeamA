using NGeneral;
using UnityEngine;
using UnityEngine.Audio;

namespace NInGame
{
    public sealed class AudioManager : ASingletonMonoBehaviour<AudioManager>
    {
        public enum AudioType
        {
            BGM,
            SE,
        }

        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioMixerGroup bgmMixerGroup;
        [SerializeField] private AudioMixerGroup seMixerGroup;
        [SerializeField] private AudioSource bgmAudioSource;
        [SerializeField] private AudioSource seAudioSource;

        public void DoPlay(AudioClip audioClip, AudioType audioType)
        {
            if (audioClip == null)
            {
                Debug.LogError("AudioClip is null.");
                return;
            }

            switch (audioType)
            {
                case AudioType.BGM:
                    {
                        if (bgmAudioSource != null)
                        {
                            bgmAudioSource.clip = audioClip;
                            bgmAudioSource.outputAudioMixerGroup = bgmMixerGroup;
                            bgmAudioSource.playOnAwake = false;
                            bgmAudioSource.loop = true;

                            bgmAudioSource.Play();
                        }
                    }
                    break;
                case AudioType.SE:
                    {
                        if (seAudioSource != null)
                        {
                            seAudioSource.outputAudioMixerGroup = seMixerGroup;
                            seAudioSource.playOnAwake = false;
                            seAudioSource.loop = false;

                            seAudioSource.PlayOneShot(audioClip);
                        }
                    }
                    break;
                default:
                    Debug.LogError($"AudioType {audioType} is not supported.");
                    break;
            }
        }

        public void GetVolume(AudioType audioType, out float volume)
        {
            volume = 0f;

            string paramName = GetAudioMixerGroupParamName(audioType);
            if (string.IsNullOrEmpty(paramName))
            {
                Debug.LogError($"AudioType {audioType} is not supported.");
                return;
            }

            if (audioMixer != null)
            {
                audioMixer.GetFloat(paramName, out volume);
            }
        }

        public void SetVolume(AudioType audioType, float volume)
        {
            string paramName = GetAudioMixerGroupParamName(audioType);
            if (string.IsNullOrEmpty(paramName))
            {
                Debug.LogError($"AudioType {audioType} is not supported.");
                return;
            }

            if (audioMixer != null)
            {
                audioMixer.SetFloat(paramName, volume);
            }
        }

        private string GetAudioMixerGroupParamName(AudioType audioType) => audioType switch
        {
            AudioType.BGM => "BgmVolume",
            AudioType.SE => "SeVolume",
            _ => string.Empty,
        };
    }
}