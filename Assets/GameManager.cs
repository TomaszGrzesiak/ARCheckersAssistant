using System.Runtime.CompilerServices;
using Unity.XR.CoreUtils;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject redChecker;
    public GameObject blackChecker;
    public GameObject coordianteGameObject;
    public GameObject[] coordinates;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PreLoad();
        PlaceChecker("Black", "2B");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void PlaceChecker(string type, string position){
        for (int i = 0; i < coordinates.Length; i++)
        {
            if(position.Equals(coordinates[i].name)){
                if (type.Equals("Red"))
                coordinates[i].gameObject.transform.GetChild(0).gameObject.SetActive(true);
                else if (type.Equals("Black"))
                coordinates[i].gameObject.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    void PreLoad(){
        int coordCount = coordianteGameObject.transform.childCount;
        coordinates = new GameObject[coordCount];
        for (int i = 0; i < coordCount; i++)
        {
            coordinates[i] = coordianteGameObject.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < coordinates.Length; i++)
        {
            Instantiate(redChecker, coordinates[i].transform.gameObject.transform);
            coordinates[i].gameObject.transform.GetChild(0).gameObject.SetActive(false);
            Instantiate(blackChecker, coordinates[i].transform.gameObject.transform);
            coordinates[i].gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}
