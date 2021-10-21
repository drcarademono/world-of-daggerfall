using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DaggerfallWorkshop.Loc
{
    public class LocationPrefabEditor : LocationEditor
    {
        private enum EditMode { EditLocation, ObjectPicker };

        private float magicNumberSca = 0.64f;
        private float magicNumberLoc = -3.2f;

        private GameObject parent, ground;
        private List<GameObject> objScene = new List<GameObject>();

        private string searchField = "";
        private List<string> searchListNames = new List<string>();
        private List<string> searchListID = new List<string>();
        private bool[] availableIDs = new bool[1000];

        private EditMode editMode;
        private int objectPicker, listMode, sublistMode;
        private Vector2 scrollPosition = Vector2.zero, scrollPosition2 = Vector2.zero;
        private string[] listModeName = { "3D Model", "Billboard"};
        private string[] billboardLists = { "All", "Lights", "Treasure"};

        private LocationPrefab locationPrefab;

        [MenuItem("Daggerfall Tools/Location Prefab Editor")]
        static void Init()
        {
            LocationPrefabEditor window = (LocationPrefabEditor)GetWindow(typeof(LocationPrefabEditor));
            window.titleContent = new GUIContent("Location Prefab Editor");
        }

        private void Awake()
        {
            CreataGUIStyles();
            UpdateObjList(); 
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        void OnGUI()
        {
            if (editMode == EditMode.EditLocation)
            {
                if (parent != null && !parent.gameObject.activeSelf)
                    parent.gameObject.SetActive(true);

                EditLocationWindow();
            }
            else if (editMode == EditMode.ObjectPicker)
            {
                if (parent != null && parent.gameObject.activeSelf)
                    parent.gameObject.SetActive(false);

                ObjectPickerWindow();
            }
        }

        private void EditLocationWindow()
        {
            if (GUI.Button(Rect_NewFile, "New Prefab"))
            {
                if (parent != null)
                    DestroyImmediate(parent);

                locationPrefab = new LocationPrefab();
                parent = new GameObject("Location Prefab");
            }

            if (GUI.Button(Rect_SaveFile, "Save Prefab"))
            {
                string path = EditorUtility.SaveFilePanel("Save as", LocationHelper.locationPrefabFolder, "new location", "txt");
                if (path.Length != 0)
                {
                    LocationHelper.SaveLocationPrefab(locationPrefab, path);
                }
            }

            if (GUI.Button(Rect_LoadFile, "Load Prefab"))
            {
                string path = EditorUtility.OpenFilePanel("Open", LocationHelper.locationPrefabFolder, "txt");

                if (path.Length == 0)
                    return;

                locationPrefab = new LocationPrefab();
                objScene = new List<GameObject>();
                locationPrefab = LocationHelper.LoadLocationPrefab(path);

                if (locationPrefab == null)
                    return;

                if (parent != null)
                    DestroyImmediate(parent);

                parent = new GameObject("Location Prefab");

                foreach (LocationPrefab.LocationObject obj in locationPrefab.obj)
                {
                    CreateObject(obj);
                    availableIDs[obj.objectID] = true;
                }
            }

            if (parent != null)
            {
                GUI.Box(new Rect(4, 32, 516, 56), "", lightGrayBG);

                GUI.Label(new Rect(16, 40, 64, 16), "Area Y:");
                locationPrefab.height = EditorGUI.IntSlider(new Rect(90, 40, 400, 16), locationPrefab.height, 1, 126);

                GUI.Label(new Rect(16, 64, 64, 16), "Area X:");
                locationPrefab.width = EditorGUI.IntSlider(new Rect(90, 64, 400, 16), locationPrefab.width, 1, 126);

                scrollPosition = GUI.BeginScrollView(new Rect(2, 128, 532, 512), scrollPosition, new Rect(0, 0, 512, 20 + ((objScene.Count+1) * 60)),false, true);

                for (int i = 0; i < objScene.Count; i++)
                {
                    if (objScene[i] == null)
                    {
                        availableIDs[locationPrefab.obj[i].objectID] = false;
                        objScene.RemoveAt(i);
                        locationPrefab.obj.RemoveAt(i);
                        return;
                    }

                    locationPrefab.obj[i].pos = new Vector3(objScene[i].transform.localPosition.x, objScene[i].transform.localPosition.y, objScene[i].transform.localPosition.z);
                    locationPrefab.obj[i].rot = objScene[i].transform.rotation;
                    locationPrefab.obj[i].scale = objScene[i].transform.localScale;

                    if (Selection.Contains(objScene[i]))
                    {
                        GUI.BeginGroup(new Rect(6, 10 + (i * 60), 496, 52), lightGreenBG);
                    }
                    else
                        GUI.BeginGroup(new Rect(6, 10 + (i * 60), 496, 52), lightGrayBG);

                    GUI.Label(new Rect(2, 4, 128, 16), "" + objScene[i].name);
                    GUI.Label(new Rect(2, 20, 128, 16), "Name: " + locationPrefab.obj[i].name);
                    GUI.Label(new Rect(2, 36, 128, 16), "ID: " + locationPrefab.obj[i].objectID);

                    GUI.Label(new Rect(136, 4, 256, 16), "Position : " + locationPrefab.obj[i].pos);
                    GUI.Label(new Rect(136, 20, 256, 16), "Rotation : " + locationPrefab.obj[i].rot.eulerAngles);
                    GUI.Label(new Rect(136, 36, 256, 16), "Scale    : " + locationPrefab.obj[i].scale);

                    if (GUI.Button(new Rect(392, 20, 64, 16), "Duplicate"))
                    {
                        int newID = 0;

                        for(int j = 0; j < availableIDs.Length; j++)
                        {
                            if(availableIDs[j] == false)
                            {
                                newID = j;
                                availableIDs[j] = true;
                                break;
                            }
                        }

                        locationPrefab.obj.Add(new LocationPrefab.LocationObject(locationPrefab.obj[i].type, locationPrefab.obj[i].name, locationPrefab.obj[i].pos, locationPrefab.obj[i].rot, locationPrefab.obj[i].scale));
                        CreateObject(locationPrefab.obj[locationPrefab.obj.Count - 1], true);
                        locationPrefab.obj[locationPrefab.obj.Count - 1].objectID = newID;
                        //locationPrefab.obj.Sort((a, b) => a.objectID.CompareTo(b.objectID));
                    }

                    GUI.color = new Color(0.9f, 0.5f, 0.5f);
                    if (GUI.Button(new Rect(476, 0, 20, 20), "X") || (Event.current.Equals(Event.KeyboardEvent("Delete")) && Selection.Contains(objScene[i])))
                    {
                        availableIDs[locationPrefab.obj[i].objectID] = false;
                        DestroyImmediate(objScene[i]);
                        objScene.RemoveAt(i);
                        locationPrefab.obj.RemoveAt(i);
                        return;
                    }
                    GUI.color = Color.white;

                    if (GUI.Button(new Rect(0, 0, 758, 64), "", emptyBG))
                    {
                        Selection.activeGameObject = objScene[i];
                    }

                    GUI.EndGroup();
                }

                if (GUI.Button(new Rect(6, 10 + (objScene.Count * 60), 496, 52), "Add New Object"))
                {
                    editMode = EditMode.ObjectPicker;
                }

                GUI.EndScrollView();

                //Make sure we always have a ground
                if (ground == null)
                    ground = GameObject.CreatePrimitive(PrimitiveType.Plane);

                //Always make sure that the ground is set correctly
                ground.transform.SetParent(parent.transform);
                ground.name = "Surface";
                ground.transform.localScale = new Vector3(locationPrefab.width* magicNumberSca, 0, locationPrefab.height * magicNumberSca);
                ground.transform.localRotation = new Quaternion();
                ground.transform.localPosition = new Vector3(-(((locationPrefab.width-1)* magicNumberLoc) + magicNumberLoc), 0, -(((locationPrefab.height - 1) * magicNumberLoc) + magicNumberLoc));
            }
        }

        private void ObjectPickerWindow()
        {
            listMode = GUI.SelectionGrid(new Rect(4, 8, 384, 20), listMode, listModeName, 3);

            if(listMode == 1)
                sublistMode = GUI.SelectionGrid(new Rect(16, 42, 384, 16), sublistMode, billboardLists, 3);

            GUI.Label(new Rect(new Rect(4, 72, 64, 16)), "Search: ");
            searchField = EditorGUI.TextField(new Rect(70, 72, 156, 16), searchField);

            if (GUI.changed)
                UpdateObjList();

            scrollPosition2 = GUI.BeginScrollView(new Rect(4, 96, 256, 472), scrollPosition2, new Rect(0, 0, 236, 20 + (searchListNames.Count * 24)));
            objectPicker = GUI.SelectionGrid(new Rect(10, 10, 216, searchListNames.Count * 24), objectPicker, searchListNames.ToArray(), 1);

            GUI.EndScrollView();

            if (GUI.Button(new Rect(16, 582, 96, 20), "OK"))
            {
                int newID = 0;

                for (int j = 0; j < availableIDs.Length; j++)
                {
                    if (availableIDs[j] == false)
                    {
                        newID = j;
                        availableIDs[j] = true;
                        break;
                    }
                }

                if (listMode == 0)
                    locationPrefab.obj.Add(new LocationPrefab.LocationObject(0, searchListID[objectPicker], Vector3.zero, new Quaternion(), new Vector3(1, 1, 1)));

                else if (listMode == 1)
                    locationPrefab.obj.Add(new LocationPrefab.LocationObject(1, searchListID[objectPicker], Vector3.zero, new Quaternion(), new Vector3(1, 1, 1)));

                locationPrefab.obj[locationPrefab.obj.Count - 1].objectID = newID;
                CreateObject(locationPrefab.obj[locationPrefab.obj.Count - 1], true);
                //locationPrefab.obj.Sort((a, b) => a.objectID.CompareTo(b.objectID));
                editMode = EditMode.EditLocation;
            }

            if (GUI.Button(new Rect(128, 582, 96, 20), "Cancel"))
            {
                editMode = EditMode.EditLocation;
            }
        }

        private void CreateObject(LocationPrefab.LocationObject locationObject, bool selectNew = false)
        {
            if (!LocationHelper.ValidateValue(locationObject.type, locationObject.name))
                return;

            GameObject newObject = LocationHelper.LoadObject(locationObject.type, locationObject.name, parent.transform,
                                     new Vector3(locationObject.pos.x, locationObject.pos.y, locationObject.pos.z),
                                     locationObject.rot,
                                     locationObject.scale, 0, 0
                );

            if (newObject != null)
            {
                objScene.Add(newObject);

                if (locationObject.type == 0)
                    newObject.name = LocationHelper.models[locationObject.name];
                else if (locationObject.type == 1)
                {
                    if(sublistMode == 0)
                        newObject.name = LocationHelper.billboards[locationObject.name];
                    else if (sublistMode == 1)
                        newObject.name = LocationHelper.billboardslights[locationObject.name];
                    else if (sublistMode == 2)
                        newObject.name = LocationHelper.billboardsTreasure[locationObject.name];
                }
                    
                if (newObject.GetComponent<DaggerfallBillboard>())
                {
                    DestroyImmediate(newObject.GetComponent<DaggerfallBillboard>());
                }

                if (selectNew)
                    Selection.activeGameObject = objScene[objScene.Count - 1];
            }
            else
            {
                Debug.LogError("Failed to load object " + name);
            }
        }

        private void UpdateObjList()
        {
            searchListNames.Clear();
            searchListID.Clear();

            if (listMode == 0)
            {
                foreach (KeyValuePair<string, string> pair in LocationHelper.models)
                {
                    if (pair.Value.ToLower().Contains(searchField.ToLower()))
                    {
                        searchListNames.Add(pair.Value);
                        searchListID.Add(pair.Key);
                    }     
                }
            }
            else if (listMode == 1)
            {
                if (sublistMode == 0)
                {
                    foreach (KeyValuePair<string, string> pair in LocationHelper.billboards)
                    {
                        if (pair.Value.ToLower().Contains(searchField.ToLower()))
                        {
                            searchListNames.Add(pair.Value);
                            searchListID.Add(pair.Key);
                        }
                    }
                }
                else if (sublistMode == 1)
                {
                    foreach (KeyValuePair<string, string> pair in LocationHelper.billboardslights)
                    {
                        if (pair.Value.ToLower().Contains(searchField.ToLower()))
                        {
                            searchListNames.Add(pair.Value);
                            searchListID.Add(pair.Key);
                        }
                    }
                }
                else if (sublistMode == 2)
                {
                    foreach (KeyValuePair<string, string> pair in LocationHelper.billboardsTreasure)
                    {
                        if (pair.Value.ToLower().Contains(searchField.ToLower()))
                        {
                            searchListNames.Add(pair.Value);
                            searchListID.Add(pair.Key);
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (parent != null)
                DestroyImmediate(parent.gameObject);
        }
    }
}