public interface IFireable
{
    public float RateOfFire { get; set; }
    public float ProjectileSpeed { get; set; }
    public void Fire();
}
