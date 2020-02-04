using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [System.Serializable]
    public struct Probability
    {
        public string name;
        public ItemType itemType;
        public float probability;
        public Sprite sprite;
    }

    public static ItemManager Instance { get; private set; }

    [Header("Prefab And Data")]
    public ItemController itemPrefab;
    public Probability[] probabilitys;

    [Header("Spawn Setup")]
    public float itemSpanSpace = 2f;
    public ConveryorMover converyorMover;
    public bool enableSpawn = false;

    private List<ItemController> itemControllers = new List<ItemController>();

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        itemPrefab.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        bool isForward = converyorMover.speed > 0;
        bool isReverse = converyorMover.speed < 0;

        if (enableSpawn)
        {
            if (isForward)
            {
                float firstPosition = converyorMover.GetFirstPosition();
                if (firstPosition >= itemSpanSpace || firstPosition < 0)
                {
                    GameObject go = CreateItem();
                    converyorMover.PutItemAtBegin(go.transform);
                }
            }
            else if (isReverse)
            {
                float lastPosition = converyorMover.GetLastPosition();
                float totalDistance = converyorMover.GetTotalDistance();
                if (lastPosition <= totalDistance - itemSpanSpace || lastPosition < 0)
                {
                    GameObject go = CreateItem();
                    converyorMover.PutItemAtEnd(go.transform);
                }
            }
        }

        List<Transform> outsideItems = converyorMover.GetOutsideItems();
        List<ItemController> removeItems = new List<ItemController>();
        for (int i = 0; i < itemControllers.Count; i++)
        {
            if (outsideItems.Contains(itemControllers[i].transform))
            {
                removeItems.Add(itemControllers[i]);
            }
        }
        for (int i = 0; i < removeItems.Count; i++)
        {
            RemoveItem(removeItems[i]);
        }

    }

    GameObject CreateItem()
    {
        float _TotalProbability = 0f;
        for (int i = 0; i < probabilitys.Length; i++)
        {
            _TotalProbability += probabilitys[i].probability;
        }

        float _RandomValue = Random.Range(0, _TotalProbability);

        float _CurrentProbability = 0f;
        int _RandomIndex = probabilitys.Length - 1;
        for (int i = 0; i < probabilitys.Length; i++)
        {
            _CurrentProbability += probabilitys[i].probability;
            if (_RandomValue <= _CurrentProbability)
            {
                _RandomIndex = i;
                break;
            }
        }

        int _ID = (int)probabilitys[_RandomIndex].itemType;

        GameObject go = (GameObject)GameObject.Instantiate(itemPrefab.gameObject);
        ItemController ic = go.GetComponent<ItemController>();
        ic.Init(_ID, probabilitys[_RandomIndex].sprite);

        go.transform.parent = transform;
        go.SetActive(true);

        itemControllers.Add(ic);

        return go;
    }

    void RemoveItem(ItemController itemController)
    {
        if (!itemControllers.Contains(itemController)) return;

        itemControllers.Remove(itemController);

        converyorMover.DetachItem(itemController.transform);

        Destroy(itemController.gameObject);
    }

    public void PickItem(ItemController itemController)
    {
        RemoveItem(itemController);
    }

    public void RemoveAll()
    {
        for (int i = 0; i < itemControllers.Count; i++)
        {
            converyorMover.DetachItem(itemControllers[i].transform);
            Destroy(itemControllers[i].gameObject);
        }
        itemControllers.Clear();
    }
}
