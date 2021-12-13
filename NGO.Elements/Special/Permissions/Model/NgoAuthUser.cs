using System;

namespace NGO.Elements.Special.Permissions.Model
{
    public class NgoAuthUser
    {
        public NgoAuthUser(string login, string password)
        {
            if (string.IsNullOrEmpty(login)) 
                throw new ArgumentException("Bad login", "login");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Bad password", "password");

            this.login = login;
            this.password = password;
        }

        #region Свойства

        public string login { get; private set; }

        public string password { get; private set; }

        #endregion
    }
}
