using UnityEngine;

namespace GsKit.Settings
{
    [CreateAssetMenu(fileName = "Text Effect Settings", menuName = "GsKit/Settings/Text Effects")]
    public class TextEffectSettings : AbstractSettings
    {
        public override string Category => "Text Effects";

        [Header("Typewriter Defaults")]
        [SerializeField]
        [Tooltip("Time spent between each letter")]
        private float _typewritingDelay = 0.1f;

        [SerializeField]
        [Tooltip("Default sound when playing any letter (null for no sound)")]
        private string _typewritingSoundId = "snd_default_text";

        [Header("Shake Defaults")]
        [SerializeField]
        [Tooltip("The strength of the shake")]
        private float _shakeStrength = 1.0f;

        [SerializeField]
        [Tooltip("Time spent on normal position in seconds")]
        private float _shakePreDelay = 0.025f;

        [SerializeField]
        [Tooltip("Time spent on shook position in seconds")]
        private float _shakePostDelay = 0.025f;

        [SerializeField]
        [Tooltip("Decides if the shake will be for each letter(letter), or as a whole(all)")]
        private string _shakePer = "all";

        [Header("Wave Defaults")]
        [SerializeField]
        [Tooltip("The strength of the wave (affects the max and min y levels)")]
        private float _waveStrength = 1.0f;

        [SerializeField]
        [Tooltip("The speed of the wave")]
        private float _waveSpeed = 1.0f;

        [SerializeField]
        private float _waveStep = 1.0f;

        [SerializeField]
        [Tooltip("Decides if the shake will be for each letter(letter), vertex(vertex), or as a whole(all)")]
        private string _wavePer = "all";

        public float TypewritingDelay => _typewritingDelay;
        public string TypewritingSoundId => _typewritingSoundId;

        public float ShakeStrength => _shakeStrength;
        public float ShakePreDelay => _shakePreDelay;
        public float ShakePostDelay => _shakePostDelay;
        public string ShakePer => _shakePer;

        public float WaveStrength => _waveStrength;
        public float WaveSpeed => _waveSpeed;
        public float WaveStep => _waveStep;
        public string WavePer => _wavePer;
    }
}