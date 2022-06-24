using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EngKidAPI
{
    public enum UserLogInStates
    {
        NOT_LOGGED_IN = 0,
        LOGGED_IN,
    }

    public enum KidAgeGroups
    {
        UNDER_FIVE = 1,
        FIVE_TO_EIGHT,
        EIGHT_TO_TWELVE,
        OVER_TWELVE
    }

    public enum KidGenders
    {
        MALE = 1,
        FEMALE
    }

    public enum MilestoneIDs
    {
        NULL = 0,
        MILESTONE_1,
        MILESTONE_2,
        MILESTONE_3,
        MILESTONE_4,
        MILESTONE_5,
        MILESTONE_6,
        MILESTONE_7
    }

    public enum UnitIDs
    {
        NULL = 0,

        //milestone 1
        UNIT_1_1,
        UNIT_1_2,
        UNIT_1_3,
        UNIT_1_4,
        UNIT_1_5,
        //UNIT_1_TEST,

        //milestone 2
        UNIT_2_1,
        UNIT_2_2,
        UNIT_2_3,
        UNIT_2_4,
        UNIT_2_5,
        //UNIT_2_TEST,

        //milestone 3
        UNIT_3_1,
        UNIT_3_2,
        UNIT_3_3,
        UNIT_3_4,
        UNIT_3_5,
        //UNIT_3_TEST,

        //milestone 4
        UNIT_4_1,
        UNIT_4_2,
        UNIT_4_3,
        UNIT_4_4,
        UNIT_4_5,
        //UNIT_4_TEST,

        //milestone 5
        UNIT_5_1,
        UNIT_5_2,
        UNIT_5_3,
        UNIT_5_4,
        UNIT_5_5,
        //UNIT_5_TEST,

        //milestone 6
        UNIT_6_1,
        UNIT_6_2,
        UNIT_6_3,
        UNIT_6_4,
        UNIT_6_5,
        //UNIT_6_TEST,

        //milestone 7
        UNIT_7_1,
        UNIT_7_2,
        UNIT_7_3,
        UNIT_7_4,
        UNIT_7_5,
        //UNIT_7_TEST,
    }

    public enum LessonIDs
    {
        NULL = 0,
        LESSON_1,
        LESSON_2,
        LESSON_3,
        LESSON_4,
        LESSON_5
    }

    public enum ActivityIDs
    {
        NULL = 0,
        ACTIVITY_1,
        ACTIVITY_2,
        ACTIVITY_3,
        ACTIVITY_4,
        ACTIVITY_5,
    }

    public enum UnitStates
    {
        LOCKED = 0,
        UNLOCKED,
    }

    public enum UnitProgression
    {
        NOT_STARTED = 0,
        IN_PROGRESS,
        PASSED
    }

    public class CommonAPI : MonoBehaviour
    {

    }

    public class SelfDestroy : MonoBehaviour
    {
        public void InitSelfDestroy(float fuse_time)
        {
            StartCoroutine(DelayedDestroy(fuse_time));
        }

        IEnumerator DelayedDestroy(float fuse_time)
        {
            yield return new WaitForSeconds(fuse_time);
            Destroy(this.gameObject);
        }
    }
}