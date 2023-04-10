using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private bool overrideSelectedProfileID = false;
    [SerializeField] private string testSelectedProfileID = "test";

    [Header("File Storage Config")]
    
    [SerializeField] private string fileName;

    private GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;

    private FileDataHandler dataHandler;

    private string selectedProfileID = "";

    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(this.gameObject);

        if (disableDataPersistence)
        {
            Debug.LogWarning("Data Persistence is currently disabled!");
        }


        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);

        this.selectedProfileID = dataHandler.GetMostRecentlyUpdatedProfileID();
        if (overrideSelectedProfileID)
        {
            this.selectedProfileID = testSelectedProfileID;
            Debug.LogWarning("Overrode selected profile ID with test ID: " + testSelectedProfileID);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded Called");
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }



    public void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("OnSceneUnloaded Called");
        SaveGame();
    }


    public void ChangeSelectedProfileID(string newProfileID)
    {
        //update the profile to use for saving and loading
        this.selectedProfileID = newProfileID;
        LoadGame();
    }



    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        //return immediately if data persistence is disabled
        if (disableDataPersistence)
        {
            return;
        }
        // Load any saved data from a file using the data handler.
        this.gameData = dataHandler.Load(selectedProfileID);

        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }


        // if no data can be loaded, initialize to a new game
        if (this.gameData == null)
        {
            Debug.Log("No data was found. A New Game needs to be started before data can be loaded");
            return;
            
        }
        //  push the Loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
        Debug.Log("Loaded F1 Presses = " + gameData.textCounter);
    }

    public void SaveGame()
    {
        //return immediately if data persistence is disabled
        if (disableDataPersistence)
        {
            return;
        }
        // if we don't have any data to save, Log a warning here
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be loaded");
            return;
        }
        //  pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData); 
        }
        // timestamp the data so we know when it was last saved
        gameData.lastUpdated = System.DateTime.Now.ToBinary();
        // save that data to a file using the data handler   
        dataHandler.Save(gameData, selectedProfileID);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }



    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
    
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
       
        
        return new List<IDataPersistence>(dataPersistenceObjects);
    
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }

}