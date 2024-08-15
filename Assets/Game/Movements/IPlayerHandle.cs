using Game.Player;

namespace Game.Movements
{
    public interface IPlayerHandle
    {
        public void SetPlayer(PlayerController newPlayer);
    }
}