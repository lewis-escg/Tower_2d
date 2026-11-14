using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGeneration : MonoBehaviour
{
    public GameObject Player;
    public List<GameObject> TowerSegmentVariants;
    public GameObject TowerSource;

    private Transform frontConnector;
    private Transform rearConnector;

    // Start is called before the first frame update
    void Start()
    {
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[1], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[1], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], true);
        AppendSegment(TowerSegmentVariants[0], true);
    }
    
    void Update()
    {
        
    }

    void AppendSegment(GameObject segment, bool rear)
    {
        Transform connector;
        Transform newConnector;
        GameObject newSegment = Instantiate(segment);
        newSegment.transform.parent = TowerSource.transform;

        if (frontConnector == null || rearConnector == null)
        {
            frontConnector = TowerSource.transform;
            rearConnector = TowerSource.transform;
        }
        if (rear == true) // if we selected the rear connector
        {
            connector = rearConnector; // use rear connector
            newConnector = newSegment.transform.Find("Connectors/Front");
        }
        else
        {
            connector = frontConnector; // use front connector
            newConnector = newSegment.transform.Find("Connectors/Rear");
        }

        newSegment.transform.position += connector.position - newConnector.position;
        // set new front or rear connector
        if (rear == true || rearConnector == null) rearConnector = newSegment.transform.Find("Connectors/Rear");
        if (rear == false || frontConnector == null) frontConnector = newSegment.transform.Find("Connectors/Front");
    }
}
