using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetChoosing : MonoBehaviour
{
    List<Detection> detectors;

    public List<VisibleTarget.TargetType> priorities;
    private VisibleTarget currentTarget;


    void Start()
    {
        detectors = new List<Detection>();

        foreach (Detection i in GetComponentsInChildren<Detection>())
        {
            detectors.Add(i);
        }
    }

    public VisibleTarget CheckForTarget()
    {
        //Scan for each eye and combine results into one hashset
        HashSet<VisibleTarget> visibleTargets = new HashSet<VisibleTarget>();

        foreach (Detection detector in detectors)
        {
            visibleTargets.UnionWith(detector.ScanForTargets());
        }

        //If current target is no longer visible, set current target to null
        if (!visibleTargets.Contains(currentTarget))
        {
            currentTarget = null;
        }

        //If no valid visible targets were found, set current target to null
        if (visibleTargets.Count == 0)
        {
            currentTarget = null;
        }
        else
        {
            //Iterate through list of target types from most to least important
            foreach (VisibleTarget.TargetType type in priorities)
            {
                //If the current target is of the type, stop iterating
                if (currentTarget && currentTarget.type == type)
                {
                    break;
                }

                //Iterate through all visible targets
                foreach (VisibleTarget target in visibleTargets)
                {
                    //Choose the first target of the type as current target and stop iteration
                    if (target.type == type)
                    {
                        currentTarget = target;
                        break;
                    }
                }
            }
        }

        return currentTarget;
    }
}
