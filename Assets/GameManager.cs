using System.Runtime.CompilerServices;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    public InputActionAsset inputActions;

    private InputAction touchOrClickAction;
    public GameObject arrowObj;
    public GameObject redChecker;
    public GameObject blackChecker;
    public GameObject coordianteGameObject;
    public GameObject[] coordinates;

    public float speed = 1.0f;  // Speed of the Lerp
    private float progress = 0.0f; // Tracks the progress of the Lerp

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PreLoad();
        PlaceChecker("Black", "2B");


        
    }

    // Update is called once per frame
    void Update()
    {   
        Advice("2B", "8H", 1);
        if(Mouse.current.leftButton.isPressed){
            RemoveChecker("Black", "2B");
        }
    }

    void Advice(string checker, string goal, int color){
        GameObject checkerObj = null;
        GameObject goalObj = null;
        GameObject startPos = null;
        GameObject arrowO = null;
        Vector3 arrowPosition = new Vector3();
        for (int i = 0; i < coordinates.Length; i++)
        {
            if (checker.Equals(coordinates[i].name))
            {
                checkerObj = coordinates[i].gameObject.transform.GetChild(color).gameObject;
                startPos = coordinates[i].gameObject;
                arrowO = coordinates[i].gameObject.transform.GetChild(2).gameObject;

                if (!arrowO.activeSelf && checkerObj.activeSelf)
                {
                    arrowO.SetActive(true);
                }
                arrowPosition = coordinates[i].transform.position;
                arrowPosition.y += 0.2f;
            }
            if (goal.Equals(coordinates[i].name))
            {
                goalObj = coordinates[i].gameObject;
            }
        }

        // Increment the progress based on the speed and time
        progress += Time.deltaTime * speed;

        checkerObj.transform.position = Vector3.Lerp(startPos.transform.position, goalObj.transform.position, progress);
        Vector3 arrowGoalPos = goalObj.transform.position;
        arrowGoalPos.y += 0.2f;
        arrowO.transform.position = Vector3.Lerp(arrowPosition, goalObj.transform.position, progress);

        

        // If Lerp is complete (progress >= 1), reset the position
        if (progress >= 1.5f)
        {
            progress = 0.0f; // Reset progress to start over
            checkerObj.transform.position = goalObj.transform.position; // Move back to the starting position
        }
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
    void RemoveChecker(string type, string position)
    {
        for (int i = 0; i < coordinates.Length; i++)
        {
            if (position.Equals(coordinates[i].name))
            {
                if (type.Equals("Red"))
                    coordinates[i].gameObject.transform.GetChild(0).gameObject.SetActive(false);
                else if (type.Equals("Black"))
                    coordinates[i].gameObject.transform.GetChild(1).gameObject.SetActive(false);
            }
            coordinates[i].gameObject.transform.GetChild(2).gameObject.SetActive(false);
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
            Vector3 arrowPos = coordinates[i].transform.position;
            arrowPos.y += 0.2f;

            // Instantiate the arrow object at the new position and set it as a child of the coordinate
            GameObject arrowInstance = Instantiate(arrowObj, arrowPos, coordinates[i].transform.rotation);

            arrowInstance.transform.SetParent(coordinates[i].transform);
            arrowInstance.SetActive(false); // Set the parent to the coordinate
            //coordinates[i].gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}
