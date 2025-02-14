using System;
using UnityEngine;
using GsKit.Service;
using GsKit.Text;

namespace GsKit.Settings
{
    public class SettingsService : IService
    {
        public SettingsService(GsSettings settings)
        {
            ServiceLocator.Instance.RegisterService(this);
            _settings = settings;

            GsText.ResetHandlers();
            foreach (var entry in settings.Text.TagHandlers)
            {
                Type type = Type.GetType(entry.Value);
                if (type != null) GsText.AddTagHandler(entry.Key, type);
            }
        }

        private GsSettings _settings;

        public GsSettings Settings => _settings;

        public AbstractSettings GetSettingsByCategory(string category)
        {
            switch (category)
            {
                case "Text":
                    return Settings.Text;

                default:
                    foreach (AbstractSettings settings in Settings.CustomSettings)
                    {
                        if (settings.Category.Equals(category))
                        {
                            return settings;
                        }
                    }
                    throw new ArgumentException($"Category \n{category}\n was not found.");
            }
        }
    }
}