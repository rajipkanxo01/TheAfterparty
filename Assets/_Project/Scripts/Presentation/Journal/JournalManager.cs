using System.Collections.Generic;
using _Project.Scripts.Data.Clues;
using UnityEngine;

namespace _Project.Scripts.Presentation.Journal
{
    public class JournalManager : MonoBehaviour
    {
        [SerializeField] private ClueDatabase clueDatabase;
        [SerializeField] private GameObject clueUIPrefab;
        private Vector3 startSpawn = new Vector3(-300, 30, 0);

        public void ToggleClues(bool show)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(show);
            }
        }

        public void RecreateClues(System.Collections.Generic.IReadOnlyCollection<string> clues)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            int i = 0;
            foreach (string clue in clues)
            {
                ClueData clueData = clueDatabase.GetClueById(clue);
                GameObject clueUI = Instantiate(clueUIPrefab, transform.position + startSpawn + new Vector3(i * 200, 0, 0), Quaternion.identity, transform);
                clueUI.GetComponent<JournalClueManager>().SetClueData(clueData);

                ++i;
            }
        }
    }
}