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
        int coordCount = coordianteGameObject.transform.childCount;
        coordinates = new GameObject[coordCount];
        for (int i = 0; i< coordCount; i++){
            coordinates[i] = coordianteGameObject.transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
