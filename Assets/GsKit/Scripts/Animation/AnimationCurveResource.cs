using UnityEngine;

namespace GsKit.Animation
{
    [CreateAssetMenu(fileName = "Animation Curve", menuName = "GsKit/Animation/Animation Curve")]
    public class AnimationCurveResource : Resources.AbstractResource
    {
        [Tooltip("The animation curve.")]
        [SerializeField]
        private AnimationCurve m_animationCurve;

        public AnimationCurve Curve => m_animationCurve;
    }
}