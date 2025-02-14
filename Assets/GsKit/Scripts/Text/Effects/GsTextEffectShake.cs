using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GsKit.Settings;
using GsKit.Service;

namespace GsKit.Text.Effects
{
    public class GsTextEffectShake : IGsTextEffect
    {
        private TMP_Text _tmp;
        private TextEffectSettings _effectSettings;
        private int _tmpIndex;
        private int _length;
        private float _time;
        private int _maxVisibleChars = 0;
        private Vector3[] _previousDiffs;
        private bool _post;
        private float _strength;
        private float _preDelay;
        private float _postDelay;
        private string _per;
        private Action _applyEffect = () => { Debug.Log("Not set."); };

        public void Setup(TMP_Text tmp, Dictionary<string, string> attributes)
        {
            _tmp = tmp;
            _effectSettings = ServiceLocator.Instance.GetService<SettingsService>().Settings.Text.Effects;
            _strength = float.Parse(attributes.GetValueOrDefault("strength", _effectSettings.ShakeStrength.ToString()));
            _preDelay = float.Parse(attributes.GetValueOrDefault("pre-delay", _effectSettings.ShakePreDelay.ToString()));
            _postDelay = float.Parse(attributes.GetValueOrDefault("post-delay", _effectSettings.ShakePostDelay.ToString()));
            _per = attributes.GetValueOrDefault("per", _effectSettings.ShakePer);
        }

        public void SetTMPIndex(int index)
        {
            _tmpIndex = index;
        }

        public void SetLength(int length)
        {
            _length = length;
            switch (_per)
            {
                case "letter":
                    _applyEffect = ShakeByLetter;
                    _previousDiffs = new Vector3[_length];
                    break;

                case "all":
                default:
                    _applyEffect = ShakeAll;
                    _previousDiffs = new Vector3[1];
                    break;
            }
            for (int i = 0; i < _previousDiffs.Length; i++) { _previousDiffs[i] = Vector3.zero; }
        }

        public void Update()
        {
            _time += Time.deltaTime;
            if (_maxVisibleChars != _tmp.maxVisibleCharacters)
            {
                for (int i = 0; i < _previousDiffs.Length; i++) { _previousDiffs[i] = Vector3.zero; }
                _maxVisibleChars = _tmp.maxVisibleCharacters;
            }
            _applyEffect();
        }

        private void ShakeAll()
        {
            TMP_TextInfo textInfo = _tmp.textInfo;
            if (!_post)
            {
                if (_time >= _preDelay)
                {
                    _post = true;
                    _time -= _preDelay;

                    Vector3 newDiff = Random.insideUnitCircle * _strength * 0.075f;
                    for (int i = 0; i < _length; i++)
                    {
                        TMP_CharacterInfo charInfo = textInfo.characterInfo[_tmpIndex + i];
                        Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                        int vertexIndex = charInfo.vertexIndex;

                        if (!charInfo.isVisible) continue;

                        vertices[vertexIndex] = vertices[vertexIndex] + newDiff;
                        vertices[vertexIndex + 1] = vertices[vertexIndex + 1] + newDiff;
                        vertices[vertexIndex + 2] = vertices[vertexIndex + 2] + newDiff;
                        vertices[vertexIndex + 3] = vertices[vertexIndex + 3] + newDiff;
                    }
                    _previousDiffs[0] = newDiff;
                }
            }
            else
            {
                if (_time >= _postDelay)
                {
                    _post = false;
                    _time -= _postDelay;

                    Vector3 previousDiff = _previousDiffs[0];
                    for (int i = 0; i < _length; i++)
                    {
                        TMP_CharacterInfo charInfo = textInfo.characterInfo[_tmpIndex + i];
                        Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                        int vertexIndex = charInfo.vertexIndex;

                        if (!charInfo.isVisible) continue;

                        vertices[vertexIndex] = vertices[vertexIndex] - previousDiff;
                        vertices[vertexIndex + 1] = vertices[vertexIndex + 1] - previousDiff;
                        vertices[vertexIndex + 2] = vertices[vertexIndex + 2] - previousDiff;
                        vertices[vertexIndex + 3] = vertices[vertexIndex + 3] - previousDiff;
                    }
                }
            }
        }

        private void ShakeByLetter()
        {
            TMP_TextInfo textInfo = _tmp.textInfo;
            if (!_post)
            {
                if (_time >= _preDelay)
                {
                    _post = true;
                    _time -= _preDelay;

                    for (int i = 0; i < _length; i++)
                    {
                        Vector3 newDiff = Random.insideUnitCircle * _strength * 0.075f;
                        TMP_CharacterInfo charInfo = textInfo.characterInfo[_tmpIndex + i];
                        Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                        int vertexIndex = charInfo.vertexIndex;

                        if (!charInfo.isVisible) continue;

                        vertices[vertexIndex] = vertices[vertexIndex] + newDiff;
                        vertices[vertexIndex + 1] = vertices[vertexIndex + 1] + newDiff;
                        vertices[vertexIndex + 2] = vertices[vertexIndex + 2] + newDiff;
                        vertices[vertexIndex + 3] = vertices[vertexIndex + 3] + newDiff;

                        _previousDiffs[i] = newDiff;
                    }
                }
            }
            else
            {
                if (_time >= _postDelay)
                {
                    _post = false;
                    _time -= _postDelay;

                    for (int i = 0; i < _length; i++)
                    {
                        TMP_CharacterInfo charInfo = textInfo.characterInfo[_tmpIndex + i];
                        Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                        int vertexIndex = charInfo.vertexIndex;

                        if (!charInfo.isVisible) continue;

                        Vector3 previousDiff = _previousDiffs[i];

                        vertices[vertexIndex] = vertices[vertexIndex] - previousDiff;
                        vertices[vertexIndex + 1] = vertices[vertexIndex + 1] - previousDiff;
                        vertices[vertexIndex + 2] = vertices[vertexIndex + 2] - previousDiff;
                        vertices[vertexIndex + 3] = vertices[vertexIndex + 3] - previousDiff;
                    }
                }
            }
        }
    }
}