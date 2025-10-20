using System;
using System.Collections.Generic;
using System.IO;

namespace My_own_website.Services
{
    public class LoginManager
    {
        private readonly string _contentRootPath;

        // Aktuell eingeloggter Benutzer
        public string CurrentUsername { get; private set; } = "";

        // Global: alle eingeloggten Benutzer
        private static HashSet<string> LoggedInUsers = new();

        public LoginManager(string contentRootPath)
        {
            _contentRootPath = contentRootPath;
        }

        /// <summary>
        /// Versucht, einen Benutzer einzuloggen.
        /// </summary>
        /// <param name="username">Benutzername</param>
        /// <param name="password">Passwort</param>
        /// <param name="error">Fehlermeldung</param>
        /// <returns>true wenn Login erfolgreich</returns>
        public bool Login(string username, string password, out string error)
        {
            error = "";

            var accountsFile = Path.Combine(_contentRootPath, "Data", "accounts");
            if (!File.Exists(accountsFile))
            {
                error = "Accounts-Datei nicht gefunden.";
                return false;
            }

            var lines = File.ReadAllLines(accountsFile);
            foreach (var line in lines)
            {
                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    if (parts[0].Trim() == username && parts[1].Trim() == password)
                    {
                        // Prüfen, ob Benutzer schon eingeloggt ist
                        if (LoggedInUsers.Contains(username))
                        {
                            error = "Dieser Benutzer ist bereits eingeloggt.";
                            return false;
                        }

                        CurrentUsername = username;
                        LoggedInUsers.Add(username);
                        return true;
                    }
                }
            }

            error = "Benutzername oder Passwort falsch.";
            return false;
        }

        /// <summary>
        /// Loggt den aktuellen Benutzer aus.
        /// </summary>
        public void Logout()
        {
            if (!string.IsNullOrEmpty(CurrentUsername))
            {
                LoggedInUsers.Remove(CurrentUsername);
                CurrentUsername = null;
            }
        }

        /// <summary>
        /// Prüft, ob ein Benutzer aktuell eingeloggt ist.
        /// </summary>
        public bool IsLoggedIn => !string.IsNullOrEmpty(CurrentUsername);

        /// <summary>
        /// Erzwingt Logout aller Benutzer (für Tests oder Neustart).
        /// </summary>
        public void LogoutAll()
        {
            LoggedInUsers.Clear();
            CurrentUsername = null;
        }
    }
}
