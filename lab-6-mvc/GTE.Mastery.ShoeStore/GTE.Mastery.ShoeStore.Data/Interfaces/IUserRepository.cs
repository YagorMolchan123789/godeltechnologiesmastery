namespace GTE.Mastery.ShoeStore.Data.Interfaces
{
    public interface IUserRepository
    {
        bool UserExists(string email);
    }
}
