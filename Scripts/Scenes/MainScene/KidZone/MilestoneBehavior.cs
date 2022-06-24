using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilestoneBehavior : MonoBehaviour
{
    public int m_MilestoneID = 0;
    public List<UnitBaseBehavior> m_UnitsList = new List<UnitBaseBehavior>();
    public UnitBaseBehavior m_ExamUnit;

    public void OnInitUnitsList()
    {
        int i = 1;
        foreach (UnitBaseBehavior unit in m_UnitsList)
        {
            unit.m_UnitState = EngKidAPI.UnitStates.LOCKED;
            unit.SetUnitNumberText((m_MilestoneID * 5 + i).ToString());
            i++;
        }    
    }

    public void SetUnitsStates(int mark)
    {
        //Debug.Log(mark);
        for (int i = 0; i <= mark; i++)
        {
            m_UnitsList[i].m_UnitState = EngKidAPI.UnitStates.UNLOCKED;
            m_UnitsList[i].UpdateDisplay();
        }
    }

    public void SetUnitsData(MilestoneInfo milestone_info)
    {
        for (int i = 0; i < m_UnitsList.Count && i < milestone_info.units.Length; i++)
        {
            m_UnitsList[i].m_UnitDatabaseID = milestone_info.units[i]._id;
        }

        m_ExamUnit.m_UnitDatabaseID = milestone_info.checkpoint_exam._id;
    }
}
