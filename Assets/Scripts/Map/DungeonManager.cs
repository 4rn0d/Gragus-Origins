using System.Collections;
using Scripts;
using UI;

namespace Map
{
    using UnityEngine;

    public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;

    public GameObject loadingPanel;
    public GameObject OtherUI;
    public AudioSource musicAudioSource;
    public GameObject dungeonGeneratorPrefab;

    public GameObject gragusPrefab;
    private GameObject _gragusInstance;

    public int floor = 1;
    public GameObject currentDungeon;
    public MinimapManager minimapManager;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        if (_gragusInstance == null)
        {
            _gragusInstance = Instantiate(gragusPrefab);
            DontDestroyOnLoad(_gragusInstance);
        }

        StartCoroutine(GenerateNewFloor());
    }

    private IEnumerator GenerateNewFloor()
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.Stop();
        }
        if (loadingPanel != null) loadingPanel.SetActive(true);
        if (OtherUI != null) OtherUI.SetActive(false);
        yield return null;
        currentDungeon = Instantiate(dungeonGeneratorPrefab);
        var dungeonGen = currentDungeon.GetComponent<DungeonGenerator>();
        dungeonGen.normalRoomCount = (3 + (floor * 2));
        if (floor == 1 || floor == 2)
            dungeonGen.specialRoomCount = 1;
        else
            dungeonGen.specialRoomCount = 2;
        dungeonGen.SetPlayerInstance(_gragusInstance);
        yield return new WaitUntil(() => dungeonGen.IsGenerationComplete);
        PositionGragusAtStart();
        yield return new WaitForSecondsRealtime(2f);
        
        if (minimapManager == null)
        {
            Debug.LogError("MinimapManager GameObject not found!");
            yield break;
        }
        var mapScript = minimapManager.GetComponent<MinimapManager>();
        if (mapScript == null)
        {
            Debug.LogError("MinimapManager component not found!");
            yield break;
        }
        var rooms = dungeonGen.GetRoom();
        if (rooms == null)
        {
            Debug.LogError("DungeonGenerator.GetRoom() returned null!");
            yield break;
        }
        mapScript.Initialize(rooms);
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (OtherUI != null) OtherUI.SetActive(true);
        if (musicAudioSource != null)
        {
            musicAudioSource.Play();
        }
    }
    
    public void GoToNextFloor()
    {
        floor++;

        if (currentDungeon != null)
            Destroy(currentDungeon);

        StartCoroutine(GenerateNewFloor());
    }


    private void PositionGragusAtStart()
    {
        if (currentDungeon == null || _gragusInstance == null) return;

        var dungeonGen = currentDungeon.GetComponent<DungeonGenerator>();
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