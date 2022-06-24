using common;
using UnityEngine;
using UnityEngine.UI;

namespace V2
{
    public class SpaceShip : MonoBehaviour
    {
        private Vector3 _oldPos;
        private AudioSource _audio;
        public Transform[] Points;
        private void Start()
        {
            _oldPos = transform.localPosition;
            _audio = GetComponent<AudioSource>();
        }
        public void goIn()
        {
            transform.localPosition = _oldPos;
            LeanTween.moveLocal(gameObject,Points[0].localPosition, 5).setOnComplete(() => {
                LeanTween.dispatchEvent(V2Event.SOUND,"bell");
                LeanTween.delayedCall(2, () => {
                    LeanTween.dispatchEvent(V2Event.ACTION, V2Event.TRASH); 
                    LeanTween.delayedCall(5, () => goOut());
                });
            });
            _audio.Play();
        }
        public void goOut()
        {
            LeanTween.moveLocal(gameObject,Points[1].localPosition, 3);
            _audio.Play();
        }
    }
}
