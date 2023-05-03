using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    int UILayer;
    public PlayerCar player;
    public Inventory inventory;
    public static GameManager instance;
    public CameraManager cameraManager;
    public ItemData itemData;
    public InventoryUI inventoryUI;
    public List<Cluster> clusters;
    public SGrid playerCluster;
    public bool gameStarted;
    public CanvasGroup instructions;
    private float timer = 0f;
    public Cluster clusterPrefab;
    public enum PlayerState
    {
        Drive,
        Inventory
    }

    public PlayerState playerState;
    private void Awake()
    {
        instance = this;
        UILayer = LayerMask.NameToLayer("UI");
        clusters = new List<Cluster>(9);
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        Debug.Log(Application.targetFrameRate);
        inventory = new Inventory();
        inventory.width = 4;
        inventory.height = 5;
        inventory.items = new List<Inventory.SlotItem>();
        player.currentAngle = -45f;
    }

    void Start()
    {
        GenerateClustersAround(new SGrid(0, 0));
    }

    void GenerateClustersAround(SGrid grid)
    {
        for(var x = -1;x<=1;x++)
        {
            for(var y = -1;y<=1;y++)
            {
                GenerateCluster(grid.Shift(x,y));
            }
        }
    }
    void GenerateCluster(SGrid grid)
    {
        foreach (var cluster in clusters)
        {
            if (cluster.grid.Equals(grid)) return;
        }
        var c = new GameObject("Cluster").AddComponent<Cluster>();
        c.grid = grid;
        clusters.Add(c);
        c.transform.position = new Vector2(100 * grid.x, 100 * grid.y);
        c.Generate();
    }
    private void Update()
    {
        if (gameStarted)
        {
            PlayerInput();
            var currentCluster = CurrentCluster();
            if (!playerCluster.Equals(currentCluster))
            {
                playerCluster = currentCluster;
                GenerateClustersAround(currentCluster);
            }

            timer += Time.deltaTime;
            if (timer > 5f)
            {
                instructions.alpha = 6f-timer;
            }
        }
        else
        {
            if (Input.anyKeyDown)
            {
                gameStarted = true;
            }
            else
            {
                player.motion = Vector2.one;
            }
        }
    }

    void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerState = playerState == PlayerState.Drive ? PlayerState.Inventory : PlayerState.Drive;
            if (playerState == PlayerState.Inventory)
            {
                inventoryUI.Open();
            }
            else
            {
                inventoryUI.Close();
            }
        }
        switch (playerState)
        {
            case PlayerState.Drive:
                break;
            case PlayerState.Inventory:
                if (Input.GetMouseButtonDown(0))
                {
                    bool touchingUI = false;
                    PointerEventData eventData = new PointerEventData(EventSystem.current);
                    eventData.position = Input.mousePosition;
                    List<RaycastResult> raysastResults = new List<RaycastResult>(32);
                    EventSystem.current.RaycastAll(eventData, raysastResults);
                    foreach (var result in raysastResults)
                    {
                        if (result.gameObject.layer == UILayer)
                        {
                            touchingUI = true;
                            break;
                        }
                    }

                    if (!touchingUI)
                    {
                        var coll = Physics2D.OverlapPointAll(cameraManager.cam.ScreenToWorldPoint(Input.mousePosition));
                        foreach (var c in coll)
                        {
                            if (c.TryGetComponent(out ItemGib physObj))
                            {
                                var draggable = inventoryUI.GetDraggable();
                                draggable.Load(ItemData.Instance.items[physObj.itemID]);
                                draggable.rectTransform.position = Input.mousePosition;
                                draggable.dragging = true;
                                physObj.gameObject.SetActive(false);
                                break;
                            }
                        }
                    }
                }
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(100,100,0.01f));
    }

    public SGrid CurrentCluster()
    {
        return new SGrid(Mathf.RoundToInt(player.Transform.position.x / 100),Mathf.RoundToInt(player.Transform.position.x / 100));
    }
}