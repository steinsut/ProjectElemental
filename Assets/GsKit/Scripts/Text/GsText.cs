using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using GsKit.Extensions;
using GsKit.Text.Effects;
using GsKit.Audio;
using GsKit.Service;
using GsKit.Pooling;
using GsKit.Resources;
using GsKit.Settings;

namespace GsKit.Text
{
    [RequireComponent(typeof(TextMeshPro))]
    public class GsText : MonoBehaviour
    {
        private ResourceService _resourceService;
        private SettingsService _settingsService;
        private PoolService _poolService;
        private TextSettings _textSettings;
        private TextEffectSettings _effectSettings;
        private TextMeshPro _tmp;
        private string _realText;
        private string _gsSanitizedText;
        private List<GsTextPart> _textParts = new();
        private float _time = 0;
        private bool _isTypewriting = false;
        private float _typewritingDelay = 0;
        private Queue<(int, Dictionary<string, string>)> _typewriterAttributes = new();
        private AbstractResource _twAudio = null;
        private Pool.Object _twAudioPlayerPoolObj;
        private AudioPlayer _twAudioPlayer;

        private static Dictionary<string, Type> s_defaultTagHandlers = new()
        {
            {"wave", typeof(GsTextEffectWave)},
            {"shake", typeof(GsTextEffectShake)}
        };

        private static Dictionary<string, Type> s_tagHandlers = new();

        private static Dictionary<string, Regex> s_defaultTagRegexes = new()
        {
            {"wave", new Regex(@"<(wave)( .*?)?>", RegexOptions.Compiled)},
            {"shake", new Regex(@"<(shake)( .*?)?>", RegexOptions.Compiled)}
        };

        private static Dictionary<string, Regex> s_tagRegexes = new();

