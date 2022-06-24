using UnityEngine;
namespace Stories
{
    public class BrigeInterActiveZone : MonoBehaviour
    {
        public string id = "";
        public void OnMoveToInteractiveZone()
        {
            SendEventFirebase.SendEventActivityAccess(gameObject.name, "story", "speaking_practice");
            /*
            SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */
            SceneManagerBehavior.GetInstance().StoryId = id;
            StartCoroutine(SceneManagerBehavior.GetInstance().ChangeToNewScene(SceneManagerBehavior.GetInstance().m_InteractiveZoneScene, this.gameObject));

        }
        public void ToKidZone()
        {
            StartCoroutine(SceneManagerBehavior.GetInstance().ChangeToNewScene(SceneManagerBehavior.GetInstance().m_MainWorldMapScene,this.gameObject));
        }
    }
}
