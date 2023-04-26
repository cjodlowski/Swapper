using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class laser : MonoBehaviour
{
    LineRenderer line;
    // Start is called before the first frame update

    private void OnEnable()
    {
        line = gameObject.GetComponent<LineRenderer>();
        
    }

    public void SetPositions(Vector3[] positions)
    {
        line.positionCount = positions.Length;
        line.SetPositions(positions);
    }

    public Vector3[] GetPositions()
    {
        var positions = new Vector3[line.positionCount];
        line.GetPositions(positions);
        return positions;
    }

    public void SetColor(Color c)
    {
        line.endColor = c;
        line.startColor = c;
    }
}
