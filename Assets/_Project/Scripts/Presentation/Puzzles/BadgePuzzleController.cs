using _Project.Scripts.Presentation.Clues;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class BadgePuzzleController : MonoBehaviour
{
    private Vector2Int[] correctSpots = { new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(3, 1),
                           new Vector2Int(1, 2), new Vector2Int(2, 2), new Vector2Int(3, 2),
                           new Vector2Int(1, 3), new Vector2Int(2, 3), new Vector2Int(3, 3), };
    private Vector2Int[] startSpots = { new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(3, 1),
                           new Vector2Int(3, 2), new Vector2Int(2, 2), new Vector2Int(2, 3),
                           new Vector2Int(1, 2), new Vector2Int(3, 3), new Vector2Int(1, 3), };

    private Vector2Int[] currentSpots = { new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(3, 1),
                           new Vector2Int(3, 2), new Vector2Int(2, 2), new Vector2Int(2, 3),
                           new Vector2Int(1, 2), new Vector2Int(3, 3), new Vector2Int(1, 3), };
    private Vector2Int emptySpot = new Vector2Int(2, 2);

    [SerializeField] private Transform[] pieces;
    private bool done = false;
    [SerializeField] private ClueObject clueObj;
    private float waitTimer = 0f;

    private void Start()
    {
        for (int i = 0; i < pieces.Length; ++i)
        {
            pieces[i].localPosition = PositionFromGridPos(currentSpots[i]);
        }
    }

    private void Update()
    {
        if(done)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= 1f)
            {
                clueObj.FinishedPuzzle();
                gameObject.SetActive(false);
            }
        }
    }

    public void MoveBlock(int blockID)
    {
        if (!done)
        {
            // Adjacency check for the clicked block
            if ((Mathf.Abs(emptySpot.x - currentSpots[blockID].x) == 1) != (Mathf.Abs(emptySpot.y - currentSpots[blockID].y) == 1))
            {
                Vector2Int t = currentSpots[blockID];
                currentSpots[blockID] = emptySpot;
                emptySpot = t;
                pieces[blockID].localPosition = PositionFromGridPos(currentSpots[blockID]);

                // Check if they are in the right position
                bool solved = true;
                for (int i = 0; i < currentSpots.Length; ++i)
                {
                    // middle spot
                    if (i == 4) continue;

                    if (currentSpots[i] != correctSpots[i])
                    {
                        solved = false;
                        break;
                    }
                }
                if (solved)
                {
                    done = true;
                }
            }
        }
    }

    private Vector2 PositionFromGridPos(Vector2Int gridPos)
    {
        return new Vector2(250 * (gridPos.x - 2), 250 * (2 - gridPos.y));
    }

    public void Reset()
    {
        if (!done)
        {
            emptySpot = new Vector2Int(2, 2);
            for (int i = 0; i < pieces.Length; ++i)
            {
                currentSpots[i] = startSpots[i];
                pieces[i].localPosition = PositionFromGridPos(currentSpots[i]);
            }
        }
    }
}
