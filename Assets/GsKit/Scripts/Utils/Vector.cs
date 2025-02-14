using UnityEngine;

namespace GsKit.Utils
{
    public static class Vector
    {
        public enum XCorner
        {
            Left = -1,
            Right = 1
        }

        public enum YCorner
        {
            Up = 1,
            Down = -1
        }

        public enum ZCorner
        {
            Front = 1,
            Back = -1
        }

        public static Vector2 CenterToCorner(Vector2 vec,
                                            float width,
                                            float height,
                                            XCorner xCorner = XCorner.Left,
                                            YCorner yCorner = YCorner.Up)
        {
            vec.x += (width / 2) * ((int)xCorner);
            vec.y += (height / 2) * ((int)yCorner);

            return vec;
        }

        public static Vector3 CenterToCorner(Vector3 vec,
                                            float width,
                                            float height,
                                            float depth,
                                            XCorner xCorner = XCorner.Left,
                                            YCorner yCorner = YCorner.Up,
                                            ZCorner zCorner = ZCorner.Back)
        {
            vec.x += (width / 2) * ((int)xCorner);
            vec.y += (height / 2) * ((int)yCorner);
            vec.z += (depth / 2) * ((int)zCorner);

            return vec;
        }
    }
}