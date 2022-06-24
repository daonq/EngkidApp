using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KidZoneWorldMapBehavior))]
public class KidZoneMilestoneManager : MonoBehaviour
{
    public KidZoneWorldMapBehavior m_MapBehavior;

    //internals
    [HideInInspector] public MilestoneInfo[] currentMilestoneInfoList;
    [HideInInspector] public UnitResultInfo[] currentUnitResultInfoList;
    [HideInInspector] public AccountProgressInfo currentAccountProgressInfo;

    private void OnEnable()
    {
        StartCoroutine(CheckUserPremiumStatus());
    }

    IEnumerator CheckUserPremiumStatus()
    {
        yield return null;
        yield return null;
        yield return null;
        DataBaseInterface.GetInstance().GetJSONRequest(
            DataBaseInterface.USER_PREMIUM_STATUS_URI,
            callback_flag =>
            {
                if (callback_flag == false)
                    Debug.Log("Error: get premium status detail failed.");
                else
                    Debug.Log("Done! Get premium status detail successful!");
            },
            server_reply =>
            {
                //Debug.Log(server_reply.data.permissions);
                UserDataManagerBehavior.GetInstance().SetPremiumPermission(server_reply.data.permissions);

                StartCoroutine(OnRequestMilestonesList());
            },
            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
            );
    }

    IEnumerator OnRequestMilestonesList()
    {
        //Debug.Log("Requested milestone info data!");
        //delay 3 frames
        yield return null;
        yield return null;
        yield return null;
        DataBaseInterface.GetInstance().GetJSONRequest(
            DataBaseInterface.GET_MILESTONES_DETAIL_URI,
            callback_flag =>
            {
                if (callback_flag == false)
                    Debug.Log("Error: get milestone detail failed.");
                //else
                //    Debug.Log("Done! Get milestone detail successful!");
            },
            server_reply =>
            {
                currentMilestoneInfoList = server_reply.data.milestones;
                currentUnitResultInfoList = server_reply.data.unit_results;
                currentAccountProgressInfo = server_reply.data.account_progress;

                m_MapBehavior.UpdateAllMilestonesInfo(currentMilestoneInfoList, currentUnitResultInfoList, currentAccountProgressInfo);

                UserDataManagerBehavior.GetInstance().currentMilestoneInfoList.Clear(); //= new MilestoneInfo[currentMilestoneInfoList.Length];
                for (int i = 0; i < currentMilestoneInfoList.Length; i++)
                {
                    UserDataManagerBehavior.GetInstance().currentMilestoneInfoList.Add(currentMilestoneInfoList[i]);
                }
            },
            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
            );
    }
}
