using System.Collections;
using Scripts;

namespace Map
{
    using UnityEngine;

    public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;

    public GameObject dungeonGeneratorPrefab;

    public GameObject gragusPrefab;
    private GameObject _gragusInstance;

    public int floor = 0;
    private GameObject _currentDungeon;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        // Instantiate Gragus only once
        if (_gragusInstance == null)
        {
            _gragusInstance = Instantiate(gragusPrefab);
            DontDestroyOnLoad(_gragusInstance);
        }

        StartCoroutine(GenerateFirstDungeonNextFrame());
    }

    private IEnumerator GenerateFirstDungeonNextFrame()
    {
        yield return null;

        _currentDungeon = Instantiate(dungeonGeneratorPrefab);
        var dungeonGen = _currentDungeon.GetComponent<DungeonGenerator>();
        dungeonGen.normalRoomCount = (6 + (floor * 3));
        dungeonGen.specialRoomCount = 1 + floor;
        dungeonGen.SetPlayerInstance(_gragusInstance);
        
        yield return new WaitUntil(() => dungeonGen.IsGenerationComplete);

        PositionGragusAtStart();
    }


    public void GoToNextFloor()
    {
        floor++;

        if (_currentDungeon != null)
            Destroy(_currentDungeon);

        StartCoroutine(GenerateNewFloor());
    }

    private IEnumerator GenerateNewFloor()
    {
        yield return null;

        DungeonGenerator genData = dungeonGeneratorPrefab.GetComponent<DungeonGenerator>();
        genData.normalRoomCount = (6 + (floor * 3));
        genData.specialRoomCount = 1 + floor;

        _currentDungeon = Instantiate(dungeonGeneratorPrefab);
        var dungeonGen = _currentDungeon.GetComponent<DungeonGenerator>();
        dungeonGen.SetPlayerInstance(_gragusInstance);

        yield return new WaitUntil(() => dungeonGen.IsGenerationComplete);

        PositionGragusAtStart();
    }


    private void PositionGragusAtStart()
    {
        if (_currentDungeon == null || _gragusInstance == null) return;

        var dungeonGen = _currentDungeon.GetComponent<DungeonGenerator>();
        if (dungeonGen == null) return;

        Vector3 basePos = dungeonGen.GetStartRoomPosition();
        Vector3 offset = dungeonGen.playerOffsetInStartRoom;
        
        _gragusInstance.transform.position = basePos + offset + Vector3.up * 0.5f;
        var controller = _gragusInstance.GetComponent<PlayerController>();
        if (controller != null && !controller.enabled)
        {
            controller.enabled = true;
            Debug.Log("Player controller re-enabled.");
        }
    }


    public int GetCurrentFloor() => floor;
}


}