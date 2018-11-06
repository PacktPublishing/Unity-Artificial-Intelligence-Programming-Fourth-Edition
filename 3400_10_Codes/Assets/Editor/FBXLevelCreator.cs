
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class FBXLevelCreator : EditorWindow
{
    struct ObjectTransform
    {
        public string ID;
        public List<Vector3> position;
        public List<Quaternion> rotation;
        public List<Vector3> scale;
    }

    //Level Designing Stuffs
    private Transform[] objectTypes;
    private Transform levelSource;
    private List<ObjectTransform> totalObjectTransforms;
    private bool bLevelLoaded = false;
    private bool bAutoSearch = true;

    private string objName = "Object";
    private string prefabPath01 = "Prefabs/Misc/";
    private string prefabPath02 = "Prefabs/Bamboo/Models/";
    private string prefabPath03 = "Prefabs/Forest/Models/";
    private string prefabPath04 = "Prefabs/Pillar/Models/";
    private string lastCharacterToSplit = "0123456789 "; 

    /// <summary>
    /// Window drawing operations
    /// </summary>
    void OnGUI()
    {
        if (bLevelLoaded)
        {
            ShowLevelDesign();
        }
        else
        {
            ShowRenameLayout();
            LoadLevelFile();
        }
    }

    private void LoadLevelFile()
    {
        //Level Path Text Field
        bAutoSearch = GUILayout.Toggle(bAutoSearch, "Auto Search Prefabs with the same names");
        prefabPath01 = EditorGUILayout.TextField("Path 01 : ", prefabPath01);
        prefabPath02 = EditorGUILayout.TextField("Path 02 : ", prefabPath02);
        prefabPath03 = EditorGUILayout.TextField("Path 03 : ", prefabPath03);
        prefabPath04 = EditorGUILayout.TextField("Path 04 : ", prefabPath04);
        GUILayout.Label("Load the Level from Transform.", EditorStyles.boldLabel);
        levelSource = EditorGUILayout.ObjectField(levelSource, typeof(Transform)) as Transform;

        //levelPath = EditorGUILayout.TextField(levelPath);
        EditorGUILayout.Space();

        //Load and store the list of object's position in the array list
        if (GUILayout.Button("Load"))
        {
            ProcessLevel();
            bLevelLoaded = true;
        }
    }

    private void ShowRenameLayout()
    {
        //Rename Selected Object to One unique name
        GUILayout.Label("Rename Selected Objects.", EditorStyles.boldLabel);
        objName = GUILayout.TextField(objName);


        //levelPath = EditorGUILayout.TextField(levelPath);
        EditorGUILayout.Space();

        if (GUILayout.Button("Rename Objects"))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                if (obj.transform.childCount > 0)
                {
                    //Also look in the child transform
                    foreach (Transform child in obj.transform)
                    {
                        child.gameObject.name = objName;
                    }
                }
                else
                {
                    obj.name = objName;
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.Label("Type the last character to split", EditorStyles.boldLabel);
        lastCharacterToSplit = GUILayout.TextField(lastCharacterToSplit);

        //Automically Rename Objects which are just differennce in numbers
        if (GUILayout.Button("Intelligent Rename Objects"))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                if (obj.transform.childCount > 0)
                {
                    //Also look in the child transform
                    foreach (Transform child in obj.transform)
                    {
                        string[] strList;
                        string strName = child.gameObject.name;
                        strList = strName.Split(lastCharacterToSplit.ToCharArray());
                        child.gameObject.name = strList[0];
                    }
                }
                else
                {
                    obj.name = objName;
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }

    //Show the design of the current level....
    private void ShowLevelDesign()
    {
        GUILayout.Label("Game Level");

        //Load and store the list of object's position in the array list
        if (GUILayout.Button("Build Level"))
        {
            Build();
            Debug.Log("Level Built");
        }

        if (GUILayout.Button("Go Back"))
        {
            bLevelLoaded = false;
        }

        //Create the object field for each object types exported from Max file
        for (int i = 0; i < totalObjectTransforms.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(totalObjectTransforms[i].ID + " : ");
            objectTypes[i] = EditorGUILayout.ObjectField(objectTypes[i], typeof(Transform)) as Transform;
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
    }

    //Build the actual Game Level in Unity based on the position value of the max
    private void Build()
    {
        GameObject objLevel = new GameObject("GameLevel");
        objLevel.transform.position = new Vector3(0, 0, 0);
        for (int i = 0; i < totalObjectTransforms.Count; i++)
        {
            //Create a new Parent Game Object for each Type
            if (totalObjectTransforms[i].position.Count > 0 && objectTypes[i]) //Create only it has more than 1 object
            {
                GameObject objLevelName = new GameObject(totalObjectTransforms[i].ID);
                objLevelName.transform.position = new Vector3(0, 0, 0);

                for (int j = 0; j < totalObjectTransforms[i].position.Count; j++)
                {
                    if (objectTypes[i] != null)
                    {
                        Transform obj = PrefabUtility.InstantiatePrefab(objectTypes[i]) as Transform;
                        obj.position = totalObjectTransforms[i].position[j];
                        obj.rotation = totalObjectTransforms[i].rotation[j];
                        obj.localScale = totalObjectTransforms[i].scale[j];

                        if (objLevelName)
                            obj.transform.parent = objLevelName.transform;
                    }
                    else
                    {
                        Debug.Log(totalObjectTransforms[i].ID + " : Transforms missing");
                        break;
                    }
                }

                //Then make a child of the GameLevel Object
                if (objLevel)
                    objLevelName.transform.parent = objLevel.transform;
            }
        }
    }

    private void ProcessLevel()
    {
        //Calculate number of Different Objects include in the scene
        totalObjectTransforms = new List<ObjectTransform>();

        levelSource = Selection.activeGameObject.transform;

        foreach (Transform child in levelSource)
        {
            string name = child.name;
            ObjectTransform tmpObjectTransform;

            int indexFound = totalObjectTransforms.FindIndex(delegate(ObjectTransform _mo)
            {
                if (_mo.ID == name)
                {
                    return true;
                }
                return false;
            });

            tmpObjectTransform.ID = name;
            //Debug.Log(name);

            //Store Rotation and Scale Value
            tmpObjectTransform.position = new List<Vector3>();
            tmpObjectTransform.rotation = new List<Quaternion>();
            tmpObjectTransform.scale = new List<Vector3>();

            tmpObjectTransform.position.Add(child.position);
            tmpObjectTransform.rotation.Add(child.rotation);
            tmpObjectTransform.scale.Add(child.localScale);


            if (indexFound < 0)
            {
                totalObjectTransforms.Add(tmpObjectTransform);
            }
            else
            {
                totalObjectTransforms[indexFound].position.Add(child.position);
                totalObjectTransforms[indexFound].rotation.Add(child.rotation);
                totalObjectTransforms[indexFound].scale.Add(child.localScale);
            }
        }

        //Create the GameObject type for each FBX ObjectTypes
        //Need to be an Transform Type as GameObject Type just don't work
        objectTypes = new Transform[totalObjectTransforms.Count];

        if (bAutoSearch)
        {
            //Automatically load the same name from prefabs if have one
            for (int i = 0; i < totalObjectTransforms.Count; i++)
            {
                string path01 = prefabPath01 + totalObjectTransforms[i].ID;
                string path02 = prefabPath02 + totalObjectTransforms[i].ID;
                string path03 = prefabPath03 + totalObjectTransforms[i].ID;
                string path04 = prefabPath04 + totalObjectTransforms[i].ID;

                string[] paths = { path01, path02, path03, path04};

                GameObject prefabObj = (GameObject)Resources.Load(paths[0]);
                for (int j = 0; j < 4; j++)
                {
                    prefabObj = (GameObject)Resources.Load(paths[j]);

                    if (prefabObj)
                    {
                        objectTypes[i] = prefabObj.transform;
                        Debug.Log(totalObjectTransforms[i].ID + " : Loaded Successfully");
                        break;
                    }
                }

                Debug.Log(" No Prefabs at these path " + prefabPath01 + prefabPath02 + totalObjectTransforms[i].ID);
            }
        }
        //Debug.Log(objectTypes.Length);
    }

    /// <summary>
    /// Retrives the TransformUtilities window or creates a new one
    /// </summary>
    [MenuItem("LevelCreation/FBXLevelCreator %F")]
    static void Init()
    {
        FBXLevelCreator window = (FBXLevelCreator)EditorWindow.GetWindow(typeof(FBXLevelCreator));
        window.Show();
    }
}