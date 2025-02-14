using UnityEngine;

namespace GsKit.Audio
{
    [CreateAssetMenu(fileName = "Audio Track Resource", menuName = "GsKit/Audio/Audio Track Resource")]
    [System.Serializable]
    public class AudioTrackResource : Resources.AbstractResource
    {
        [Tooltip("The name of this track.")]
        [SerializeField]
        private string _audioName;

        [Tooltip("A value to make seperating audio resources easier.")]
        [SerializeField]
        private AudioType _audioType;

        [Tooltip("The audio clip.")]
        [SerializeField]
        private AudioClip _clip;

        [Header("Metadata")]
        [Tooltip("Cover image of the audio.")]
        [SerializeField]
        private Sprite _audioCover;

        [Tooltip("The artist of this audio.")]
        [SerializeField]
        private string _artist;

        public string AudioName => _audioName;
        public AudioType AudioType => _audioType;

        public AudioClip Clip => _clip;

        public Sprite AudioCover => _audioCover;
        public string Artist => _artist;
    }
}