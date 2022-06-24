using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class SignInBehavior : MonoBehaviour
{
    [Header("Inputs")]
    public InputField m_IDInput;
    public InputField m_Passwordinput;

    [Header("Log In Errors and Popups")]
    public Text m_IDErrorText;
    public Text m_PasswordErrorText;
    public Button m_LogInButton;

    [Header("Popups Settings")]
    public GameObject m_SignInFailedPopUp;
    public GameObject m_InactiveAccountPopUp;
    public GameObject m_WrongPasswordCodePopUp;
    public GameObject m_RequestedPasswordCodePopUp;
    public GameObject m_ChangePasswordSuccessfullyPopUp;
    public GameObject m_EmailNotExistedPopUp;

    [Header("Forget password")]
    public GameObject m_RequestPasswordResetCodeWindow;
    public InputField m_RegisteredEmail;
    public GameObject m_ConfirmingPasswordResetCodeWindow;
    public InputField m_SixDigitsCode;
    public GameObject m_ChoosenewpasswordWindow;
    public GameObject m_NewPasswordNotMatchErrorText;
    public GameObject m_NewPasswordValidationErrorText;
    public InputField m_NewPasswordInput;
    public InputField m_NewPasswordRepeatInput;
    public Button m_SetNewPasswordConfirmButton;

    [Header("Parent Activation")]
    public GameObject m_ActivationWindow;

    [Header("Password Area")]
    public Button m_ButtonPass;
    public Sprite m_PassInvisible;
    public Sprite m_PassVisible;
    public Text m_EmailForgotPasswordErr;

    private void Awake()
    {
        if (Debug.isDebugBuild) Debug.Log("SignInBehavior Loaded");
    }

    [Header("New Password Area")]
    public Button m_ShowHidePasswordNew;

    private void Start()
    {
        m_IDErrorText.text = "";
        m_PasswordErrorText.text = "";

        m_RequestPasswordResetCodeWindow.SetActive(false);
        m_ConfirmingPasswordResetCodeWindow.SetActive(false);
        m_ChoosenewpasswordWindow.SetActive(false);
        m_ActivationWindow.SetActive(false);

        m_EmailForgotPasswordErr.enabled = false;

        m_NewPasswordInput.text = "";

        if (PlayerPrefs.HasKey("USER_LAST_LOGGED_IN_ID") == true)
        {
            m_IDInput.text = PlayerPrefs.GetString("USER_LAST_LOGGED_IN_ID");
        }
    }

    private void OnEnable()
    {
        m_IDErrorText.text = "";
        m_PasswordErrorText.text = "";

        m_RequestPasswordResetCodeWindow.SetActive(false);
        m_ConfirmingPasswordResetCodeWindow.SetActive(false);
        m_ChoosenewpasswordWindow.SetActive(false);
        m_ActivationWindow.SetActive(false);

        m_ButtonPass.image.sprite = m_PassInvisible;

        m_NewPasswordInput.text = "";

        if (PlayerPrefs.HasKey("USER_LAST_LOGGED_IN_ID") == true)
            m_IDInput.text = PlayerPrefs.GetString("USER_LAST_LOGGED_IN_ID");
    }

    public void OnRequestActivationCode()
    {
        UserDataManagerBehavior.GetInstance().OnParentLogIn(UserDataManagerBehavior.GetInstance().currentParentID,
                                                            UserDataManagerBehavior.GetInstance().currentParentPassword,
                                                            null,
                                                            log_in_reply =>
                                                            {
                                                                //check if verified
                                                                if (log_in_reply.statusCode.Contains(DataBaseInterface.EMAIL_UNVERIFIED_CODE) == true)
                                                                {
                                                                    m_ActivationWindow.SetActive(true);
                                                                }
                                                                else if (log_in_reply.statusCode.Contains(DataBaseInterface.LOG_IN_SUCCESSFUL) == true)
                                                                {
                                                                    UserDataManagerBehavior.GetInstance().SetUserLogInState(EngKidAPI.UserLogInStates.LOGGED_IN);

                                                                    UserDataManagerBehavior.GetInstance().currentParentSessionIDToken = log_in_reply.data.token;
                                                                    //Debug.Log(UserDataManagerBehavior.GetInstance().currentParentSessionIDToken);
                                                                    CustomEventManager.GetInstance().ParentLoggedIn();
                                                                }
                                                            }
            );
    }

    public void OnLogInClicked()
    {
        if (CheckEmailValidation() == false || CheckPasswordValidation() == false)
        {
            return;
        }
        else
        {
            UserDataManagerBehavior.GetInstance().currentParentID = m_IDInput.text;
            UserDataManagerBehavior.GetInstance().currentParentPassword = m_Passwordinput.text;

            UserDataManagerBehavior.GetInstance().OnParentLogIn(m_IDInput.text, m_Passwordinput.text, log_in_flag =>
            {
                m_LogInButton.interactable = true;

                if (log_in_flag == false)
                    m_SignInFailedPopUp.SetActive(true);
            },
            log_in_reply =>
            {
                if (log_in_reply != null)
                {
                    //check if verified
                    if (log_in_reply.statusCode.Contains(DataBaseInterface.EMAIL_UNVERIFIED_CODE) == true)
                    {
                        //m_ActivationWindow.SetActive(true);
                        m_InactiveAccountPopUp.SetActive(true);
                    }
                    else if (log_in_reply.statusCode.Contains(DataBaseInterface.LOG_IN_SUCCESSFUL) == true)
                    {
                        UserDataManagerBehavior.GetInstance().SetUserLogInState(EngKidAPI.UserLogInStates.LOGGED_IN);

                        UserDataManagerBehavior.GetInstance().currentParentSessionIDToken = log_in_reply.data.token;

                        PlayerPrefs.SetString("USER_TOKEN", log_in_reply.data.token);
                        PlayerPrefs.SetString("USER_LAST_LOGGED_IN_ID", m_IDInput.text);
                        PlayerPrefs.Save();//TODO NOBITA

                        //Debug.Log(UserDataManagerBehavior.GetInstance().currentParentSessionIDToken);
                        CustomEventManager.GetInstance().ParentLoggedIn();
                        //Debug.Log("Login!");
                        //SceneManagerBehavior.GetInstance().OpenActivationWhenLoginScreen(null);
                    }
                    //Debug.Log("Login reply != null");
                    //Debug.Log("Status code: " + log_in_reply.statusCode);
                }
            }
            );
        }
    }

    public bool IsValidEmailAddress(string s)
    {
        var regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
        return regex.IsMatch(s);
    }

    public void OnForgetPasswordClicked()
    {
        if (IsValidEmailAddress(m_IDInput.text) == true)
            m_RegisteredEmail.text = m_IDInput.text;
        else
            m_RegisteredEmail.text = "";

        m_RequestPasswordResetCodeWindow.SetActive(true);
    }

    public void OnRequestPasswordResetCode()
    {
        if (CheckEmailValidationForgot() == false)
        {
            return;
        }
        //TODO: request server for a 6 digits code to reset the password
        UserDataManagerBehavior.GetInstance().OnForgetPasswordClicked(m_RegisteredEmail.text,
            callback_flag =>
            {
                if (callback_flag == true)
                {
                    m_ConfirmingPasswordResetCodeWindow.SetActive(true);
                    m_RequestedPasswordCodePopUp.SetActive(true);
                }
                else
                {
                    m_EmailNotExistedPopUp.SetActive(true);
                }
            },
            ret_message =>
            {

            }
            );
    }

    public void OnPasswordResetCodeConfirmed()
    {
        UserDataManagerBehavior.GetInstance().OnSixDigitsCodeConfirmed(m_SixDigitsCode.text, callback_flag =>
        {
            if (callback_flag == false)
            {
                m_WrongPasswordCodePopUp.SetActive(true);
            }
            else
            {
                m_ChoosenewpasswordWindow.SetActive(true);
                m_SetNewPasswordConfirmButton.interactable = false;
            }
        });
    }

    public void OnNewPasswordValueChanged()
    {
        if (m_NewPasswordInput.text.Length < 6)
        {
            m_NewPasswordValidationErrorText.SetActive(true);
            m_SetNewPasswordConfirmButton.interactable = false;
        }
        else
        {
            m_NewPasswordValidationErrorText.SetActive(false);
            m_SetNewPasswordConfirmButton.interactable = true;
        }
    }

    public void OnNewPasswordConfirmed()
    {
        if (CheckNewPasswordValidation() == false)
        {
            return;
        }
        else
        {
            UserDataManagerBehavior.GetInstance().OnSetNewPassword(m_NewPasswordInput.text, callback_flag =>
            {
                if (callback_flag == false)
                {
                    m_SignInFailedPopUp.SetActive(true);
                }
                else
                {
                    m_ChangePasswordSuccessfullyPopUp.SetActive(true);
                    m_SetNewPasswordConfirmButton.interactable = false;
                }
            });
        }
    }

    public void OnShowPassword()
    {
        if (m_Passwordinput.contentType == InputField.ContentType.Password)
        {
            m_ButtonPass.image.sprite = m_PassVisible;
            m_Passwordinput.contentType = InputField.ContentType.Standard;
            m_Passwordinput.ForceLabelUpdate();
        }
        else
        {
            m_ButtonPass.image.sprite = m_PassInvisible;
            m_Passwordinput.contentType = InputField.ContentType.Password;
            m_Passwordinput.ForceLabelUpdate();
        }
    }

    public void OnShowPassword(InputField input)
    {
        if (input.contentType == InputField.ContentType.Password)
        {
            input.contentType = InputField.ContentType.Standard;
            input.ForceLabelUpdate();
        }
        else
        {
            input.contentType = InputField.ContentType.Password;
            input.ForceLabelUpdate();
        }
    }

    public void OnEmailValidation()
    {
        if (m_IDInput.text == "")
        {
            m_IDErrorText.enabled = true;
            m_IDErrorText.text = "Bạn cần nhập email hoặc số điện thoại.";
        }
        else
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(m_IDInput.text);
                if (address.Address == m_IDInput.text)
                {
                    m_IDErrorText.enabled = false;
                }
                else
                {
                    bool isNumberic = true;
                    for (int i = 0; i < m_IDInput.text.Length; i++)
                    {
                        if (!char.IsDigit(m_IDInput.text[i]))
                        {
                            isNumberic = false;
                            break;
                        }
                    }
                    if (isNumberic)
                    {
                        if (m_IDInput.text.Length < 9)
                        {
                            m_IDErrorText.text = "Số điện thoại bạn nhập có định dạng chưa hợp lệ.";
                            m_IDErrorText.enabled = true;
                        }
                        else
                        {
                            m_IDErrorText.enabled = false;
                        }
                    }
                    else
                    {
                        m_IDErrorText.text = "Email bạn nhập có định dạng không hợp lệ.";
                        m_IDErrorText.enabled = true;
                    }
                }
            }
            catch
            {
                bool isNumberic = true;
                for (int i = 0; i < m_IDInput.text.Length; i++)
                {
                    if (!char.IsDigit(m_IDInput.text[i]))
                    {
                        isNumberic = false;
                        break;
                    }
                }
                if (isNumberic)
                {
                    if (m_IDInput.text.Length < 9)
                    {
                        m_IDErrorText.text = "Số điện thoại bạn nhập có định dạng chưa hợp lệ.";
                        m_IDErrorText.enabled = true;
                    }
                    else
                    {
                        m_IDErrorText.enabled = false;
                    }
                }
                else
                {
                    m_IDErrorText.text = "Email bạn nhập có định dạng không hợp lệ.";
                    m_IDErrorText.enabled = true;
                }

            }
        }
    }

    public void OnPasswordValidation()
    {
        if (m_Passwordinput.text.Length < 6)
        {
            m_PasswordErrorText.enabled = true;
            if (m_Passwordinput.text.Length <= 0)
            {
                m_PasswordErrorText.text = "Bạn cần nhập mật khẩu.";
            }
            else
            {
                m_PasswordErrorText.text = "Mật khẩu cần có tối thiểu 6 ký tự";
            }
        }
        else
        {
            m_PasswordErrorText.enabled = false;
        }
    }

    public void OnEmailValidationForgot(Text textError)
    {
        if (m_RegisteredEmail.text.Length <= 0)
        {
            textError.enabled = true;
            textError.text = "Bạn cần nhập địa chỉ email";
        }
        else
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(m_RegisteredEmail.text);
                if (address.Address == m_RegisteredEmail.text)
                {
                    textError.enabled = false;
                }
                else
                {
                    textError.text = "Email bạn nhập có định dạng chưa hợp lệ";
                    textError.enabled = true;
                }
            }
            catch
            {
                textError.text = "Email bạn nhập có định dạng chưa hợp lệ";
                textError.enabled = true;
            }
        }
    }

    public bool CheckEmailValidationForgot()
    {
        if (m_RegisteredEmail.text.Length <= 0)
        {
            m_EmailForgotPasswordErr.enabled = true;
            m_EmailForgotPasswordErr.text = "Bạn cần nhập địa chỉ email";
            return false;
        }
        else
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(m_RegisteredEmail.text);
                if (address.Address == m_RegisteredEmail.text)
                {
                    m_EmailForgotPasswordErr.enabled = false;
                    return true;
                }
                else
                {
                    m_EmailForgotPasswordErr.text = "Email bạn nhập có định dạng chưa hợp lệ";
                    m_EmailForgotPasswordErr.enabled = true;
                    return false;
                }
            }
            catch
            {
                m_EmailForgotPasswordErr.text = "Email bạn nhập có định dạng chưa hợp lệ";
                m_EmailForgotPasswordErr.enabled = true;
                return false;
            }
        }
    }

    public void OnActiveCode()
    {
        m_ActivationWindow.SetActive(true);
    }

    public bool CheckEmailValidation()
    {
        if (m_IDInput.text == "")
        {
            m_IDErrorText.enabled = true;
            m_IDErrorText.text = "Bạn cần nhập email hoặc số điện thoại.";
            return false;
        }
        else
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(m_IDInput.text);
                if (address.Address == m_IDInput.text)
                {
                    m_IDErrorText.enabled = false;
                }
                else
                {
                    bool isNumberic = true;
                    for (int i = 0; i < m_IDInput.text.Length; i++)
                    {
                        if (!char.IsDigit(m_IDInput.text[i]))
                        {
                            isNumberic = false;
                            break;
                        }
                    }
                    if (isNumberic)
                    {
                        if (m_IDInput.text.Length < 9)
                        {
                            m_IDErrorText.text = "Số điện thoại bạn nhập có định dạng chưa hợp lệ.";
                            m_IDErrorText.enabled = true;
                            return false;
                        }
                        else
                        {
                            m_IDErrorText.enabled = false;
                            return false;
                        }
                    }
                    else
                    {
                        m_IDErrorText.text = "Email bạn nhập có định dạng không hợp lệ.";
                        m_IDErrorText.enabled = true;
                        return false;
                    }
                }
            }
            catch
            {
                bool isNumberic = true;
                for (int i = 0; i < m_IDInput.text.Length; i++)
                {
                    if (!char.IsDigit(m_IDInput.text[i]))
                    {
                        isNumberic = false;
                        break;
                    }
                }
                if (isNumberic)
                {
                    if (m_IDInput.text.Length < 9)
                    {
                        m_IDErrorText.text = "Số điện thoại bạn nhập có định dạng chưa hợp lệ.";
                        m_IDErrorText.enabled = true;
                        return false;
                    }
                    else
                    {
                        m_IDErrorText.enabled = false;
                    }
                }
                else
                {
                    m_IDErrorText.text = "Email bạn nhập có định dạng không hợp lệ.";
                    m_IDErrorText.enabled = true;
                    return false;
                }

            }
        }
        return true;
    }

    public bool CheckPasswordValidation()
    {
        if (m_Passwordinput.text.Length < 6)
        {
            m_PasswordErrorText.enabled = true;
            if (m_Passwordinput.text.Length <= 0)
            {
                m_PasswordErrorText.text = "Bạn cần nhập mật khẩu.";
                return false;
            }
            else
            {
                m_PasswordErrorText.text = "Mật khẩu cần có tối thiểu 6 ký tự";
                return false;
            }
        }
        else
        {
            m_PasswordErrorText.enabled = false;
            return true;
        }
    }

    public void OnNewPasswordValidation()
    {
        if (m_NewPasswordInput.text.Length < 6)
        {
            m_NewPasswordNotMatchErrorText.SetActive(true);
            if (m_NewPasswordInput.text.Length <= 0)
            {
                m_NewPasswordNotMatchErrorText.GetComponent<Text>().text = "Bạn chưa nhập mật khẩu.";
            }
            else
            {
                m_NewPasswordNotMatchErrorText.GetComponent<Text>().text = "Mật khẩu cần có tối thiểu 6 ký tự";
            }
        }
        else
        {
            m_NewPasswordNotMatchErrorText.SetActive(false);
        }
    }

    public bool CheckNewPasswordValidation()
    {
        if (m_NewPasswordInput.text.Length < 6)
        {
            m_NewPasswordNotMatchErrorText.SetActive(true);
            if (m_NewPasswordInput.text.Length <= 0)
            {
                m_NewPasswordNotMatchErrorText.GetComponent<Text>().text = "Bạn chưa nhập mật khẩu.";
                return false;
            }
            else
            {
                m_NewPasswordNotMatchErrorText.GetComponent<Text>().text = "Mật khẩu cần có tối thiểu 6 ký tự";
                return false;
            }
        }
        else
        {
            m_NewPasswordNotMatchErrorText.SetActive(false);
            return true;
        }
    }

    public void OnShowNewPassword()
    {
        if (m_NewPasswordInput.contentType == InputField.ContentType.Password)
        {
            m_ShowHidePasswordNew.image.sprite = m_PassVisible;
            m_NewPasswordInput.contentType = InputField.ContentType.Standard;
            m_NewPasswordInput.ForceLabelUpdate();
        }
        else
        {
            m_ShowHidePasswordNew.image.sprite = m_PassInvisible;
            m_NewPasswordInput.contentType = InputField.ContentType.Password;
            m_NewPasswordInput.ForceLabelUpdate();
        }
    }

}
