using UnityEngine;

namespace GsKit
{
    namespace Spritete
    {
        [CreateAssetMenu(menuName = "GsKit/Test/Sprite", fileName = "gay")]
        public class TextureTest : Resources.AbstractResource
        {
            [SerializeField] private Texture2D tex;

            public Texture2D Texture => tex;
        }
    }
}