using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SixDigitCodeDisplayBehavior : MonoBehaviour
{
    public InputField m_CodeInputField;
    public List<Text> m_DisplayText = new List<Text>();

    private void OnEnable()
    {
        m_CodeInputField.text = "";
        m_CodeInputField.placeholder.GetComponent<Text>().text = "";
    }

    public void OnValueChanged()
    {
        foreach (Text text in m_DisplayText)
        {
            text.text = "";
        }

        for (int i = 0; i < m_CodeInputField.text.Length && i < m_DisplayText.Count; i++)
        {
            m_DisplayText[i].text = m_CodeInputField.text[i].ToString();
        }    
    }    
}
