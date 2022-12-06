using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeSpawner : MonoBehaviour
{
    public GameObject challengers;
    private Vector3[] challengerPositions = new []{
            new Vector3(-448,0,94),
            new Vector3(-386,0,94),
            new Vector3(-297,0,1),
            new Vector3(-207,0,1),
            new Vector3(0,0,0),
            new Vector3(120,0,1),
            new Vector3(90,0,38),
            new Vector3(90,0,115),
            new Vector3(90,0,178),
            new Vector3(90,0,382),
            new Vector3(90,0,557),
            new Vector3(121,0,513),
            new Vector3(121,0,458),
            new Vector3(121,0,333),
            new Vector3(-447,0,578),
            new Vector3(-412,0,498),
            new Vector3(-416,0,478),
            new Vector3(-413,0,358),
            new Vector3(-384,0,318),
            new Vector3(-387,0,257),
            new Vector3(-387,0,195),
            new Vector3(-359,0,140),
            new Vector3(-297,0,75),
            new Vector3(-237,0,102),
            new Vector3(-212,0,79),
            new Vector3(-89,0,81),
            new Vector3(0,0,138),
            new Vector3(60,0,140),
            new Vector3(33,0,338),
            new Vector3(0,0,377),
            new Vector3(-32,0,517),
            new Vector3(-60,0,577),
            new Vector3(26,0,536),
            new Vector3(-177,0,377),
            new Vector3(-270,0,377),
            new Vector3(-299,0,295),
            new Vector3(-298,0,235),
            new Vector3(-178,0,258),
            new Vector3(-88,0,300),
            new Vector3(-30,0,297),
            new Vector3(0,0,274),
            new Vector3(-269,0,179),
            new Vector3(-147,0,313) 
        };

    void Awake()
    {
        challengerPositions = shuffle(challengerPositions);
        for(int i=1; i<=35; i++){
            challengers.transform.GetChild(i).transform.localPosition = challengerPositions[i];
            if(i <= 6){
                challengers.transform.GetChild(i).GetComponent<Challenger>().points = 75;
            }
            else if(i <= 18){
                challengers.transform.GetChild(i).GetComponent<Challenger>().points = 150;
            }
            else if(i <= 30){
                challengers.transform.GetChild(i).GetComponent<Challenger>().points = 250;
            }
            else{
                challengers.transform.GetChild(i).GetComponent<Challenger>().points = 500;
            }
        }
    }

    private Vector3[] shuffle(Vector3[] positions){
        for(int i=0; i < positions.Length; i++){
            Vector3 temp = positions[i];
            int r = Random.Range(i, positions.Length);
            positions[i]= positions[r];
            positions[r]= temp;
        }
        return positions;
    }
}