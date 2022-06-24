using UnityEngine;
using UniRx;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using common;
using TMPro;
namespace CAM_6
{
    public class InputContent : MonoBehaviour, IPointerClickHandler
    {
        public GameObject support(int i)
        {
            return transform.GetChild(1).transform.GetChild(i).gameObject;
        }
        private GameObject _btn;
        private GameObject _tick;
        private GameObject _hint;
        private GameObject _line;
        public string content = "Yes";
        public int _count = 0;
        public int _countChar = 0;
        public InputField _input;
        public bool isTutorial = false;
        public ClipByTime[] clipByTime;
        private AudioSource _audio;
        private char[] _charWord;
        public string result = "";
        public Color[] colors;
        public TextMeshProUGUI _toolTip;
        public int Score = 0;
        private void Awake()
        {
            _input = GetComponent<InputField>();
            _audio = GetComponent<AudioSource>();
            _btn = support(2);
            _tick = support(3);
            _hint = support(1);
            _line = support(0);
            //_toolTip = transform.GetChild(transform.childCount-1).GetComponent<TextMeshProUGUI>();
        }
        public void clear()
        {
            _btn.SetActive(false);
            _hint.SetActive(false);
            _tick.SetActive(true);
            _line.SetActive(true);
            _tick.GetComponent<Button>().interactable = false;
            _toolTip.gameObject.SetActive(false);
        }
        public void OnChange()
        {
            if (isTutorial)
            {
                _charWord = content.ToCharArray();
                char[] fullchar = Input.inputString.ToCharArray();
                if (fullchar.Length > 0)
                {
                    string check = fullchar[fullchar.Length - 1].ToString();
                    string _currentWord="";
                    if (_countChar < _charWord.Length)
                    {
                        _currentWord = _charWord[_countChar].ToString();
                    }
                    if ((check == _currentWord.ToLower() || check == _currentWord.ToUpper()) && _countChar<_charWord.Length)
                    {
                        if (Input.GetKeyDown(KeyCode.Backspace))
                        {
                            LeanTween.dispatchEvent(TextEvent.SOUND, "keyfail");
                        }
                        else
                        {
                            result += check;
                            _input.textComponent.color = Color.white;
                            _countChar++;
                            LeanTween.dispatchEvent(TextEvent.SOUND, "keyyes");
                        }
                    }
                    else
                    {
                        LeanTween.dispatchEvent(TextEvent.SOUND, "keyfail");
                    }
                    
                } 
                else
                {
                    LeanTween.dispatchEvent(TextEvent.SOUND, "keyfail");
                }
                _input.text = result;

            }
        }
        private void Play(int N)
        {
            if (!_audio.isPlaying)
            {
                _audio.clip = clipByTime[N].clip;
                _audio.Play();
            }
        }
        public void EditDone()
        {
            _tick.GetComponent<Button>().interactable = true;
            
        }
        public void check()
        {
            _tick.GetComponent<Button>().interactable = false;
            if (_input.text.ToLower()==content.ToLower())
            {
                _input.textComponent.color = Color.green;
                _countChar = 0;
                _input.enabled = false;
                if(_count==0) Score += 3;
                if (_count == 1) Score += 2;
                if (_count == 2) Score += 1;
                LeanTween.alpha(_tick.GetComponent<RectTransform>(), 1f, 0.3f).setFrom(0f).setOnComplete(() =>
                {
                    Play(0);
                    LeanTween.dispatchEvent(TextEvent.ACTION, TextEvent.HIGHLINE);
                    LeanTween.delayedCall(clipByTime[0].time+0.5f, () => {
                        LeanTween.dispatchEvent(TextEvent.ACTION, TextEvent.BEGIN);
                    });
                });
                LeanTween.dispatchEvent(TextEvent.SOUND, "correct");
                _count = 0;
            } else {
                _btn.transform.GetChild(0).GetComponent<Text>().text = content;
                LeanTween.dispatchEvent(TextEvent.SOUND, "wrong");
                _input.textComponent.color = colors[0];
                _count++;
                if (_count == 1)
                {
                    LeanTween.dispatchEvent(TextEvent.ACTION,TextEvent.HIGHLINE);
                }
                if (_count==2)
                {
                    _hint.SetActive(true);
                    _btn.transform.GetChild(0).GetComponent<Text>().color = colors[0];
                    LeanTween.dispatchEvent(TextEvent.ACTION, TextEvent.HIGHLINEEND);
                    LeanTween.alpha(_hint.GetComponent<RectTransform>(), 1f, 0).setFrom(0f).setDelay(0.5f).setOnComplete(() => {
                        LeanTween.dispatchEvent(TextEvent.SOUND,"appear");
                        LeanTween.delayedCall(0.5f, () => {
                            if (isTutorial) Play(1);
                        });
                    });
                }
                if (_count == 3)
                {
                    _hint.SetActive(false);
                    _tick.SetActive(false);
                    _countChar = 0;
                    _input.enabled = false;
                    string word = _input.text;
                    _toolTip.gameObject.SetActive(true);
                    _toolTip.text = "<s>" + word + "</s>";
                    _toolTip.color = colors[0];
                    _input.text = "";
                    _btn.transform.GetChild(0).GetComponent<Text>().color = colors[1];
                    if (isTutorial)  _input.textComponent.gameObject.SetActive(false);
                    LeanTween.delayedCall(0.5f, () => {
                        LeanTween.dispatchEvent(TextEvent.ACTION, TextEvent.HIGHLINE);
                        Hint(true);
                        LeanTween.delayedCall(0.5f,() => {
                            Play(1);
                            LeanTween.delayedCall(clipByTime[1].time+2, () => {
                               LeanTween.dispatchEvent(TextEvent.ACTION,TextEvent.BEGIN);
                               Score += 1;
                            });
                        });
                    });
                }
            }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            LeanTween.dispatchEvent(TextEvent.SOUND, "tapTextFiled");
            _input.textComponent.color = Color.white;
            _btn.SetActive(false);
        }
        public void Hint(bool isAuto)
        {
            _btn.SetActive(!_btn.activeSelf?true:false);
            LeanTween.dispatchEvent(TextEvent.SOUND, isAuto?"appear": "tapTextFiled");
            Image img = _btn.GetComponent<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, isAuto ? 0 : 255f);
        }
        public void HideHint()
        {
            if(_btn.activeSelf) _btn.SetActive(false);
        }
    }
}
