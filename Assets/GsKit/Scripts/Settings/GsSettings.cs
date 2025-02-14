using System.Collections.Generic;
using UnityEngine;

namespace GsKit.Settings
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "All Settings", menuName = "GsKit/Settings/All")]
    public class GsSettings : AbstractSettings
    {
        public override string Category => "All";

        [SerializeField]
        [Tooltip("Settings related to text in GsKit.")]
        private TextSettings _text;

        public TextSettings Text => _text;
    }
}