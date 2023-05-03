using System.Collections.Generic;
using Castle;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DraggableItemUI : MonoBehaviour, IPointerDownHandler
{
    public RectTransform rectTransform;
    [System.NonSerialized]
    public ItemGib itemGib;
    public int itemID;
    public Image image;
    public Orientation orientation;
    public Item Item => ItemData.Instance.items[itemID];
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public float width, height;
    public bool dragging;
    public CanvasGroup nameGroup, descGroup;
    public Inventory.SlotItem cachedSlot;
    public enum HoverState
    {
        Free,
        Occupied,
        Unoccupied
    }
    public void Load(Item item)
    {
        itemID = item.itemID;
        image.sprite = item.icon;
        nameText.SetText(item.name);
        descText.SetText(item.description);
        width = item.width * 110;
        height = item.height * 110;
        rectTransform.sizeDelta = image.rectTransform.sizeDelta = new Vector2(width,height);
        orientation = Orientation.Right;
    }
    public void Load(Inventory.SlotItem slotItem)
    {
        Load(slotItem.GetItem);
        itemGib = null;
        orientation = slotItem.orientation;
        cachedSlot = slotItem;
    }
    void Update()
    {
        if (dragging)
        {
            nameGroup.alpha = descGroup.alpha = Mathf.Lerp(nameGroup.alpha, 1, Time.unscaledDeltaTime * 5);
            if (Input.GetKeyDown(KeyCode.D))
            {
                switch (orientation)
                {
                    case Orientation.Right:
                        orientation = Orientation.Up;
                        break;
                    case Orientation.Up:
                        orientation = Orientation.Left;
                        break;
                    case Orientation.Left:
                        orientation = Orientation.Down;
                        break;
                    case Orientation.Down:
                        orientation = Orientation.Right;
                        break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                switch (orientation)
                {
                    case Orientation.Right:
                        orientation = Orientation.Down;
                        break;
                    case Orientation.Up:
                        orientation = Orientation.Right;
                        break;
                    case Orientation.Left:
                        orientation = Orientation.Up;
                        break;
                    case Orientation.Down:
                        orientation = Orientation.Left;
                        break;
                }
            }
            var targetRotation = Quaternion.identity;
            switch (orientation)
            {
                case Orientation.Right:
                    targetRotation = Quaternion.identity;
                    rectTransform.sizeDelta = new Vector2(width,height);
                    break;
                case Orientation.Up:
                    targetRotation = Quaternion.Euler(0, 0, -90);
                    rectTransform.sizeDelta = new Vector2(height,width);
                    break;
                case Orientation.Left:
                    targetRotation = Quaternion.Euler(0, 0, -180);
                    rectTransform.sizeDelta = new Vector2(width,height);
                    break;
                case Orientation.Down:
                    targetRotation = Quaternion.Euler(0, 0, -270);
                    rectTransform.sizeDelta = new Vector2(height,width);
                    break;
            }
            image.rectTransform.localRotation = Quaternion.Lerp(image.rectTransform.localRotation, targetRotation,
                Time.unscaledDeltaTime * 15);
            var checkState = Check(out var vec, out var slotID);
            if (checkState == HoverState.Free)
            {
                rectTransform.position = Vector3.Lerp(rectTransform.position, Input.mousePosition, Time.unscaledDeltaTime * 15);
                if (Input.GetMouseButtonUp(0))
                {
                    EndDrag();
                }
            }
            else
            {
                rectTransform.position = Vector3.Lerp(rectTransform.position, vec, Time.unscaledDeltaTime * 15);
                if (Input.GetMouseButtonUp(0))
                {
                    if (checkState == HoverState.Unoccupied)
                    {
                        PlaceItem(slotID);
                    }
                    else
                    {
                        EndDrag();
                    }
                }
            }
        }
        else
        {
            nameGroup.alpha = descGroup.alpha = Mathf.Lerp(nameGroup.alpha, 0, Time.unscaledDeltaTime * 5);
        }
    }

    HoverState Check(out Vector3 vec,out int slotID)
    {
        var offset = new Vector2(-(width - 110) / 2, -(height - 110) / 2) + Vector2.one * 10;
        if (orientation is Orientation.Up or Orientation.Down)
        {
            offset = new Vector2(offset.y, offset.x);
        }
        var data = new PointerEventData(EventSystem.current);
        data.position = (Vector2)Input.mousePosition+offset;
        var raycastResults = new List<RaycastResult>(32);
        EventSystem.current.RaycastAll(data,raycastResults);
        for (var i = 0; i < raycastResults.Count; i++)
        {
            if (!raycastResults[i].gameObject.TryGetComponent(out Slot slot))continue;
            
            vec = raycastResults[i].gameObject.transform.position - (Vector3)offset;
            slotID = raycastResults[i].gameObject.transform.GetSiblingIndex();
            slot.inventoryUI.Hover(slotID,Item,orientation);
            return GameManager.instance.inventory.CanInsert(slotID,Item,orientation) ? HoverState.Unoccupied : HoverState.Occupied;
        }

        vec = Input.mousePosition;
        slotID = -1;
        return HoverState.Free;
    }

    void PlaceItem(int slotID)
    {
        if (GameManager.instance.inventory.items == null)
        {
            GameManager.instance.inventory.items = new List<Inventory.SlotItem>();
        }
        cachedSlot = new Inventory.SlotItem()
        {
            itemID = itemID,
            orientation = orientation,
            position = GameManager.instance.inventory.Position(slotID)
        };
        GameManager.instance.inventory.items.Add(cachedSlot);
        GameManager.instance.inventory.ResetOccupiedGrids();
        GameManager.instance.inventoryUI.SetOccupied(GameManager.instance.inventory);
        dragging = false;
    }
    void EndDrag()
    {
        dragging = false;
        gameObject.SetActive(false);
        if (Item.gib != null)
        {
            var gib = 
            Instantiate(Item.gib, GameManager.instance.cameraManager.cam.ScreenToWorldPoint(Input.mousePosition).RepZ(0),
                Quaternion.identity);
            gib.Fly(0.2f);
        }
        
        //EventSystem.current.RaycastAll();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(Item.name);
        dragging = true;
        GameManager.instance.inventory.items.Remove(cachedSlot);
        GameManager.instance.inventory.ResetOccupiedGrids();
        GameManager.instance.inventoryUI.SetOccupied(GameManager.instance.inventory);
    }
}