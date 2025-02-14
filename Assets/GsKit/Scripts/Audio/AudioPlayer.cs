using System;
using System.Collections.Generic;
using UnityEngine;
using GsKit.Resources;
using GsKit.Extensions;

namespace GsKit.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;
        private AbstractResource _audioResource;
        private int _currentTrackIndex = 0;
        private bool _isPaused = false;
        private bool _isGroup = false;
        private System.Random random = new();

        public bool isPaused => _isPaused;
        public bool isPlaying => _audioSource.isPlaying;
        public bool isStopped => !_audioSource.isPlaying && !_isPaused;
        public AudioSource Source => _audioSource;

        public int CurrentTrackIndex
        {
            get => _currentTrackIndex;
            set => _currentTrackIndex = value;
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play(bool stopIfPlaying, bool next = false)
        {
            if (_audioSource.isPlaying)
            {
                if (!stopIfPlaying) throw new InvalidOperationException($"An audio is already playing.");
                _audioSource.Stop();
            }
            if (_isPaused)
            {
                _audioSource.UnPause();
                _isPaused = false;
            }
            else
            {
                UpdateClip(next);
                _audioSource.Play();
                _isPaused = false;
            }
        }

        public void Pause()
        {
            if (!_audioSource.isPlaying) throw new InvalidOperationException("No audio is being played.");
            _audioSource.Pause();
            _isPaused = true;
        }

        public void Unpause()
        {
            if (!_isPaused) throw new InvalidOperationException("Audio is not currently paused.");
            _audioSource.UnPause();
            _isPaused = false;
        }

        public void Stop()
        {
            if (_audioSource.isPlaying || _isPaused)
            {
                _audioSource.Stop();
                _isPaused = false;
            }
        }

        private void UpdateClip(bool next = false)
        {
            if (_isGroup)
            {
                AudioGroupResource group = (AudioGroupResource)_audioResource;
                if (next)
                {
                    switch (group.PlayOrder)
                    {
                        case AudioPlayOrder.IN_ORDER:
                            _currentTrackIndex = (_currentTrackIndex + 1) % group.Clips.Count;
                            break;

                        case AudioPlayOrder.RANDOM:
                            _currentTrackIndex = random.Next(0, group.Clips.Count);
                            break;

                        case AudioPlayOrder.REVERSE:
                            _currentTrackIndex = _currentTrackIndex - 1 - group.Clips.Count * (int)Mathf.Floor(_currentTrackIndex / group.Clips.Count);
                            break;
                    }
                }
                _audioSource.Stop();
                _audioSource.clip = group.Clips[_currentTrackIndex];
            }
            else
            {
                _audioSource.Stop();
                _audioSource.clip = ((AudioTrackResource)_audioResource).Clip;
            }
        }

        public void SetAudio(AudioTrackResource audio)
        {
            _audioResource = audio;
            _isGroup = false;
        }

        public void SetAudio(AudioGroupResource audio)
        {
            _audioResource = audio;
            _currentTrackIndex = 0;
            _isGroup = true;
        }
    }
}