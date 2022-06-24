using UnityEngine;
using UnityEngine.UI;
namespace common
{
    public class FixRatio : MonoBehaviour
    {
        public float[] matchWH = new float[2] { 0.5f, 0 };
        public float ratio = 0;
        public void Start()
        {
            ratio = (float)Screen.width / (float)Screen.height;
            CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
            canvasScaler.matchWidthOrHeight = matchWH[ratio > 1.61f ? 0 : 1];
        }
    }
}
