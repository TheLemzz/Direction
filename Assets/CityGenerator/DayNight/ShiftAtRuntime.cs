using UnityEngine;

/*
Just an example of how to switch day and night at runtime
Requires DayNight prefab to be in scene (Hierarchy)
*/

public class ShiftAtRuntime : MonoBehaviour
{
    private DayNight dayNight;

    private void Start()
    {

        dayNight = FindObjectOfType<DayNight>();

    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (dayNight)
            {
                dayNight.isNight = !dayNight.isNight;
                dayNight.ChangeMaterial();

            }
        }

    }

}
