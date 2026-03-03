using UnityEditor;
using UnityEngine;

namespace CityGen
{

    [CustomEditor(typeof(TrafficCar))]
    public class TCarEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TrafficCar TF = (TrafficCar)target;

            if (GUILayout.Button("Generate WheelColliders"))
            {
                if (TF.gameObject.activeInHierarchy)
                    TF.Configure();
                else
                    Debug.LogWarning("Place the object in the hierarchy");
            }

        }


    }
}
