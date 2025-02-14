using System.Collections.Generic;
using UnityEngine;
using GsKit.Resources;

namespace GsKit.Settings
{
    [System.Serializable]
    public abstract class AbstractSettings : AbstractResource
    {
        [SerializeField]
        [Tooltip("This is in case a developer using this kit wants to use its settings system to store settings for their own code.")]
        protected List<AbstractSettings> _customSettings;

        public abstract string Category { get; }

        public List<AbstractSettings> CustomSettings => _customSettings;
    }
}