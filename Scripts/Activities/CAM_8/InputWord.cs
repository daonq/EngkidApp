using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace common
{
    public class InputWord : MonoBehaviour, IPointerClickHandler
    {
        public InputField _inputField;
        private int _count;
        public bool isTutorial = false;
        private string result = "";
        private char[] _charWord;
        public string id = "family";
        public bool isHelpFinish = false;
        public GameObject Mypic;
        public bool isNext = false;
        public Image line;
        public int MAX=8;
        void Awake()
        {
            _inputField = GetComponent<InputField>();
            if (isTutorial) _charWord = id.ToCharArray();
        }
        void Start()
        {
            _count = 0;
        }
        public void OnEdit()
        {
            LeanTween.dispatchEvent(TextEvent.SOUND,"tap");
            LeanTween.dispatchEvent(TextEvent.DONEINPUT,transform);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_inputField.enabled)
            {
                _inputField.placeholder.gameObject.SetActive(false);
                LeanTween.dispatchEvent(TextEvent.BEGININPUT, transform);
                if (!isTutorial && Input.inputString.Length >= MAX)
                {
                    _inputField.text = Input.inputString.ToLower().Substring(MAX);
                }
                if(!isTutorial) _inputField.textComponent.color = Color.black;
            }
        }
        public void highLine(bool isHighLine)
        {
            line.transform.GetChild(0).gameObject.SetActive(isHighLine);
        }
        public void onChange()
        {
            if (isTutorial)
            {
                char[] fullchar = Input.inputString.ToCharArray();
                if (fullchar.Length>0)
                {
                    string check = fullchar[fullchar.Length - 1].ToString();
                    string _currentWord = _charWord[_count].ToString();
                    if (check == _currentWord.ToLower() || check == _currentWord.ToUpper())
                    {
                        result += check;
                        _count++;
                        LeanTween.dispatchEvent(TextEvent.SOUND, "keyyes");
                    }
                    else
                    {
                        LeanTween.dispatchEvent(TextEvent.SOUND, "keyfail");
                    }
                    _inputField.text = result;
                }
            }
            else
            {
                if (Input.inputString.Length >= MAX)
                {
                   // _inputField.text = Input.inputString.ToLower().Substring(-1*MAX);
                }
            }
        }
        
    }
}
