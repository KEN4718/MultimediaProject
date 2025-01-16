using UnityEngine;
using System.Collections.Generic;

public class TypeSwitcher : MonoBehaviour
{
    [Tooltip("List of cubes whose type property will be changed.")]
    public List<WayPoint> targetCubes;

    [Tooltip("The type value to set on the target cubes.")]
    public int newType;

    private void OnTriggerEnter(Collider other)
    {
        ZeraMovement zera = other.GetComponent<ZeraMovement>();
        if (zera != null)
        {
            if (targetCubes != null && targetCubes.Count > 0)
            {
                foreach (WayPoint cube in targetCubes)
                {
                    if (cube != null)
                    {
                        zera.inType = newType;
                        cube.SetType(newType, 2);
                    }
                }
            }
        }
    }

    public void ExecuteOnce()
    {
        foreach (WayPoint myGrid in targetCubes)
        {
            myGrid.type = newType;
        }
    }
}
