using System.Collections.Generic;
using TMPro;

namespace GsKit.Text
{
    public interface IGsTextEffect
    {
        public void Setup(TMP_Text textMesh, Dictionary<string, string> attributes);

        public void SetTMPIndex(int index);

        public void SetLength(int length);

        public void Update();
    }
}