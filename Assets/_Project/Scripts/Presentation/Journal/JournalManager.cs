using System.Collections.Generic;
using _Project.Scripts.Data.Clues;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project.Scripts.Presentation.Journal
{
    public class JournalManager : MonoBehaviour
    {
        [SerializeField] private Image journalPanel;
        [SerializeField] private ClueDatabase clueDatabase;
        [SerializeField] private GameObject clueUIPrefab;
        [SerializeField] private GameObject memoryUIPrefab;
        private Vector3 startSpawn = new Vector3(-300, 30, 0);

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HideClues;
            journalPanel.enabled = false;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HideClues;  
        }

        private void HideClues(Scene scene, LoadSceneMode mode)
        {
            journalPanel.enabled = false;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        public void ToggleClues(bool show)
        {
            journalPanel.enabled = show;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(show);
            }
        }

        public void RecreateClues(System.Collections.Generic.IReadOnlyCollection<string> clues, System.Collections.Generic.IReadOnlyCollection<string> memories)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            int i = 0;
            foreach (string clue in clues)
            {
                ClueData clueData = clueDatabase.GetClueById(clue);
                GameObject clueUI = Instantiate(clueUIPrefab, transform.position + startSpawn + new Vector3(i * 400, 0, 0), Quaternion.identity, transform);
                clueUI.GetComponent<JournalClueManager>().SetClueData(clueData);

                ++i;
            }

            foreach(string memory in memories)
            {
                GameObject memoryUI = Instantiate(memoryUIPrefab, transform.position + startSpawn + new Vector3(i * 400, 0, 0), Quaternion.identity, transform);
                memoryUI.GetComponent<JournalMemoryManager>().SetMemoryData(memory);

                ++i;
            }
        }
    }
}