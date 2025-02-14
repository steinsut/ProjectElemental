using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using GsKit.Resources;

namespace GsKit.Audio
{
    [CreateAssetMenu(fileName = "Audio Group", menuName = "GsKit/Audio/Audio Group Resource")]
    public class AudioGroupResource : AbstractResource
    {
        [SerializeField]
        [Tooltip("Clips to play.")]
        private List<AudioClip> _audioClips;

        [SerializeField]
        [Tooltip("The order to play the tracks in.")]
        private AudioPlayOrder _playOrder;

        public ReadOnlyCollection<AudioClip> Clips
        { get { return _audioClips.AsReadOnly(); } }

        public AudioPlayOrder PlayOrder
        { get { return _playOrder; } }
    }
}