using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Loc
{
    /// <summary>
    /// Holds data locationPrefab
    /// </summary>
    public class LocationPrefab
    {
        public int height = 8;
        public int width = 8;
        public List<LocationObject> obj = new List<LocationObject>();

        public void AddObject(int objType, string id, Vector3 pos, Quaternion rot, Vector3 scale)
        {
            obj.Add(new LocationObject(objType, id, pos, rot, scale));
        }
        //Location objects
        public class LocationObject
        {
            public int type = 0; //0 == Mesh, 1 == Billbord
            public string name = ""; 
            public int objectID = 0;
            public Vector3 pos = Vector3.zero;
            public Quaternion rot = Quaternion.Euler(0, 0, 0);
            public Vector3 scale = new Vector3(1,1,1);

            public LocationObject()
            {

            }

            public LocationObject(int type, string name, Vector3 pos, Quaternion rot, Vector3 scale)
            {
                this.type = type;
                this.name = name;
                this.pos = pos;
                this.rot = rot;
                this.scale = scale;
            }
        }
    }
    /// <summary>
    /// Holds data for locationInstances
    /// </summary>
    public class LocationInstance
    {
        public int locationID;
        public string name = "";
        public int type;
        public string prefab = "";
        public int worldX = 0;
        public int worldY = 0;
        public int terrainX = 0;
        public int terrainY = 0;
        
        public LocationInstance()
        {
           
        }

        public LocationInstance(string name, int type, string prefab, int worldX, int worldY, int terrainX, int terrainY)
        {
            this.type = type;
            this.name = name;
            this.prefab = prefab;
            this.worldX = worldX;
            this.worldY = worldY;
            this.terrainX = terrainX;
            this.terrainY = terrainY;
        }

        public void UpdateLocationID()
        {
            locationID = Random.Range(0, 99999999);                      
        }
    }
}