        public static void AddTagHandler(string tagToHandle, Type type)
        {
            if (typeof(IGsTextEffect).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement IGsTextEffect.");
            if (type.GetConstructor(Type.EmptyTypes) == null) throw new ArgumentException($"{type} has no parameterless constructor.");
            s_tagHandlers.Add(tagToHandle, type);
            s_tagRegexes.Add(tagToHandle, new Regex($@"<({tagToHandle})( .*?)?>", RegexOptions.Compiled));
        }

        public static void ResetHandlers()
        {
            s_tagHandlers.Clear();
            s_tagRegexes.Clear();
        }

        private void Awake()
        {
            _tmp = GetComponent<TextMeshPro>();
            _resourceService = ServiceLocator.Instance.GetService<ResourceService>();
            _settingsService = ServiceLocator.Instance.GetService<SettingsService>();
            _poolService = ServiceLocator.Instance.GetService<PoolService>();
        }

        private void Start()
        {
            _textSettings = _settingsService.Settings.Text;
            _effectSettings = _textSettings.Effects;
            _twAudioPlayerPoolObj = _poolService.GetPool("pool_audio_player").GetNextObject();
            _twAudioPlayer = _twAudioPlayerPoolObj.GetComponent<AudioPlayer>();

            _tmp.richText = !_textSettings.DisableTMPTags;

            if (!string.IsNullOrEmpty(_effectSettings.TypewritingSoundId))
            {
                if (_effectSettings.TypewritingSoundId == "null") _twAudio = null;
                else
                {
                    _twAudio = _resourceService.GetResource(_effectSettings.TypewritingSoundId);
                    Type audioType = _twAudio.GetType();

                    if (audioType == typeof(AudioTrackResource)) _twAudioPlayer.SetAudio((AudioTrackResource)_twAudio);
                    else if (audioType == typeof(AudioGroupResource)) _twAudioPlayer.SetAudio((AudioGroupResource)_twAudio);
                    else throw new ArgumentException($"Resource with ID \"{_twAudio.ResourceID}\" is not an audio type.");
                }
            }

            SetText(_tmp.text);
        }

        private void Update()
        {
            if (_isTypewriting)
            {
                _time += Time.deltaTime;
                if (_time >= _typewritingDelay)
                {
                    _time -= _typewritingDelay;
                    if (_tmp.maxVisibleCharacters == _tmp.GetParsedText().Length) _isTypewriting = false;
                    else
                    {
                        if (_typewriterAttributes.Count > 0)
                        {
                            if (_typewriterAttributes.Peek().Item1 == _tmp.maxVisibleCharacters) SetTypewritingProperties();
                        }
                        if (_twAudio != null) _twAudioPlayer.Play(true, true);
                        _tmp.maxVisibleCharacters++;
                        _tmp.ForceMeshUpdate();
                    }
                }
            }
            foreach (GsTextPart part in _textParts) part.Update();
            TMP_TextInfo textInfo = _tmp.textInfo;
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                _tmp.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
        }

        private void ParseText(string text, bool hasTMPTags = false)
        {
            ParseText(text, new Dictionary<Type, Stack<Dictionary<string, string>>>(), hasTMPTags);
        }

        private void ParseText(string text, Dictionary<Type, Stack<Dictionary<string, string>>> effects, bool hasTMPTags = false)
        {
            bool tagOpen = false;
            int tagStartIndex = 0;
            string partText = "";
            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '\\':
                        i++;
                        if (i != text.Length - 1) partText += text[i + 1];
                        break;

                    case '<':
                        if (!tagOpen)
                        {
                            tagOpen = true;
                            tagStartIndex = i;
                        }
                        break;

                    case '>':
                        if (!tagOpen) break;
                        tagOpen = false;

                        int tagEndIndex = i;
                        string tagContents = text.Substring(tagStartIndex + 1, tagEndIndex - tagStartIndex - 1);
                        if (tagContents[0] == '/')
                        {
                            partText += $"<{tagContents}>";
                            break;
                        }
                        string[] tagContentsSplit = tagContents.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        string tagName = tagContentsSplit[0];
                        Type handlerType = null;

                        if (!_textSettings.DisableCustomTags)
                        {
                            handlerType = s_tagHandlers.GetValueOrDefault(tagName, null);
                        }
                        if (handlerType == null || !_textSettings.DisableDefaultGsTags)
                        {
                            handlerType = s_defaultTagHandlers.GetValueOrDefault(tagName, null);
                        }

                        List<string> tempList = new List<string>(tagContentsSplit);
                        tempList.RemoveAt(0);
                        string[] tagContentsWithoutName = tempList.ToArray();

                        if (handlerType != null)
                        {
                            int tagClosure = tagEndIndex + FindTagClosure(text.Substring(tagEndIndex + 1), tagName) + 1;
                            if (tagClosure != -1)
                            {
                                if (!string.IsNullOrEmpty(partText))
                                {
                                    if (hasTMPTags) _gsSanitizedText += partText;
                                    else _textParts.Add(CreateTextPart(partText, effects));

                                    partText = "";
                                }

                                PrepareEffect(handlerType, tagContentsWithoutName, effects);
                                ParseText(text.Substring(tagEndIndex + 1, tagClosure - tagEndIndex - 1), effects, hasTMPTags);
                                i = tagClosure + tagName.Length + 1;
                            }
                            else partText += $"<{tagContents}>";
                        }
                        else partText += $"<{tagContents}>";
                        break;

                    default:
                        if (!tagOpen) partText += text[i];
                        break;
                }
            }
            if (!string.IsNullOrEmpty(partText))
            {
                if (hasTMPTags) _gsSanitizedText += partText;
                else _textParts.Add(CreateTextPart(partText, effects));

                partText = "";
            }
            List<Type> keys = new List<Type>(effects.Keys);
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[keys[i]].Count == 1) effects.Remove(keys[i]);
                else effects[keys[i]].Pop();
            }
        }

        private int FindTagClosure(string text, string tagName)
        {
            bool found = false;
            Regex tagBeginRegex = s_defaultTagRegexes[tagName];
            int tagSearchIndex = 0;
            int tagClosure = text.IndexOf($"</{tagName}>", tagSearchIndex, StringComparison.Ordinal);

            while (!found && (tagClosure > 0))
            {
                Match regexMatch = tagBeginRegex.Match(text, tagSearchIndex, tagClosure - tagSearchIndex);
                if (!regexMatch.Success)
                {
                    found = true;
                    break;
                }
                tagSearchIndex = regexMatch.Index + regexMatch.Length + 2;
                if (!found) tagClosure = text.IndexOf($"</{tagName}>", tagClosure + tagName.Length + 2, StringComparison.Ordinal);
            }
            return tagClosure;
        }

        private Dictionary<string, string> ParseAttributes(string[] tagContents, bool skipFirstEntry = true)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            for (int i = skipFirstEntry ? 1 : 0; i < tagContents.Length; i++)
            {
                int equalsIndex = tagContents[i].IndexOf('=');
                if (equalsIndex > 0)
                {
                    if (equalsIndex != tagContents[i].Length - 1)
                    {
                        string[] att = tagContents[i].Split('=');
                        attributes.Add(att[0], att[1]);
                    }
                    else
                    {
                        if (i != tagContents.Length - 1)
                        {
                            attributes.Add(tagContents[i].Remove(0), tagContents[i + 1]);
                            i++;
                        }
                    }
                }
                else
                {
                    if (i != tagContents.Length - 1)
                    {
                        int nextEqualsIndex = tagContents[i + 1].IndexOf('=');
                        if (nextEqualsIndex == 0)
                        {
                            if (tagContents[i + 1].Length == 0 && i != tagContents.Length - 2)
                            {
                                attributes.Add(tagContents[i], tagContents[i + 2]);
                                i += 2;
                            }
                            else
                            {
                                attributes.Add(tagContents[i], tagContents[i + 1].Remove(0));
                                i++;
                            }
                        }
                        else
                        {
                            attributes.Add(tagContents[i], tagContents[i]);
                        }
                    }
                    else
                    {
                        attributes.Add(tagContents[i], tagContents[i]);
                    }
                }
            }
            return attributes;
        }

        private void PrepareEffect(string[] tagContents, Dictionary<Type, Stack<Dictionary<string, string>>> effects)
        {
            Type handlerType = s_defaultTagHandlers[tagContents[0]];
            if (!effects.ContainsKey(handlerType)) effects.Add(handlerType, new Stack<Dictionary<string, string>>());
            effects[handlerType].Push(ParseAttributes(tagContents));
        }

        private void PrepareEffect(Type handlerType, string[] tagContents, Dictionary<Type, Stack<Dictionary<string, string>>> effects)
        {
            if (!effects.ContainsKey(handlerType)) effects.Add(handlerType, new Stack<Dictionary<string, string>>());
            effects[handlerType].Push(ParseAttributes(tagContents));
        }

        private GsTextPart CreateTextPart(string text, Dictionary<Type, Stack<Dictionary<string, string>>> effects)
        {
            List<IGsTextEffect> effectList = new List<IGsTextEffect>(effects.Count);
            foreach (var pair in effects)
            {
                IGsTextEffect effect = (IGsTextEffect)Activator.CreateInstance(pair.Key);
                effect.Setup(_tmp, pair.Value.Peek());
                effectList.Add(effect);
            }
            return new GsTextPart(text, effectList);
        }

        private void TypewritingChecks()
        {
            int totalLength = 0;
            Regex twRegex = new Regex("<(tw)( .*?)?>", RegexOptions.Compiled);
            for (int i = 0; i < _textParts.Count; i++)
            {
                int removedLength = 0;
                GsTextPart part = _textParts[i];
                MatchCollection twMatches = twRegex.Matches(part.Text);
                if (twMatches.Count > 0)
                {
                    for (int j = 0; j < twMatches.Count; j++)
                    {
                        Match match = twMatches[j];

                        Dictionary<string, string> attributes;
                        if (match.Groups.Count == 3) attributes = ParseAttributes(match.Groups[2].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries), false);
                        else attributes = new();

                        _tmp.text = _tmp.text.Remove(match.Index + totalLength, match.Length);
                        part.Text = part.Text.Remove(match.Index - removedLength, match.Length);
                        _typewriterAttributes.Enqueue((match.Index + totalLength, attributes));
                        removedLength += match.Length;
                        if (match.Index == 0)
                        {
                            part.SetTMPIndex(part.GetTMPIndex() - removedLength);
                        }
                        for (int k = i + 1; k < _textParts.Count; k++) _textParts[k].SetTMPIndex(_textParts[k].GetTMPIndex() - removedLength);
                        _isTypewriting = true;
                    }
                }
                totalLength += part.Text.Length;
            }
            if (_isTypewriting)
            {
                SetTypewritingProperties();
                _tmp.maxVisibleCharacters = 0;
            }
        }

        private void SetTypewritingProperties()
        {
            (int, Dictionary<string, string>) tuple = _typewriterAttributes.Dequeue();
            _typewritingDelay = float.Parse(tuple.Item2.GetValueOrDefault("delay", "0.1"));

            string id = tuple.Item2.GetValueOrDefault("sound", _twAudio.ResourceID);
            if (id.Equals("null")) _twAudio = null;
            else
            {
                if (!id.Equals(_twAudio.ResourceID))
                {
                    _twAudio = _resourceService.GetResource(id);
                    Type audioType = _twAudio.GetType();

                    if (audioType == typeof(AudioTrackResource)) _twAudioPlayer.SetAudio((AudioTrackResource)_twAudio);
                    else if (audioType == typeof(AudioGroupResource)) _twAudioPlayer.SetAudio((AudioGroupResource)_twAudio);
                    else throw new ArgumentException($"Resource with ID \"{_twAudio.ResourceID}\" is not an audio type.");
                }
            }
        }

        public string GetUnparsedText()
        { return _realText; }

        public string GetParsedText()
        {
            string ret = "";
            foreach (GsTextPart part in _textParts) ret += part.Text;
            return ret;
        }

        public void SetText(string text)
        {
            Reset();
            _realText = text;

            _tmp.ForceMeshUpdate();
            ParseText(_tmp.GetParsedText(), false);
            ParseText(_tmp.text, true);

            int currentIndex = 0;
            foreach (GsTextPart part in _textParts)
            {
                part.SetTMPIndex(currentIndex);
                currentIndex += part.Text.Length;
            }
            _tmp.text = _gsSanitizedText; ;
            TypewritingChecks();
            _tmp.ForceMeshUpdate();
        }

        private void Reset()
        {
            _textParts.Clear();
            _typewriterAttributes.Clear();
            _isTypewriting = false;
            _typewritingDelay = 0;
            if (!string.IsNullOrEmpty(_effectSettings.TypewritingSoundId))
            {
                if (_effectSettings.TypewritingSoundId == "null") _twAudio = null;
                else
                {
                    Type audioType = _twAudio.GetType();

                    if (audioType == typeof(AudioTrackResource)) _twAudioPlayer.SetAudio((AudioTrackResource)_twAudio);
                    else if (audioType == typeof(AudioGroupResource)) _twAudioPlayer.SetAudio((AudioGroupResource)_twAudio);
                    else throw new ArgumentException($"Resource with ID \"{_twAudio.ResourceID}\" is not an audio type.");
                }
                _twAudio = _resourceService.GetResource(_effectSettings.TypewritingSoundId);
            }
            else _twAudio = null;
            _time = 0;
        }

        private void OnDestroy()
        {
            _twAudioPlayerPoolObj.ReturnToPool();
            Reset();
        }
    }
}