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

        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
        AppendSegment(TowerSegmentVariants[0], false);
    }
    
    void Update()
    {
        
    }

    void AppendSegment(GameObject segment, bool rear)
    {
        Transform connector;
        Transform newConnector;
        GameObject newSegment = Instantiate(segment); // instantiate (make a clone) of the segment prefab
        newSegment.transform.parent = TowerSource.transform; // parent it to the tower object 
        newSegment.transform.position = new Vector3(0, 0, newSegment.transform.position.z); // reset position just in case

        if (frontConnector == null || rearConnector == null) // when connectors are null at the start
        {
            frontConnector = TowerSource.transform; // use root as connector
            rearConnector = TowerSource.transform;
        }
        if (rear == true) // if we selected the rear connector
        {
            connector = rearConnector; // use rear connector
            newConnector = newSegment.transform.Find("Connectors/Front"); // get the front connector of the segment we're adding
        }
        else
        {
            connector = frontConnector; // use front connector
            newConnector = newSegment.transform.Find("Connectors/Rear"); // get the rear connector of the segment we're adding
        }

        newSegment.transform.position += connector.position - newConnector.position; // offset the new segment by the difference in positions of their connectors
        // set new front or rear connector
        if (rear == true || rearConnector == null) rearConnector = newSegment.transform.Find("Connectors/Rear"); // if this was a rear segment, update the lowermost connector 
        if (rear == false || frontConnector == null) frontConnector = newSegment.transform.Find("Connectors/Front"); // if this was a front segment, update the uppermost connector
    }
}
