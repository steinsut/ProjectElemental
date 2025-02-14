using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GsKit.Settings;
using GsKit.Service;

namespace GsKit.Text.Effects
{
    public class GsTextEffectWave : IGsTextEffect
    {
        private TMP_Text _tmp;
        private TextEffectSettings _effectSettings;
        private int _tmpIndex;
        private int _length;
        private int _maxVisibleChars = 0;
        private Vector3[] _previousDiffs;
        private float _speed;
        private float _strength;
        private float _step;
        private string _per;
        private Action _applyEffect = () => { Debug.Log("Not set."); };

        public void Setup(TMP_Text tmp, Dictionary<string, string> attributes)
        {
            _tmp = tmp;
            _effectSettings = ServiceLocator.Instance.GetService<SettingsService>().Settings.Text.Effects;
            _speed = float.Parse(attributes.GetValueOrDefault("speed", _effectSettings.WaveSpeed.ToString()));
            _strength = float.Parse(attributes.GetValueOrDefault("strength", _effectSettings.WaveStrength.ToString()));
            _step = float.Parse(attributes.GetValueOrDefault("step", _effectSettings.WaveStep.ToString()));
            _per = attributes.GetValueOrDefault("per", _effectSettings.WavePer);
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
                    _applyEffect = WaveByLetter;
                    _previousDiffs = new Vector3[_length];
                    break;

                case "vertex":
                    _applyEffect = WaveByVertex;
                    _previousDiffs = new Vector3[_length * 2];
                    break;

                case "all":
                default:
                    _applyEffect = WaveAll;
                    _previousDiffs = new Vector3[1];
                    break;
            }
            for (int i = 0; i < _previousDiffs.Length; i++) { _previousDiffs[i] = Vector3.zero; }
        }

        public void Update()
        {
            if (_maxVisibleChars != _tmp.maxVisibleCharacters)
            {
                for (int i = 0; i < _previousDiffs.Length; i++) { _previousDiffs[i] = Vector3.zero; }
                _maxVisibleChars = _tmp.maxVisibleCharacters;
            }
            _applyEffect();
        }

        private void WaveAll()
        {
            TMP_TextInfo textInfo = _tmp.textInfo;
            Vector3 newDiff = new Vector3(0, Mathf.Sin(Time.time * 2 * _speed) * _strength, 0);
            for (int i = 0; i < _length; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[_tmpIndex + i];
                Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                int vertexIndex = charInfo.vertexIndex;

                if (!charInfo.isVisible) continue;

                Vector3 previousDiff = _previousDiffs[0];

                vertices[vertexIndex] = vertices[vertexIndex] - previousDiff;
                vertices[vertexIndex + 1] = vertices[vertexIndex + 1] - previousDiff;
                vertices[vertexIndex + 2] = vertices[vertexIndex + 2] - previousDiff;
                vertices[vertexIndex + 3] = vertices[vertexIndex + 3] - previousDiff;

                vertices[vertexIndex] = vertices[vertexIndex] + newDiff;
                vertices[vertexIndex + 1] = vertices[vertexIndex + 1] + newDiff;
                vertices[vertexIndex + 2] = vertices[vertexIndex + 2] + newDiff;
                vertices[vertexIndex + 3] = vertices[vertexIndex + 3] + newDiff;
            }
            _previousDiffs[0] = newDiff;
        }

        private void WaveByLetter()
        {
            TMP_TextInfo textInfo = _tmp.textInfo;
            for (int i = 0; i < _length; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[_tmpIndex + i];
                Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                int vertexIndex = charInfo.vertexIndex;

                if (!charInfo.isVisible) continue;

                Vector3 previousDiff = _previousDiffs[i];
                Vector3 newDiff = new Vector3(0, Mathf.Sin((Time.time + 0.05f * _step * i) * 2 * _speed) * _strength, 0);

                vertices[vertexIndex] = vertices[vertexIndex] - previousDiff;
                vertices[vertexIndex + 1] = vertices[vertexIndex + 1] - previousDiff;
                vertices[vertexIndex + 2] = vertices[vertexIndex + 2] - previousDiff;
                vertices[vertexIndex + 3] = vertices[vertexIndex + 3] - previousDiff;

                vertices[vertexIndex] = vertices[vertexIndex] + newDiff;
                vertices[vertexIndex + 1] = vertices[vertexIndex + 1] + newDiff;
                vertices[vertexIndex + 2] = vertices[vertexIndex + 2] + newDiff;
                vertices[vertexIndex + 3] = vertices[vertexIndex + 3] + newDiff;

                _previousDiffs[i] = newDiff;
            }
        }

        private void WaveByVertex()
        {
            TMP_TextInfo textInfo = _tmp.textInfo;
            int stepCount = -1;
            for (int i = 0; i < _length; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[_tmpIndex + i];
                Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                int vertexIndex = charInfo.vertexIndex;

                if (!charInfo.isVisible) continue;

                stepCount++;
                Vector3 previousDiff = _previousDiffs[stepCount];
                Vector3 newDiff = new Vector3(0, Mathf.Sin((Time.time + 0.05f * _step * stepCount) * 1.5f * _speed) * _strength, 0);

                vertices[vertexIndex] = vertices[vertexIndex] - previousDiff;
                vertices[vertexIndex + 1] = vertices[vertexIndex + 1] - previousDiff;

                vertices[vertexIndex] = vertices[vertexIndex] + newDiff;
                vertices[vertexIndex + 1] = vertices[vertexIndex + 1] + newDiff;

                _previousDiffs[stepCount] = newDiff;

                stepCount++;
                previousDiff = _previousDiffs[stepCount];
                newDiff = new Vector3(0, Mathf.Sin((Time.time + 0.05f * _step * stepCount) * 1.5f * _speed) * _strength, 0);

                vertices[vertexIndex + 2] = vertices[vertexIndex + 2] - previousDiff;
                vertices[vertexIndex + 3] = vertices[vertexIndex + 3] - previousDiff;

                vertices[vertexIndex + 2] = vertices[vertexIndex + 2] + newDiff;
                vertices[vertexIndex + 3] = vertices[vertexIndex + 3] + newDiff;

                _previousDiffs[stepCount] = newDiff;
            }
        }
    }
}