public static class SendEventFirebase
{
    #region USER_PROPERTIES
    public static void SendUserProperties(string star, string exp, string diamond, string gender, string age, string sub)
    {
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("child_star", star);
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("child_experience", exp);
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("child_diamond", diamond);
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("child_gender", gender);
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("child_age", age);
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("child_subscription", sub);
    }

    public static void SetStar(string star)
    {
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("child_star", star);
    }

    public static void SetExp(string exp)
    {
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("child_experience", exp);
    }

    public static void SetDiamond(string diamond)
    {
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("child_diamond", diamond);
    }

    public static void SetGender(string gender)
    {
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("child_gender", gender);
    }

    public static void SetAge(string age)
    {
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("child_age", age);
    }

    public static void SetSub(string sub)
    {
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("child_subscription", sub);
    }
    #endregion

    #region SIGNUP_LOGIN_RATING_SHARE
    public static void SendEventSignupSuccess(string email, string phone)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("account_signup", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("email", email),
            new Firebase.Analytics.Parameter("phone", phone),
        });
    }

    public static void SendEventLoginSuccess(string email, string phone)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("account_login", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("email", email),
            new Firebase.Analytics.Parameter("phone", phone),
        });
    }

    public static void SendEventCheckReport(string type = "skill_report")
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("account_check_report", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("report_type", type),
        });
    }

    public static void SendEventRating(int star)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("app_rating", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("star", star),
        });
    }

    public static void SendEventShareFriend()
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("app_share");
    }
    #endregion

    #region IN_APP_PURCHASE
    public static void SendEventProductCheckout(string position)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("product_checkout", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("initial_position", position),
        });
    }

    public static void SendEventProductPurchase(string name)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("product_purchase", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("product_name", name),
        });
    }

    public static void SendEventProductConsultation()
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("product_consultation");
    }
    #endregion

    #region STORE
    public static void SendEventBuyItem(string id, int price)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("purchase_costume", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("costume_id", id),
            new Firebase.Analytics.Parameter("costume_price", price),
        });
    }

    public static void SendEventChangeItem(string id)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("change_costume", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("costume_id", id),
        });
    }
    #endregion

    #region KID_ZONE
    public static void SendEventUnitAccess(string name)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("unit_access", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("unit_name", name),
        });
    }

    public static void SendEventUnitComplete(string name, int star, string result = "passed")
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("unit_complete", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("unit_name", name),
            new Firebase.Analytics.Parameter("unit_star", star),
            new Firebase.Analytics.Parameter("unit_result", result),
        });
    }

    public static void SendEventLessonAccess(string name)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("lesson_access", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("lesson_name", name),
        });
    }

    public static void SendEventLessonComplete(string name, int star)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("lesson_complete", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("lesson_name", name),
            new Firebase.Analytics.Parameter("lesson_star", star),
        });
    }

    public static void SendEventActivityAccess(string name, string type = "game", string storyAction = "")
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("activitiy_access", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("activity_name", name),
            new Firebase.Analytics.Parameter("activity_type", type),
            new Firebase.Analytics.Parameter("story_action", storyAction),
        });
    }

    public static void SendEventActivityPlay(string name, string type, string time, string result = "exit", int star = 0)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("activitiy_play", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("activity_name", name),
            new Firebase.Analytics.Parameter("activity_type", type),
            new Firebase.Analytics.Parameter("activity_star", star),
            new Firebase.Analytics.Parameter("activity_time", time),
            new Firebase.Analytics.Parameter("activity_result", result),
        });
    }
    #endregion

    #region INTERACTIVE_ZONE
    public static void SendEventInteractivePractice(string name, string time, string result = "completed")
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("interactive_practice", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("speaking_item", name),
            new Firebase.Analytics.Parameter("speaking_time", time),
            new Firebase.Analytics.Parameter("practice_result", result),
        });
    }

    public static void SendEventInteractiveChallenge(string name, string time, string result = "win")
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("interactive_challenge", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("speaking_item", name),
            new Firebase.Analytics.Parameter("speaking_time", time),
            new Firebase.Analytics.Parameter("challenge_result", result),
        });
    }
    #endregion

    #region LIBRARY_ZONE
    public static void SendEventStoryDownload(string name)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("story_download", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("story_name", name),
        });
    }

    public static void SendEventStoryAccess(string name)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("story_access", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("story_name", name),
        });
    }

    public static void SendEventStoryRead(string name, string time, string result = "completed")
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("story_read", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("story_name", name),
            new Firebase.Analytics.Parameter("story_time", time),
            new Firebase.Analytics.Parameter("story_result", result),
        });
    }
    #endregion
}