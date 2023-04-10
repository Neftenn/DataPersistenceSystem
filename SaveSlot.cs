using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileID = "";
    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI PercentageCompleteText;
    [SerializeField] private TextMeshProUGUI NotesCollectedText;
    private Button saveSlotButton;

    private void Awake()
    {
        saveSlotButton = this.GetComponent<Button>(); 
    }
    public void SetData(GameData data) {
        
        // there's no data for this profileID
        if (data == null)
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }


        // there is data for this ProfileID
         else 
        {
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

            NotesCollectedText.text = "Notes Collected: " + data.notesCollected;
        }
    }

    public string GetProfileID()
    {
        return this.profileID;
    }
    public void SetInteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
    }
}