

namespace OnlineShoeKing.Repository
{
    public interface IUserRepository
    {
        string Name(string name);
        int UserId(string email);
    }
}
