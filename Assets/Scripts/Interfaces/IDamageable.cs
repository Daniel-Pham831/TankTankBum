public interface IDamageable : IDieable
{
    public float Health { get; set; }
    public void TakeDamage(float damage);
}
