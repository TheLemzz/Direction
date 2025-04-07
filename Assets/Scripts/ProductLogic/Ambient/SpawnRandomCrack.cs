using UnityEngine;

public class SpawnRandomCrack : MonoBehaviour
{
    [SerializeField, Range(0, 100)] private int _chance;
    [SerializeField] private CrackChance[] _cracks;
    
    private Transform[] _positions;


    private void Start()
    {
        transform.GetChild(0).position.AddY(0.0001f);

        _positions = transform.GetChild(0).GetComponentsInChildren<Transform>();

        if (Random.Range(0, 100) <= _chance) GenerateRandomCracks();
    }

    private void GenerateRandomCracks()
    {
        int totalChance = 0;

        foreach (CrackChance crackChance in _cracks)
        {
            totalChance += crackChance.Chance;
        }


        int randomNumber = Random.Range(0, totalChance);
        int cumulativeChance = 0;


        foreach (CrackChance crackChance in _cracks)
        {
            cumulativeChance += crackChance.Chance;
            if (randomNumber < cumulativeChance)
            {

                Instantiate(crackChance.Crack, _positions.PickRandomElement().position, crackChance.Crack.transform.rotation);
                return;
            }
        }
    }
}
