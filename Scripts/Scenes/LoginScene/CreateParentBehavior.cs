using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateParentBehavior : MonoBehaviour
{
    [Header("Inputs settings")]
    public InputField m_PhoneInput;
    public InputField m_EmailInput;
    public InputField m_PasswordInput;
    public GameObject m_PhoneErrorText;
    public GameObject m_EmailErrorText;
    public GameObject m_PasswordErrorText;

    [Header("Popup Settings")]
    public GameObject m_SignUpSuccessPopup;
    public GameObject m_EmailExistedPopup;
    public GameObject m_PhoneExistedPopup;

    [Header("Password Area")]
    public Button m_ButtonPass;
    public Sprite m_PassInvisible;
    public Sprite m_PassVisible;

    //internal 
    ParentSignUpInfo parentInfo;

    private void OnEnable()
    {
        m_PhoneErrorText.SetActive(false);
        m_EmailErrorText.SetActive(false);
        m_PasswordErrorText.SetActive(false);

        m_SignUpSuccessPopup.SetActive(false);
        m_EmailExistedPopup.SetActive(false);
        m_PhoneExistedPopup.SetActive(false);

        m_ButtonPass.image.sprite = m_PassInvisible;
    }

    public void OnPhoneNumberValidation()
    {
        if (m_PhoneInput.text.Length < 9)
        {
            m_PhoneErrorText.SetActive(true);
            if (m_PhoneInput.text.Length <= 0)
            {
                m_PhoneErrorText.GetComponent<Text>().text = "Bạn chưa nhập số điện thoại";
            }
            else
            {
                m_PhoneErrorText.GetComponent<Text>().text = "Số điện thoại không hợp lệ";
            }
        }
        else
            m_PhoneErrorText.SetActive(false);
    }

    public void OnEmailValidation()
    {
        if (m_EmailInput.text.Length <= 0)
        {
            m_EmailErrorText.SetActive(true);
            if (m_EmailInput.text.Length <= 0)
            {
                m_EmailErrorText.GetComponent<Text>().text = "Bạn chưa nhập địa chỉ email";
            }
        }
        else
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(m_EmailInput.text);
                if (address.Address == m_EmailInput.text)
                {
                    m_EmailErrorText.SetActive(false);
                }
                else
                {
                    m_EmailErrorText.GetComponent<Text>().text = "Email không đúng định dạng";
                    m_EmailErrorText.SetActive(true);
                }
            }
            catch
            {
                m_EmailErrorText.GetComponent<Text>().text = "Email không đúng định dạng";
                m_EmailErrorText.SetActive(true);
            }
        }

    }

    public void OnPasswordValidation()
    {
        if (m_PasswordInput.text.Length < 6)
        {
            m_PasswordErrorText.SetActive(true);
            if (m_PasswordInput.text.Length <= 0)
            {
                m_PasswordErrorText.GetComponent<Text>().text = "Bạn chưa nhập mật khẩu";
            }
            else
            {
                m_PasswordErrorText.GetComponent<Text>().text = "Mật khẩu cần tối thiểu 6 ký tự";
            }    
        }
        else
            m_PasswordErrorText.SetActive(false);
    }



    public void OnParentInfoConfirmed()
    {
        OnPhoneNumberValidation();
        OnEmailValidation();
        OnPasswordValidation();

        if (m_PhoneErrorText.activeSelf == true)
            return;

        if (m_EmailErrorText.activeSelf == true)
            return;

        if (m_PasswordErrorText.activeSelf == true)
            return;

        parentInfo = new ParentSignUpInfo(m_EmailInput.text,
                                          m_PhoneInput.text,
                                          m_PasswordInput.text,
                                          LocalizationManagerBehavior.GetInstance().m_CurrentLanguageCode);

        UserDataManagerBehavior.GetInstance().OnParentSignUp(parentInfo, null,
            reply =>
            {
                //Debug.Log(reply);
                //TODO: show error if any, else -> success
                if (reply != null)
                {
                    if (reply.statusCode.Contains(DataBaseInterface.EXISTED_EMAIL) == true)
                    {
                        m_EmailExistedPopup.SetActive(true);
                    }
                    else if (reply.statusCode.Contains(DataBaseInterface.EXISTED_PHONE) == true)
                    {
                        m_PhoneExistedPopup.SetActive(true);
                    }
                    else if (reply.statusCode.Contains(DataBaseInterface.SIGN_UP_SUCCESSFUL) == true)
                    {
                        m_SignUpSuccessPopup.SetActive(true);
                        UserDataManagerBehavior.GetInstance().currentParentID = m_EmailInput.text;
                        UserDataManagerBehavior.GetInstance().currentParentEmail = m_EmailInput.text;
                        UserDataManagerBehavior.GetInstance().currentParentPassword = m_PasswordInput.text;
                    }
                }
                else
                    PopupManagerBehavior.GetInstance().TriggerServerConnectionPopUp();
            }
            );
    }

    public void OnShowPassword()
    {
        if (m_PasswordInput.contentType == InputField.ContentType.Password)
        {
            m_ButtonPass.image.sprite = m_PassVisible;
            m_PasswordInput.contentType = InputField.ContentType.Standard;
            m_PasswordInput.ForceLabelUpdate();
        }
        else
        {
            m_ButtonPass.image.sprite = m_PassInvisible;
            m_PasswordInput.contentType = InputField.ContentType.Password;
            m_PasswordInput.ForceLabelUpdate();
        }
    }
}

