namespace NInGame
{
    public sealed class Enemy : ACharacter
    {
        public bool Died { get; private set; } = false;

        protected override void OnDied()
        {
            Died |= true;
            base.OnDied();
        }
    }
}