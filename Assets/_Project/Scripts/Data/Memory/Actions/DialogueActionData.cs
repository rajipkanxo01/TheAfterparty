using UnityEngine;

namespace _Project.Scripts.Data.Memory.Actions
{
    [CreateAssetMenu(fileName = "DialogueAction", menuName = "Game/Memory/Actions/Dialogue", order = 1)]
    public class DialogueActionData : ActionBaseData
    {
        public string actorId;
        public string yarnNodeName;
    }
}