using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 7/7/2018
//
//******************************

public static class ObjectPooling {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private const int _DEFAULT_POOL_SIZE = 3;

    //******************************************************************************************************************************
    //
    //      CLASSES
    //
    //******************************************************************************************************************************

    /// <summary>
    //  Represents the object pool for a particular GameObject.
    /// </summary>
    private class Pool {

        //******************************************************************************************************************************
        //
        //      VARIABLES
        //
        //******************************************************************************************************************************

        private GameObject _GameObject;
        private GameObject _Parent;
        private Stack<GameObject> _POOL_INACTIVE_OBJECTS;

        //******************************************************************************************************************************
        //
        //      FUNCTIONS
        //
        //******************************************************************************************************************************

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        //  Constructor
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="size"></param>
        public Pool(GameObject obj, int size) {

            this._GameObject = obj;
            _POOL_INACTIVE_OBJECTS = new Stack<GameObject>(size);

            _Parent = new GameObject("_POOL_" + obj.name);
            _Parent.transform.position = Vector3.zero;
            _Parent.transform.rotation = Quaternion.identity;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        //  Spawn an object from the pool.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public GameObject Spawn(Vector3 position, Quaternion rotation) {

            GameObject obj;

            if (_POOL_INACTIVE_OBJECTS.Count == 0) {

                // There aren't any objects in the pool to use so create a new one
                obj = GameObject.Instantiate(_GameObject, position, rotation);

                // Add a pool member component so we know what object pool this object belongs to
                obj.AddComponent<PoolMember>().LinkedPool = this;
            }
                       
            else { // (_POOL_INACTIVE_OBJECTS.Count > 0)

                // Get the last object in the array
                obj = _POOL_INACTIVE_OBJECTS.Pop();

                if (obj == null) {

                    // Somehow the object that was to be returned no longer exists 
                    // (IE: Has been destroyed or a scene change, so we try again)
                    return Spawn(position, rotation);
                }
            }

            // Set object's transform
            obj.transform.SetParent(_Parent.transform);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            return obj;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        //  Spawn an object from the pool.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public GameObject Spawn(Vector3 position) {

            GameObject obj;

            if (_POOL_INACTIVE_OBJECTS.Count == 0) {

                // There aren't any objects in the pool to use so create a new one
                obj = Object.Instantiate(_GameObject);

                // Add a pool member component so we know what object pool this object belongs to
                obj.AddComponent<PoolMember>().LinkedPool = this;
            }

            else { // (_POOL_INACTIVE_OBJECTS.Count > 0)

                // Get the last object in the array
                obj = _POOL_INACTIVE_OBJECTS.Pop();

                if (obj == null) {

                    // Somehow the object that was to be returned no longer exists 
                    // (IE: Has been destroyed or a scene change, so we try again)
                    return Spawn(position);
                }
            }

            // Set object's transform
            obj.transform.SetParent(_Parent.transform);
            obj.transform.position = position;
            obj.SetActive(true);
            return obj;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        //  Spawn an object from the pool.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public GameObject Spawn() {

            GameObject obj;

            if (_POOL_INACTIVE_OBJECTS.Count == 0) {

                // There aren't any objects in the pool to use so create a new one
                obj = Object.Instantiate(_GameObject);

                // Add a pool member component so we know what object pool this object belongs to
                obj.AddComponent<PoolMember>().LinkedPool = this;
            }

            else { // (_POOL_INACTIVE_OBJECTS.Count > 0)

                // Get the last object in the array
                obj = _POOL_INACTIVE_OBJECTS.Pop();

                if (obj == null) {

                    // Somehow the object that was to be returned no longer exists 
                    // (IE: Has been destroyed or a scene change, so we try again)
                    return Spawn();
                }
            }

            // Set object's transform
            obj.transform.SetParent(_Parent.transform);
            obj.SetActive(true);
            return obj;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        //  Returns an object back to the pool.
        /// </summary>
        /// <param name="obj"></param>
        public void Despawn(GameObject obj) {

            obj.SetActive(false);
            _POOL_INACTIVE_OBJECTS.Push(obj);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  This is added to newly instantiated objects, so that they 
    //  can be linked back to the correct pool on despawn.
    /// </summary>
    class PoolMember : MonoBehaviour {

        public Pool LinkedPool;
    }

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  A dictionary of all the object pools
    /// </summary>
    static Dictionary<GameObject, Pool> ObjectPools;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Initialize the dictionary
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="size"></param>
    static void Init(GameObject obj = null, int size = _DEFAULT_POOL_SIZE) {

        if (ObjectPools == null) {

            ObjectPools = new Dictionary<GameObject, Pool>();
        }

        if (ObjectPools != null && ObjectPools.ContainsKey(obj) == false) {

            ObjectPools[obj] = new Pool(obj, size);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  If you want to preload a few copies of an object at the start
    //  of a scene, you can use this. Really not needed unless you're
    //  going to go from zero instances to 100+ very quickly.
    //  Could technically be optimized more, but in practice the
    //  Spawn/Despawn sequence is going to be pretty darn quick and
    //  this avoids code duplication.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="size"></param>
    static public void PreLoad(GameObject obj, int size) {

        Init(obj, size);

        // Get reference to all the objects about to be created
        GameObject[] objects = new GameObject[size];
        for (int i = 0; i < size; i++) { objects[i] = Spawn(obj, Vector3.zero, Quaternion.identity); }

        // Despawn all the new objects
        for (int i = 0; i < size; i++) { Despawn(objects[i]); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Spawns a copy of the specified prefab (instantiating one if required).
    //  NOTE: Remember that Awake() or Start() will only run on the very first
    //  spawn and that member variables won't get reset.  OnEnable will run
    //  after spawning -- but remember that toggling IsActive will also
    //  call that function.
    /// </summary>
    /// <param name="Object"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static GameObject Spawn(GameObject Object, Vector3 position, Quaternion rotation) {

        Init(Object);

        return ObjectPools[Object].Spawn(position, rotation);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Spawns a copy of the specified prefab (instantiating one if required).
    //  NOTE: Remember that Awake() or Start() will only run on the very first
    //  spawn and that member variables won't get reset.  OnEnable will run
    //  after spawning -- but remember that toggling IsActive will also
    //  call that function.
    /// </summary>
    /// <param name="Object"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static GameObject Spawn(GameObject Object, Vector3 position) {

        Init(Object);

        return ObjectPools[Object].Spawn(position);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Spawns a copy of the specified prefab (instantiating one if required).
    //  NOTE: Remember that Awake() or Start() will only run on the very first
    //  spawn and that member variables won't get reset.  OnEnable will run
    //  after spawning -- but remember that toggling IsActive will also
    //  call that function.
    /// </summary>
    /// <param name="Object"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static GameObject Spawn(GameObject Object) {

        Init(Object);

        return ObjectPools[Object].Spawn();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Despawns the object and puts it back into its pool.
    /// </summary>
    /// <param name="obj"></param>
    public static void Despawn(GameObject obj) {

        PoolMember poolMember = obj.GetComponent<PoolMember>();

        // Despawn the object through its attached pool
        if (poolMember != null) { poolMember.LinkedPool.Despawn(obj); }

        // The object wasn't spawned via an object pool, so just destroy it normally
        else { GameObject.Destroy(obj, 1f); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}