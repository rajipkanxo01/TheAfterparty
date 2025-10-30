using _Project.Scripts.Application.Clue;

namespace _Project.Scripts.Application.Player
{
    public class PlayerProfileListener
    {
        private readonly PlayerProfile _playerProfile;

        public PlayerProfileListener(PlayerProfile profile)
        {
            _playerProfile = profile;
            ClueEvents.OnClueDiscovered += (clue) => _playerProfile.AddDiscoveredClue(clue.clueId);
        }

        public void Unsubscribe()
        {
            ClueEvents.OnClueDiscovered += (clue) => _playerProfile.AddDiscoveredClue(clue.clueId);
        }
    }
}