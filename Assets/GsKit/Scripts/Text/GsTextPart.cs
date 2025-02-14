using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GsKit.Text
{
    public class GsTextPart
    {
        private string _text = "";
        private IList<IGsTextEffect> _effects = new List<IGsTextEffect>();

        private int _tmpIndex;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                foreach (IGsTextEffect effect in _effects) effect.SetLength(_text.Length);
            }
        }

        public GsTextPart(string realText, IList<IGsTextEffect> effects)
        {
            _text = realText;
            _effects = effects;
            foreach (IGsTextEffect effect in _effects)
            {
                effect.SetLength(_text.Length);
            }
        }

        public void SetTMPIndex(int index)
        {
            _tmpIndex = index;
            foreach (IGsTextEffect effect in _effects) effect.SetTMPIndex(index);
        }

        public int GetTMPIndex()
        { return _tmpIndex; }

        public void Update()
        {
            foreach (IGsTextEffect effect in _effects) effect.Update();
        }
    }
}