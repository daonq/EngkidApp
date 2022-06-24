using UnityEngine;
namespace cam8
{
    public class Glow : MonoBehaviour
    {
        void Start()
        {
            LeanTween.alpha(GetComponent<RectTransform>(),1f, 0.3f).setFrom(0f).setLoopPingPong();
        }
    }
}
