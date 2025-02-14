using System;
using System.Collections.Generic;
using UnityEngine;
using GsKit.Extensions;
using GsKit.Text;

namespace GsKit.Settings
{
    [Serializable]
    [CreateAssetMenu(fileName = "Text Settings", menuName = "GsKit/Settings/Text")]
    public class TextSettings : AbstractSettings
    {
        [Serializable]
        private class StringPair
        {
            [Tooltip("The tag to handle.")] public string Tag;

            [Tooltip("The class that will be handling the tag. (e.g. GsSettings.Text.Effects.GsTextEffectShake)")]
            public string Handler;
        }

        [SerializeField]
        private bool _disableTMPTags = false;

        [SerializeField]
        private bool _disableDefaultGsTags = false;

        [SerializeField]
        private bool _disableCustomTags = false;

        [SerializeField]
        [Tooltip("For adding custom tags.")]
        private List<StringPair> _tagHandlers = new List<StringPair>();

        [Header("Effect Settings")]
        [SerializeField]
        private TextEffectSettings _effectSettings;

        public Dictionary<string, string> TagHandlers
        {
            get
            {
                var ret = new Dictionary<string, string>();
                foreach (var pair in _tagHandlers) ret.Add(pair.Tag, pair.Handler);
                return ret;
            }
        }

        public override string Category => "Text";

        public bool DisableTMPTags => _disableTMPTags;
        public bool DisableDefaultGsTags => _disableDefaultGsTags;
        public bool DisableCustomTags => _disableCustomTags;

        public TextEffectSettings Effects => _effectSettings;
    }
}