using UnityEngine;
using UniRx;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using common;
using TMPro;
namespace CAM_9
{
    public class Answer : MonoBehaviour
    {
        public int sttInPage = 0;
        private Image _line;
        public ClipByTime[] clipByTime;
        private AudioSource _audio;
        public bool isTutorial = false;
        private char[] _charWord;
        public string content = "Yes";
        public int _countChar = 0;
        public string result = "";
        public Color[] colors;
        public InputField _input;
        public TextMeshProUGUI _toolTip;
        public Image hint;
        public Color _color;
        void Awake()
        {
            _line = transform.GetChild(0).GetComponent<Image>();
            gameObject.AddComponent<AudioSource>();
            _audio = GetComponent<AudioSource>();
            _audio.playOnAwake = false;
            _input = transform.GetChild(2).GetComponent<InputField>();
            _color = _input.placeholder.GetComponent<Text>().color;
        }
        private void OnEnable()
        {
            Reset();
        }
        public void Reset()
        {
            _input.text = "";
            _input.textComponent.text = "";
            _input.enabled = true;
            _input.textComponent.color = Color.black;
            _input.placeholder.GetComponent<Text>().color = _color;
            hint.gameObject.SetActive(false);
            //content = "";
            //result = "";
            _countChar = 0;
            _line.enabled = true;
            _line.color = Color.black;
            _line.transform.GetChild(1).gameObject.SetActive(false);
            _line.transform.GetChild(2).gameObject.SetActive(false);
            _line.transform.GetChild(0).gameObject.SetActive(false);
            isTutorial = false;
        }
        public void clear()
        {
            content = "";
            result = "";
        }
        private void Play(int N)
        {
            if (!_audio.isPlaying)
            {
                _audio.clip = clipByTime[N].clip;
                _audio.Play();
            }
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
                    string _currentWord = "";
                    if (_countChar < _charWord.Length)
                    {
                        _currentWord = _charWord[_countChar].ToString();
                    }
                    if ((check == _currentWord.ToLower() || check == _currentWord.ToUpper()) && _countChar < _charWord.Length)
                    {
                        if (Input.GetKeyDown(KeyCode.Backspace))
                        {
                            LeanTween.dispatchEvent(Cam9Event.SOUND, "keyfail");
                        }
                        else
                        {
                            result += check;
                            _countChar++;
                            LeanTween.dispatchEvent(Cam9Event.SOUND, "keyyes");
                        }
                    }
                    else
                    {
                        LeanTween.dispatchEvent(Cam9Event.SOUND, "keyfail");
                    }

                }
                else
                {
                    LeanTween.dispatchEvent(Cam9Event.SOUND, "keyfail");
                }
                _input.text = result;
            }
        }
        public void OnEdit()
        {
            LeanTween.dispatchEvent(Cam9Event.INPUT,transform);
        }
        public void Yes()
        {
            _line.enabled = false;
            _line.transform.GetChild(1).gameObject.SetActive(true);
            _line.transform.GetChild(2).gameObject.SetActive(false);
            _line.transform.GetChild(0).gameObject.SetActive(false);
            LeanTween.dispatchEvent(Cam9Event.SOUND, "correct");
            _input.enabled = false;
            _input.textComponent.color = Color.green;
            LeanTween.delayedCall(1, () => Play(0));
        }
        public void No()
        {
            _line.enabled = false;
            _line.transform.GetChild(1).gameObject.SetActive(false);
            _line.transform.GetChild(2).gameObject.SetActive(true);
            _line.transform.GetChild(0).gameObject.SetActive(false);
            LeanTween.dispatchEvent(Cam9Event.SOUND, "wrong");
            _input.textComponent.color = Color.red;
            
        }
        public void Glow()
        {
            _line.enabled = true;
            _line.transform.GetChild(1).gameObject.SetActive(false);
            _line.transform.GetChild(2).gameObject.SetActive(false);
            _line.transform.GetChild(0).gameObject.SetActive(true);
            LeanTween.delayedCall(3, () => {
                _line.transform.GetChild(0).gameObject.SetActive(false);
            });
        }
        public void Hint()
        {
            Debug.Log("hint");
            Debug.Log("content:" + content);
            hint.transform.GetChild(0).GetComponent<Text>().text = content;
            hint.gameObject.SetActive(true);
            hint.color = new Color(hint.color.r, hint.color.g, hint.color.b, 255);
        }
        public void  closeHint()
        {
            hint.gameObject.SetActive(false);
        }
        public void toolTip()
        {
            string word = _input.text;
            _toolTip.gameObject.SetActive(true);
            _toolTip.text = "<s>" + word + "</s>";
            _toolTip.color = colors[0];
            _input.text = "";
            hint.color = new Color(hint.color.r, hint.color.g, hint.color.b,0);
            _input.enabled = false;
            LeanTween.delayedCall(0.5f,()=>Play(1));
        }
        public void MaxFail()
        {
            _input.text = content;
            _input.enabled = false;
            _toolTip.color = Color.green;
            closeHint();
            LeanTween.delayedCall(0.5f, () => Play(1));
        }
    }
}
