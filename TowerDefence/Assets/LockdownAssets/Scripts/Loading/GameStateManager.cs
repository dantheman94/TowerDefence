using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;



[System.Serializable]
public class SceneData
{

    public List<Selectable> _Selectables;
    public float _SupplyCount;
    public float _PowerCount;
    public int _Population;
    public int _TechLevel;
    public int _WaveNumber;
    public string _PlayerName;
    public Color _PlayerColor;
}
public class GameStateManager : MonoBehaviour {

    private GameManager _Manager;
    private SceneData _SceneData = new SceneData();
    private string _MapName;
    private Player _Player;
 
    private void Awake()
    {
        
    }

    // Use this for initialization
    void Start () {
        _Manager = GameManager.Instance;
        _Player = _Manager.Players[0];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GatherData()
    {
        _SceneData._Selectables = _Manager.Selectables;
        _SceneData._WaveNumber = _Player.GetWavesSurvived();
        _SceneData._PlayerColor = _Player.TeamColor;
        _SceneData._PlayerName = _Player.Name;
        _SceneData._Population = _Player.PopulationCount;
        _SceneData._PowerCount = _Player.PowerCount;
        _SceneData._SupplyCount = _Player.SuppliesCount;
    }

    public void SaveSceneData()
    {
        GatherData();

        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(Application.persistentDataPath + "/" + _MapName + "/" + _SceneData._PlayerName + ".save"))
        {
            FileStream file = File.Create(Application.persistentDataPath + "/" + _MapName + "/" + _SceneData._PlayerName + 1.ToString() + ".save");
            bf.Serialize(file, _SceneData);
            file.Close();
        }
        else
        {
            FileStream file = File.Create(Application.persistentDataPath + "/" + _MapName + "/" + _SceneData._PlayerName + ".save");
            bf.Serialize(file, _SceneData);
            file.Close();
        }
        Debug.Log(_SceneData._PlayerName + "'s data has been saved to: " + Application.persistentDataPath + "/" + _MapName + "/" + _SceneData._PlayerName);
    }

    /// <summary>
    /// Loads scene data from previous save game to data manager.
    /// </summary>
    public void LoadSceneData()
    {
        if(File.Exists(Application.persistentDataPath + "/" + _MapName + "/" + _SceneData._PlayerName + ".save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + _MapName + "/" + _SceneData._PlayerName + ".save", FileMode.Open);
            _SceneData = (SceneData)bf.Deserialize(file);
            file.Close();
            _Manager.Selectables = _SceneData._Selectables;
            _Player.Name = _SceneData._PlayerName;
            _Player.TeamColor = _SceneData._PlayerColor;
            _Player.SuppliesCount = _SceneData._SupplyCount;
            _Player.PowerCount = _SceneData._PowerCount;

            Debug.Log(_SceneData._PlayerName + "'s data has been loaded");
        }
        else
        {
            Debug.Log("File not found");
        }
    }
}
