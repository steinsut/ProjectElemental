using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using GsKit.Audio;
using GsKit.Pooling;
using GsKit.Resources;
using GsKit.Settings;
using UnityEngine.SceneManagement;

namespace GsKit
{
    public class InitializerScript : MonoBehaviour
    {
        [SerializeField] private float _rotateSpeed;

        [SerializeField] private TMPro.TextMeshProUGUI _resourceText;

        [SerializeField] private TMPro.TextMeshProUGUI _poolText;

        [SerializeField] private TMPro.TextMeshProUGUI _statusText;

        [SerializeField]
        [Tooltip("GsKit settings. You can change this directly to use different presets.")]
        private GsSettings _settings;

        [SerializeField]
        private string _targetScene; 

        private int _poolCount = 0;
        private int _poolsPopulated = 0;
        private bool _resourcesLoaded = false;

        private ResourceService _resourceService;
        private SettingsService _settingsService;
        private PoolService _poolService;

        private void Awake()
        {
            AsyncOperationHandle initializeHandle = Addressables.InitializeAsync();
            initializeHandle.WaitForCompletion();

            _resourceService = new GameObject("ResourceService").AddComponent<ResourceService>();
            DontDestroyOnLoad(_resourceService);

            _settingsService = new SettingsService(_settings);

            _poolService = new GameObject("PoolService").AddComponent<PoolService>();
            DontDestroyOnLoad(_poolService);
        }

        // Start is called before the first frame update
        private void Start()
        {
            _poolService.PoolCreated += OnPoolCreated;
            AsyncOperationHandle<IList<AbstractResource>> handle = _resourceService.LoadResourceGroup("default");
            handle.Completed += (AsyncOperationHandle<IList<AbstractResource>> hand) =>
            {
                if (hand.Status == AsyncOperationStatus.Succeeded)
                {
                    _resourceText.text += "\nDone!";
                    _resourceText.color = Color.green;
                    _resourcesLoaded = true;
                }
                else if (hand.Status == AsyncOperationStatus.Failed)
                {
                    _resourceText.text += "\nFailed!";
                    _resourceText.color = Color.red;
                    _statusText.text = "Failed!";
                    _statusText.color = Color.red;
                }
            };
        }

        // Update is called once per frame
        private void Update()
        {
            transform.Rotate(new Vector3(0, 0, _rotateSpeed * Time.deltaTime));
            if (_resourcesLoaded)
                if (_poolsPopulated == _poolCount)
                {
                    _poolText.text += "\nDone!";
                    _poolText.color = Color.green;
                    _statusText.text = "Done!";
                    _statusText.color = Color.green;
                    _resourcesLoaded = false;
                    SceneManager.LoadSceneAsync(_targetScene);
                }
        }

        protected virtual void OnPoolPopulated(object sender, EventArgs args)
        {
            _poolsPopulated++;
            ((Pool)sender).PoolPopulated -= OnPoolPopulated;
        }

        protected virtual void OnPoolCreated(object sender, Pool pool)
        {
            _poolCount++;
            _poolService.PoolCreated -= OnPoolCreated;
            pool.PoolPopulated += OnPoolPopulated;
        }
    }
}