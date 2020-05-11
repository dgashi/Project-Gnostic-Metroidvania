using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetChoosing : MonoBehaviour
{
    List<Detection> detectors;

    public List<VisibleTarget.TargetType> priorities;
    public VisibleTarget currentTarget;

    void Start()
    {
        foreach (Detection i in GetComponentsInChildren<Detection>())
        {
            detectors.Add(i);
        }
    }

    void Update()
    {
        //Scan for each eye and combine results into one hashset
        HashSet<VisibleTarget> visibleTargets = new HashSet<VisibleTarget>();

        foreach (Detection detector in detectors)
        {
            visibleTargets.UnionWith(detector.ScanForTargets());
        }

        //If no valid visible targets were found, set current target to null
        if (visibleTargets.Count == 0)
        {
            currentTarget = null;
        }
        else
        {
            foreach (VisibleTarget possibleTarget in visibleTargets)
            {

            }
        }
    }
}
