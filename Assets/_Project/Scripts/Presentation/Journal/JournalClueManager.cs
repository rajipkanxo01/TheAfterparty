using _Project.Scripts.Data.Clues;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Project.Scripts.Presentation.Journal
{
    public class JournalClueManager : MonoBehaviour
    {
        private Vector2 cluePosition;
        private Vector2 detailPosition = new Vector2(-235, 30);

        [SerializeField] private GameObject detailObj;

        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI notesText;

        private void Start()
        {
            cluePosition = transform.localPosition;
            ExitDetailed();
        }

        public void SetClueData(ClueData clueData)
        {
            image.sprite = clueData.clueIcon;
            nameText.text = clueData.clueName;
            descriptionText.text = clueData.description;
            notesText.text = "";
            foreach (string note in clueData.detectiveNotes)
            {
                notesText.text += "- " + note + '\n';
            }
        }

        public void EnterDetailed()
        {
            transform.localPosition = detailPosition;
            detailObj.SetActive(true);
        }

        public void ExitDetailed()
        {
            transform.localPosition = cluePosition;
            detailObj.SetActive(false);
        }
    }
}