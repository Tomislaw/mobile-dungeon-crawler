using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Pulley : MonoBehaviour
{
    public int ropeLength = 1;
    public int ropeLengthRolledUp = 0;

    public float rollingUpSpeed = 1;
    public float rollingDownSpeed = 0.5f;

    public bool rolledUp = true;

    public SpriteRenderer rope;
    public GameObject connectedObject;

    private float _currentRopeLength;


    void Start()
    {
        _currentRopeLength = rolledUp ? ropeLengthRolledUp : ropeLength;
        if (rope != null)
            rope.size = new Vector2(rope.size.x, _currentRopeLength);
        if (connectedObject != null)
            connectedObject.transform.position = transform.position + new Vector3(0, -_currentRopeLength - 1f, 0);
    }

    public void Roll(bool up)
    {
        rolledUp = up;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var targetRopeLength = rolledUp ? ropeLengthRolledUp : ropeLength;
        var speed = rolledUp ? rollingUpSpeed : rollingDownSpeed;
        if (_currentRopeLength != targetRopeLength)
        {
            var difference = targetRopeLength - _currentRopeLength;
            var step = Mathf.Sign(difference) * Time.fixedDeltaTime * speed;
           
            if (Mathf.Abs(difference) < Mathf.Abs(step))
                _currentRopeLength = targetRopeLength;
            else
                _currentRopeLength += step;

            if (rope != null)
                rope.size = new Vector2(rope.size.x, _currentRopeLength);

            if (connectedObject != null)
                connectedObject.transform.position = transform.position + new Vector3(0, -_currentRopeLength - 1f, 0);
        }
    }
}
