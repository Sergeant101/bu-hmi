using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows.Input;
using System.Windows.Markup;
using Monokot.Hmi.Core.Annotations;
using Monokot.Hmi.Core.Infrastructure.Collections;
using Monokot.Hmi.Core.Utils;
using Monokot.Hmi.Core.Utils.Log;
using Monokot.Hmi.Wpf.Utils;

[assembly: XmlnsDefinition("http://schemas.uralmash.com/ngo/", "NGO.Elements.Special.Permissions.Model")]
namespace NGO.Elements.Special.Permissions.Model
{   
    public class NgoAuth : INotifyPropertyChanged
    {
        private static NgoAuth _auth;
        private readonly List<NgoAuthUser> _users = new List<NgoAuthUser>();
        private readonly ObservableDictionary<string, NgoAuthState> _states = new ObservableDictionary<string, NgoAuthState>();

        private string _login;
        private string _password;
        private bool _isAuth;
        
        public event PropertyChangedEventHandler PropertyChanged;

        public class NgoAuthState : INotifyPropertyChanged
        {
            private readonly NgoAuthUser _user;
            private bool _isAuth;
            public event PropertyChangedEventHandler PropertyChanged;

            public NgoAuthState([NotNull] NgoAuthUser user)
            {
                if (user == null) 
                    throw new ArgumentNullException("user");

                _user = user;
            }

            public bool isAuth
            {
                get { return _isAuth; }
                internal set
                {
                    if (value == _isAuth) return;
                    _isAuth = value;
                    onPropertyChanged("isAuth");
                }
            }

            [NotifyPropertyChangedInvocator]
            protected virtual void onPropertyChanged(string property_name)
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(property_name));
            }
        }

        public NgoAuth()
        {
            loginCommand = new RelayCommand(loginHandler);
            logoutCommand = new RelayCommand(logoutHandler);
        }        

        #region Методы

        private void logoutHandler(object obj)
        {
            if (isAuth)
            {
                states[currentUser.login].isAuth = false;
                currentUser = null;
                isAuth = false;
            }
        }

        private void loginHandler(object obj)
        {
            var usr = _users.FirstOrDefault(x => x.login == login && x.password == password);

            // Если в коллекции есть юзер с нужным логином и паролем, то
            // логинимся, выставляем юзера и флаг isAuth
            if (usr != null)
            {
                currentUser = usr;
                states[usr.login].isAuth = isAuth = true;
            }
            else
            {
                throw new SecurityException("Invalid login or password");
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void onPropertyChanged(string property_name)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(property_name));
        }

        public void loadFromFile(string path)
        {
            _users.Clear();

            var lines = File.ReadAllLines(path);
            // 1 строка логин, 2-ая пароль
            try
            {
                foreach (var line in lines)
                {
                    string usr = null, pwd = null;
                    if (string.IsNullOrEmpty(line))
                        continue;

                    var split = line.Split(';', '=');
                    for (var i = 0; i < split.Length; i += 2)
                    {
                        if (split[i].ToLower() == "usr")
                            usr = split[i + 1];
                        else if (split[i].ToLower() == "pwd")
                            pwd = split[i + 1];
                    }

                    var user = new NgoAuthUser(usr, pwd);
                    _users.Add(user);
                    _states.Add(user.login, new NgoAuthState(user));
                }
            }
            catch
            {
                LogUitls.Report(this, "NgoAuth", "Bad config file", MessageType.Error);
            }
        }

        #endregion

        #region Свойства

        public static NgoAuth auth
        {
            get
            {
                if (_auth == null)
                    _auth = new NgoAuth();

                return _auth;
            }
        }

        public ObservableDictionary<string, NgoAuthState> states
        {
            get { return _states; }
        }

        public string login
        {
            get { return _login; }
            set
            {
                if (value == _login) return;
                _login = value;
                onPropertyChanged("login");
            }
        }

        public string password
        {
            get { return _password; }
            set
            {
                if (value == _password) return;
                _password = value;
                onPropertyChanged("password");
            }
        }

        public bool isAuth
        {
            get { return _isAuth; }
            private set
            {
                if (value == _isAuth) return;
                _isAuth = value;
                onPropertyChanged("isAuth");
            }
        }

        public NgoAuthUser currentUser { get; private set; }

        public ICommand loginCommand { get; private set; }

        public ICommand logoutCommand { get; private set; }

        #endregion
    }
}
