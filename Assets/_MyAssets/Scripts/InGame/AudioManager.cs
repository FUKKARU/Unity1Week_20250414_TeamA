using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using NGeneral;

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
        [SerializeField] private Slider bgmVolumeSlider;
        [SerializeField] private Slider seVolumeSlider;

        private static readonly float MuteVolume = -80f;
        private static readonly float MinVolume = -20f;
        private static readonly float MaxVolume = 20f;
        private static readonly int MinSliderValue = 0;
        private static readonly int MaxSliderValue = 10;

        private void Start()
        {
            if (bgmVolumeSlider != null)
            {
                GetVolume(AudioType.BGM, out float volume);
                bgmVolumeSlider.value = Volume2SliderValue(volume);
                bgmVolumeSlider.onValueChanged.AddListener((value) => SetVolume(AudioType.BGM, SliderValue2Volume(value)));
            }

            if (seVolumeSlider != null)
            {
                GetVolume(AudioType.SE, out float volume);
                seVolumeSlider.value = Volume2SliderValue(volume);
                seVolumeSlider.onValueChanged.AddListener((value) => SetVolume(AudioType.SE, SliderValue2Volume(value)));
            }
        }

        // sliderValue は整数の想定
        private float SliderValue2Volume(float sliderValue) =>
            sliderValue == MinSliderValue ? MuteVolume :
                Mathf.Round(sliderValue.Remap(MinSliderValue, MaxSliderValue, MinVolume, MaxVolume));
        private int Volume2SliderValue(float volume) =>
            Mathf.RoundToInt(volume.Remap(MinVolume, MaxVolume, MinSliderValue, MaxSliderValue));

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