using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveryorMover : MonoBehaviour
{
    private class ItemObject
    {
        public float position;
        public Transform transform;
    }

    public static ConveryorMover Instance { get; private set; }

    public float deltaMovement { get; private set; }

    public PathController path;
    public GameObject slidePrefab;
    public float space = 0.5f;
    public float speed = 0.25f;

    private float offset = 0f;
    private List<GameObject> slideGameObjects = new List<GameObject>();
    private List<ItemObject> itemObjects = new List<ItemObject>();

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        int slideCount = (int)(path.TotalDistance / space);
        float currentPosition = 0;
        for (int i = 0; i < slideCount; i++)
        {
            GameObject go = GameObject.Instantiate(slidePrefab);
            path.SetToPosition(go.transform, currentPosition, true);
            currentPosition += space;

            slideGameObjects.Add(go);
            go.transform.parent = this.transform;
        }
        slidePrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        deltaMovement = speed * Time.deltaTime;
        offset += deltaMovement;
        while (offset >= space)
        {
            offset -= space;
        }
        while (offset <= -space)
        {
            offset += space;
        }

        // Update Slide
        for (int i = 0; i < slideGameObjects.Count; i++)
        {
            path.SetToPosition(slideGameObjects[i].transform, space * i + offset, true);
        }

        // Update Item
        for (int i = 0; i < itemObjects.Count; i++)
        {
            itemObjects[i].position += deltaMovement;
            path.SetToPosition(itemObjects[i].transform, itemObjects[i].position, withRotate: false);
        }
    }


    public void PutItemAtBegin(Transform target)
    {
        itemObjects.Add(new ItemObject() { position = 0f, transform = target });
        path.SetToPosition(target, 0, withRotate: false);
    }

    public void PutItemAtEnd(Transform target)
    {
        itemObjects.Add(new ItemObject() { position = GetTotalDistance(), transform = target });
        path.SetToPosition(target, GetTotalDistance(), withRotate: false);
    }

    public void DetachItem(Transform target)
    {
        int _Index = -1;
        for (int i = 0; i < itemObjects.Count; i++)
        {
            if (itemObjects[i].transform == target)
            {
                _Index = i;
                break;
            }
        }

        if (_Index < 0) return;

        itemObjects.RemoveAt(_Index);
    }

    public float GetFirstPosition()
    {
        if (itemObjects.Count == 0) return -1;

        float position = path.TotalDistance;
        for (int i = 0; i < itemObjects.Count; i++)
        {
            if (itemObjects[i].position < position)
            {
                position = itemObjects[i].position;
            }
        }

        return position;
    }

    public float GetLastPosition()
    {
        if (itemObjects.Count == 0) return -1;

        float position = 0;
        for (int i = 0; i < itemObjects.Count; i++)
        {
            if (itemObjects[i].position > position)
            {
                position = itemObjects[i].position;
            }
        }

        return position;
    }

    public float GetTotalDistance()
    {
        return path.TotalDistance;
    }

    public List<Transform> GetOutsideItems()
    {
        List<Transform> transforms = new List<Transform>();

        if (speed == 0) return transforms;

        float totalDistance = GetTotalDistance();
        for (int i = 0; i < itemObjects.Count; i++)
        {
            if (speed > 0) {
                if (itemObjects[i].position >= totalDistance)
                {
                    transforms.Add(itemObjects[i].transform);
                }
            }
            else if(speed < 0)
            {
                if (itemObjects[i].position <= 0)
                {
                    transforms.Add(itemObjects[i].transform);
                }
            }
        }

        return transforms;
    }
}
