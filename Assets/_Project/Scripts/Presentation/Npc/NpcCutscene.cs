using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Presentation.Npc
{
    public class NpcCutscene : MonoBehaviour
    {
        [SerializeField] private Transform path;
        [SerializeField] private Transform npcSprite;
        [SerializeField] private NpcAnchor anchor;

        private List<Vector3> positions = new List<Vector3>();
        private int positionIndex = 0;
        private float timer = 0;
        private bool moving;
        private float speed;

        public bool IsMoving => moving;
        
        private void Start()
        {
            foreach (Transform child in path)
            {
                positions.Add(child.transform.position);
            }
            if (positions.Count > 0)
            {
                transform.position = positions[0];
            }
        }

        private void Update()
        {
            if (moving)
            {
                timer += speed * Time.deltaTime / Mathf.Max(Vector3.Distance(positions[positionIndex], positions[positionIndex + 1]), 1);
                npcSprite.position = Vector3.Lerp(positions[positionIndex], positions[positionIndex + 1], timer);

                if (anchor != null)
                {
                    // anchor follows npcSprite
                    anchor.HeadPoint.position = npcSprite.position;
                }

                if (timer >= 1)
                {
                    npcSprite.position = positions[positionIndex + 1];
                    positionIndex++;
                    timer = 0;
                    moving = false;
                }
            }
        }

        public void NextPosition(float _speed = 1)
        {
            if (positionIndex < positions.Count - 1)
            {
                speed = _speed;

                timer = 0;
                moving = true;
            }
        }
    }
}
