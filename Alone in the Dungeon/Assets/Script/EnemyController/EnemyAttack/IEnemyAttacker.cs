namespace EnemyController
{
    public interface IEnemyAttacker
    {
        bool IsAttacking { get; }
        event System.Action OnAttackStart;
        event System.Action OnAttackEnd;
    }
}