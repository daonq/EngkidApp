using UnityEngine;
namespace Stories
{
    public class ChangePosition : MonoBehaviour
    {
        public int XWith;
        private float six_nine;
        private float ratio;
        void Awake()
        {
            six_nine = Mathf.Ceil(100 * ((float)16 / (float)9));
            ratio = Mathf.Ceil(100 * ((float)Screen.width / (float)Screen.height));
        }
        void Start()
        {
            if (ratio == six_nine)
            {
                transform.localPosition = new Vector3(transform.localPosition.x + XWith, transform.localPosition.y, transform.localPosition.z);
            }
        }
    }
}
