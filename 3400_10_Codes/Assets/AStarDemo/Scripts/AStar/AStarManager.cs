using UnityEngine;
using System.Collections;

public class AStarManager : MonoBehaviour 
{
    #region Singleton
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static AStarManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static AStarManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AStarManager object in the scene.
                s_Instance = FindObjectOfType(typeof(AStarManager)) as AStarManager;
                if (s_Instance == null)
                    Debug.Log("Could not locate an AStarManager object. \n You have to have exactly one AStarManager in the scene.");
            }
            return s_Instance;
        }
    }

    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit()
    {
        s_Instance = null;
    }
    #endregion

    public Node startNode { get; set; }
    public Node goalNode { get; set; }

	// Use this for initialization
	void Start () 
    {
	}

    public ArrayList FindPath(Vector3 startPos, Vector3 endPos)
    {
        //Assign StartNode and Goal Node
        startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(startPos)));
        goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(endPos)));

        return AStar.FindPath(startNode, goalNode);
    }
}
                                                                                                                                                                                                                   