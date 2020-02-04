using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathController : MonoBehaviour
{
    public List<Transform> points = new List<Transform>();
    private List<float> positions = new List<float>();

    public float TotalDistance { get; private set; }

    private void Awake()
    {
        TotalDistance = 0;
        for (int i = 1; i < points.Count; i++)
        {
            TotalDistance += Vector2.Distance(
                new Vector2(points[i - 1].position.x, points[i - 1].position.y),
                new Vector2(points[i].position.x, points[i].position.y)
            );
        }
    }

    public void SetToPosition(Transform transform, float position, bool withRotate = false)
    {
        float _CurrentPosition = 0f;
        float _PrevAngle = 0f;
        for (int i = 1; i < points.Count; i++)
        {
            Vector2 _From = new Vector2(points[i - 1].position.x, points[i - 1].position.y);
            Vector2 _To = new Vector2(points[i].position.x, points[i].position.y); ;

            float _Distance = Vector2.Distance(_From, _To);

            float _Angle = Vector2.SignedAngle(new Vector2(1, 0), _To - _From);
            if (i == 1) _PrevAngle = _Angle;

            if (position >= _CurrentPosition && position <= _CurrentPosition + _Distance)
            {
                float _RawPosition = position - _CurrentPosition;
                float _RawPersent = _RawPosition / _Distance;
                Vector2 _NewPosition = Vector2.Lerp(_From, _To, _RawPersent);

                Vector3 _ObjPosition = transform.position;
                _ObjPosition.x = _NewPosition.x;
                _ObjPosition.y = _NewPosition.y;
                transform.position = _ObjPosition;

                

                if (withRotate)
                {
                    Vector3 _Rotate = transform.eulerAngles;
                    float _FromAngle = _PrevAngle % 360;
                    float _ToAngle = _Angle % 360;
                    if (_ToAngle > _FromAngle)
                    {
                        if (_ToAngle - _FromAngle > 180)
                        {
                            _ToAngle -= 360;
                        }
                    }
                    else
                    {
                        if (_FromAngle - _ToAngle > 180)
                        {
                            _FromAngle -= 360;
                        }
                    }
                    _Rotate.z = Mathf.Lerp(_FromAngle, _ToAngle, _RawPersent);
                    transform.eulerAngles = _Rotate;
                }

                
                return;
            }

            _PrevAngle = _Angle;
            _CurrentPosition += _Distance;
        }
    }

    void OnDrawGizmos()
    {
        Color originColor = Gizmos.color;
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(points[i].position, 0.15f);
            if (i > 0)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(points[i - 1].position, points[i].position);
            }
        }
        Gizmos.color = originColor;
    }
}
