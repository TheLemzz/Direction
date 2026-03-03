using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace CityGen
{
    public class TrafficLight : MonoBehaviour
    {
        [SerializeField] private Human[] _humans;
        [SerializeField] private NavMeshSurface _meshSurface;

        [SerializeField] private GameObject Green;
        [SerializeField] private GameObject Yellow;
        [SerializeField] private GameObject Red;
        [SerializeField] private GameObject Pedestrians;
        [SerializeField] private GameObject StopCollider;
        [SerializeField] private GameObject StopPedestrianCollider;

        private int _humanCount;

        private readonly List<Human> _spawnedHumans = new();

        private IEnumerator Start()
        {
            if (_humans.Length == 0) yield break;

            _meshSurface.overrideTileSize = true;
            _meshSurface.tileSize = 32;

            DynamicMeshBuilder.GetInstance().Enqueue(_meshSurface);

            yield return new WaitUntil(() => _meshSurface.navMeshData != null);

            SpawnPedastrains();
        }

        private void SpawnPedastrains()
        {
            _humanCount = Random.Range(0, 8);

            Collider[] colliders = StopPedestrianCollider.GetComponentsInChildren<Collider>();

            int lasted = _humanCount;

            for (int i = 0; i < _humanCount / 2; i++, lasted--)
            {
                _spawnedHumans.Add(
                    Instantiate(_humans.PickRandomElement(), colliders[0].transform.position, Quaternion.identity)
                    .SetPositions(colliders[0], colliders[1])
                    .SetState(CurrentHumanState.Left));
            }

            for (int i = lasted; i >= 1; i--)
            {
                _spawnedHumans.Add(
                    Instantiate(_humans.PickRandomElement(), colliders[1].transform.position, Quaternion.identity)
                    .SetPositions(colliders[0], colliders[1])
                    .SetState(CurrentHumanState.Right));
            }
        }

        public void SetStatus(string status)
        {

            Red.SetActive(status == "1");
            Yellow.SetActive(status == "2");
            Green.SetActive(status == "3");
            Pedestrians.SetActive(status == "4");
            StopCollider.SetActive(status != "3");

            if (status == "4")
            {
                foreach (Human human in _spawnedHumans)
                {
                    StartCoroutine(human.ChangeState());
                }
            }
        }
    }
}