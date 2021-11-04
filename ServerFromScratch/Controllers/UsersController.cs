using Server.ItSelf;
using System.Threading;
using System.Threading.Tasks;

namespace ServerFromScratch.Controllers
{
    public record User(string Name, string Surname, string Login);

    public class UsersController : IController
    {
        public User[] Index()
        {
            Thread.Sleep(50);
            return new[]
            {
                new User("Andrii", "Podkolzin", "WORLDKing"),
                new User("Andrii", "Podkolzin", "Zorro"),
                new User("Andrii", "Podkolzin", "podkolzzzin")
            };
        }

        public async Task<User[]> IndexAsync()
        {
            await Task.Delay(50);
            return new[]
            {
                new User("Andrii", "Podkolzin", "WORLDKing"),
                new User("Andrii", "Podkolzin", "Zorro"),
                new User("Andrii", "Podkolzin", "podkolzzzin")
            };
        }
    }
}
