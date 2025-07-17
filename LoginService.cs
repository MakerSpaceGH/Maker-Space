namespace My_own_website.Services
{
    public class LoginManager
    {
        private readonly string _contentRootPath;

        public bool IsLoggedIn { get; private set; } = false;
        public string CurrentUsername { get; private set; } = "";

        public LoginManager(string contentRootPath)
        {
            _contentRootPath = contentRootPath;
        }

        public bool Login(string username, string password)
        {
            var accountsFile = Path.Combine(_contentRootPath, "Data", "accounts");
            if (!File.Exists(accountsFile))
                return false;

            var lines = File.ReadAllLines(accountsFile);
            foreach (var line in lines)
            {
                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    if (parts[0].Trim() == username && parts[1].Trim() == password)
                    {
                        IsLoggedIn = true;
                        CurrentUsername = username;
                        return true;
                    }
                }
            }
            return false;
        }

        public void Logout()
        {
            IsLoggedIn = false;
            CurrentUsername = "";
        }
    }
}
