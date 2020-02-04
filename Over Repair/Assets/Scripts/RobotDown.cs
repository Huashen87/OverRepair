using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotDown : MonoBehaviour {

    public enum ItemID {
        Head    = 0,
        Hand    = 1,
        Leg     = 2,
        Battery = 3,
        WeldGun = 4,
        Wrench  = 5,
        Glue    = 6,
        Tape    = 7
    }

    public enum BodyPart {
        Head      = 0,
        LeftHand  = 1,
        RightHand = 2,
        LeftLeg   = 3,
        RightLeg  = 4,
        Battery   = 5
    }

    public enum PartState {
        Broken = 0,
        Normal = 1
    }

    // public properties
    public GameObject headGO;
    public GameObject leftHandGO;
    public GameObject rightHandGO;
    public GameObject leftLegGO;
    public GameObject rightLegGO;
    public SpriteRenderer bodySpriteRenderer;
    public Sprite bodyPoweredSprite;
    public Sprite bodyUnpoweredSprite;

    public PartState headState;
    public PartState leftHandState;
    public PartState rightHandState;
    public PartState leftLegState;
    public PartState rightLegState;
    public PartState batteryState;

    public bool IsAllFix
    {
        get {
            return headState == PartState.Normal && leftHandState == PartState.Normal && rightHandState == PartState.Normal && leftLegState == PartState.Normal && rightLegState == PartState.Normal && batteryState == PartState.Normal;
        }
    }

    public bool IsCanPlayNextRound
    {
        get {
            return batteryState == PartState.Normal;
        }
    }


    // private properties
    Dictionary<BodyPart, PartState>  _bodyPartStates;
    Dictionary<BodyPart, GameObject> _bodyPartGameObjects;

    List<int> tempItems = new List<int>();

    void Start () {

        _bodyPartStates = new Dictionary<BodyPart, PartState>() {
            {BodyPart.Head,      headState},
            {BodyPart.LeftHand,  leftHandState},
            {BodyPart.RightHand, rightHandState},
            {BodyPart.LeftLeg,   leftLegState},
            {BodyPart.RightLeg,  rightLegState},
            {BodyPart.Battery,   batteryState}
        };

        _bodyPartGameObjects = new Dictionary<BodyPart, GameObject>() {
            {BodyPart.Head,      headGO},
            {BodyPart.LeftHand,  leftHandGO},
            {BodyPart.RightHand, rightHandGO},
            {BodyPart.LeftLeg,   leftLegGO},
            {BodyPart.RightLeg,  rightLegGO}
        };

        UpdateSprite();
    }

    // stateData Example: {false, true, true, false, false, false} : {head, leftHand, rightHand, leftLeg, rightLeg, battery} , (0 for broken, 1 for normal)
    public void Init (bool[] stateData) {
        for (int i = 0 ; i < stateData.Length ; i++) {
            int intValueFromBool = stateData[i] ? 1 : 0;
            _bodyPartStates[ (BodyPart)i ] = (PartState)intValueFromBool;
        }
        UpdateSprite();
    }

    public void UpdateSprite () {

        headState = _bodyPartStates[BodyPart.Head];
        leftHandState = _bodyPartStates[BodyPart.LeftHand];
        rightHandState = _bodyPartStates[BodyPart.RightHand];
        leftLegState = _bodyPartStates[BodyPart.LeftLeg];
        rightLegState = _bodyPartStates[BodyPart.RightLeg];
        batteryState = _bodyPartStates[BodyPart.Battery];

        foreach (var partAndState in _bodyPartStates) {
            BodyPart  part  = partAndState.Key;
            PartState state = partAndState.Value;

            if (part != BodyPart.Battery) {
                if (state == PartState.Broken)
                    _bodyPartGameObjects[part].SetActive(false);
                else if (state == PartState.Normal)
                    _bodyPartGameObjects[part].SetActive(true);
            }
        }

        if (_bodyPartStates[BodyPart.Battery] == PartState.Broken)
            bodySpriteRenderer.sprite = bodyUnpoweredSprite;
        else if (_bodyPartStates[BodyPart.Battery] == PartState.Normal)
            bodySpriteRenderer.sprite = bodyPoweredSprite;

    }

    public bool InterActByPlayer (int itemId) {

        if (_bodyPartStates[BodyPart.Head] == PartState.Broken && (itemId == 0 || itemId == 4 || itemId == 7) && !tempItems.Contains(itemId))
            tempItems.Add(itemId);

        else if ((_bodyPartStates[BodyPart.LeftHand] == PartState.Broken || _bodyPartStates[BodyPart.RightHand] == PartState.Broken) && (itemId == 1 || itemId == 5 || itemId == 7) && !tempItems.Contains(itemId))
                tempItems.Add(itemId);

        else if ((_bodyPartStates[BodyPart.LeftLeg] == PartState.Broken || _bodyPartStates[BodyPart.RightLeg] == PartState.Broken) && (itemId == 2 || itemId == 6 || itemId == 7) && !tempItems.Contains(itemId))
                tempItems.Add(itemId);

        else if (_bodyPartStates[BodyPart.Battery] == PartState.Broken && itemId == 3)
            _bodyPartStates[BodyPart.Battery] = PartState.Normal;

        else
            return false;


        // check if repairing happened
        int itemIdIterator = 0;
        int counterForSafety = 0;
        while(itemIdIterator < 3 || counterForSafety > 100) {
            counterForSafety++;
            if ( !CheckIfRepairable(itemIdIterator) )
                itemIdIterator++;
        }

        UpdateSprite();
        return true;
    }


    public bool CheckIfRepairable (int bodyPartId) {

        if (tempItems.Contains(bodyPartId)) {
            if (tempItems.Contains(bodyPartId + 4) || tempItems.Contains(7)) {

                tempItems.Remove(bodyPartId);
                if (tempItems.Contains(bodyPartId + 4))
                    tempItems.Remove(bodyPartId + 4);
                else
                    tempItems.Remove(7);

                if (bodyPartId == 0)
                    _bodyPartStates[BodyPart.Head] = PartState.Normal;
                else if (bodyPartId == 1) {
                    if (_bodyPartStates[BodyPart.LeftHand] == PartState.Broken)
                        _bodyPartStates[BodyPart.LeftHand] = PartState.Normal;
                    else if (_bodyPartStates[BodyPart.RightHand] == PartState.Broken)
                        _bodyPartStates[BodyPart.RightHand] = PartState.Normal;
                }
                else if (bodyPartId == 2) {
                    if (_bodyPartStates[BodyPart.LeftLeg] == PartState.Broken)
                        _bodyPartStates[BodyPart.LeftLeg] = PartState.Normal;
                    else if (_bodyPartStates[BodyPart.RightLeg] == PartState.Broken)
                        _bodyPartStates[BodyPart.RightLeg] = PartState.Normal;
                }
                return true;
            }
        }
        return false;
    }

    public bool[] GetStateData () {

        bool[] result = new bool[_bodyPartStates.Count];

        foreach (KeyValuePair<BodyPart, PartState> partState in _bodyPartStates) {

            bool boolValueFromInt = (int)partState.Value != 0;
            result[ (int)partState.Key ] = boolValueFromInt;
        }

        return result;
    }

    public int[] GetMissing () {
        List<int> resultList = new List<int>();
        bool[] data = GetStateData();

        for (int i = 0 ; i < data.Length ; i++)
            if (!data[i])
                resultList.Add(i);

        return resultList.ToArray();
    }

    public int[] GetStandByItem () {
        return tempItems.ToArray();
    }

    public bool[] GetEatable () {

        bool[] result = new bool[8] {
            false, false, false, false, false, false, false, false
        };

        List<int> missingList = new List<int>( GetMissing() );
        List<int> standbyList = new List<int>( GetStandByItem() );

        // head
        if (missingList.Contains(0)) {

            if (!standbyList.Contains(0))
                result[0] = true;

            if (!standbyList.Contains(4) && !standbyList.Contains(7))
                result[4] = true;

        }
        // hand
        if (missingList.Contains(1) || missingList.Contains(2)) {

            if (!standbyList.Contains(1))
                result[1] = true;
            if (!standbyList.Contains(5) && !standbyList.Contains(7))
                result[5] = true;

        }
        // leg
        if (missingList.Contains(3) || missingList.Contains(4)) {

            if (!standbyList.Contains(2))
                result[2] = true;
            if (!standbyList.Contains(6) && !standbyList.Contains(7))
                result[6] = true;

        }
        // battery
        if (missingList.Contains(5))  {

            result[3] = true;
        }

        return result;
    }
}
