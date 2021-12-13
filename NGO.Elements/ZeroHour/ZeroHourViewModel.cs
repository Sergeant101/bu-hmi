using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Monokot.Hmi.Core.Annotations;
using Monokot.Hmi.Core.Utils;
using Monokot.Hmi.Wpf.Utils;
using Newtonsoft.Json;

namespace NGO.Elements.ZeroHour
{
    public class ZHConfig
    {
        public long t { get; set; } // total ms
        public int ttm { get; set; } // time to message, minutes
        public int tte { get; set; } // time to end, minutes
        public string ac { get; set; } // activation code, 648819578 hash: 3aeaa69debc60e0b81940c216c44eb83
        public string ec { get; set; } // entered code
        public int sps { get; set; } // показывать сообщение раз в 2 часа, если время пришло
    }

    public class ZeroHourViewModel : INotifyPropertyChanged
    {
        private Visibility _messageVisibility = Visibility.Collapsed;
        private readonly ZHConfig _config;
        private Visibility _hmiTabVisibility = Visibility.Visible;
        public event PropertyChangedEventHandler PropertyChanged;

        private int _tmpCounter = 0, _spsCounter;
        private long _latestUptime;
        private bool _isValidCode;
        private string _enteredCode;
        private readonly string _workDirectory;
        private bool _isTimeToMessage;
        private string _message;
        private bool _isHidden;
        private const string CONFIG_FILE_NAME = "dotnetconfig";
        private const int SAVE_PER_SECOND = 30;
        private readonly object _locker = new object();

        private const string MESSAGE = "ВВЕДИТЕ АКТИВАЦИОННЫЙ КОД, ИНАЧЕ БУРОВАЯ УСТАНОВКА БУДЕТ ОСТАНОВЛЕНА ЧЕРЕЗ {0:00} ДНЕЙ {1:00}:{2:00}:{3:00}";

        public ZeroHourViewModel()
        {
            HideCommand = new RelayCommand(a =>
            {
                _isHidden = true;
                _spsCounter = 0;
                UpdateMessageVisibility();
            });

            // Вычисляем путь до папки дата
            // ReSharper disable once AssignNullToNotNullAttribute
            _workDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "data");
            var cfgPath = Path.Combine(_workDirectory, CONFIG_FILE_NAME);

            try
            {
                // Пытаемся создать или загрузить конфигу
                //            if (File.Exists(cfgPath))
                //            {
                _config = JsonConvert.DeserializeObject<ZHConfig>(File.ReadAllText(cfgPath));
                ValidateCode();
                //            }
                //            else
                //            {
                //                // ReSharper disable once UseObjectOrCollectionInitializer
                //                _config = new ZHConfig();
                //                _config.ttm = 30*24*60; // TimeToMessage
                //                _config.tte = 45*24*60; // TimeToEnd
                //                _config.ac = "3aeaa69debc60e0b81940c216c44eb83"; // ActivationCode
                //                File.WriteAllText(cfgPath, JsonConvert.SerializeObject(_config));
                //            }
            }
            catch
            {
                _config = JsonConvert.DeserializeObject<ZHConfig>(File.ReadAllText(cfgPath + BACKUP));
                ValidateCode();
            }

            // Запускаем главный таймер
            var mainTimer = new Timer(1000);
            mainTimer.Elapsed += MainTimerElapsed;
            mainTimer.Start();

            // Запускаем вспомогательный таймер
            var hiddenTimer = new Timer(1000);
            hiddenTimer.Elapsed += HiddenTimerOnElapsed;
            hiddenTimer.Start();
        }

        #region Методы

        private string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        private void ValidateCode()
        {
            if (_config.ec != null)
            {
                var str = CalculateMD5Hash(_config.ec);
                _isValidCode = _config.ac == str;

                if (_isValidCode)
                {
                    SaveConfig();
                    MessageVisibility = Visibility.Collapsed;
                    HmiTabVisibility = Visibility.Visible;
                }
            }
        }

        private const string RANDOM_FILE_NAME_PREFIX = "dotnetconfig_";

        const string BACKUP = ".backup";

        private void SaveConfig()
        {
            lock (_locker)
            {
                try
                {
                    var cfgPath = Path.Combine(_workDirectory, CONFIG_FILE_NAME);
                    var tempFilePath = Path.Combine(_workDirectory, Path.GetRandomFileName());
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_config));

                    using (var fs = new FileStream(cfgPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 8, FileOptions.WriteThrough))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Flush();
                    }

                    // 5 sek chtobi ne pobilis' oba faila v sluche otl;uchki pitalova
                    System.Threading.Thread.Sleep(5000);

                    using (var fs = new FileStream(cfgPath + BACKUP, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 8, FileOptions.WriteThrough))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Flush();
                    }
                }
                catch
                {
                    // Ignore
                }
            }
        }

        private void UpdateMessageVisibility()
        {
            if (!_isValidCode && _isTimeToMessage && !_isHidden)
                MessageVisibility = Visibility.Visible;
            else MessageVisibility = Visibility.Collapsed;
        }

        private void HiddenTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (_spsCounter >= _config.sps)
            {
                _isHidden = false;
                UpdateMessageVisibility();
                _spsCounter = 0;
            }
            else
            {
                _spsCounter++;
            }
        }

        private void MainTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_isValidCode)
            {
                HmiTabVisibility = Visibility.Visible;
                _isTimeToMessage = false;
                UpdateMessageVisibility();
                return;
            }

            // Добавляем к прошедшему времени миллисекунды
            var delta = DateTimeUtils.UptimeMilliseconds - _latestUptime;
            _config.t += delta;
            _latestUptime = DateTimeUtils.UptimeMilliseconds;

            if (_tmpCounter >= SAVE_PER_SECOND)
            {
                _tmpCounter = 0;
                SaveConfig();
            }
            else _tmpCounter++;

            // Проверяем не пора ли закрыть программу
            var elpased = TimeSpan.FromMilliseconds(_config.t);
            if (elpased.TotalMinutes > _config.tte)
            {
                HmiTabVisibility = Visibility.Collapsed;
                SaveConfig();
            }

            // Проверяем не пора ли показать сообщение 
            if (elpased.TotalMinutes > _config.ttm)
            {
                var timeToEnd = TimeSpan.FromMinutes(_config.tte) - TimeSpan.FromMilliseconds(_config.t);

                if (timeToEnd.TotalMilliseconds < 0)
                    timeToEnd = new TimeSpan();

                Message = string.Format(MESSAGE,timeToEnd.Days, timeToEnd.Hours, timeToEnd.Minutes, timeToEnd.Seconds);
                _isTimeToMessage = true;
                UpdateMessageVisibility();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Свойства

        public ICommand HideCommand { get; private set; }

        public Visibility MessageVisibility
        {
            get { return _messageVisibility; }
            private set
            {
                _messageVisibility = value;
                RaisePropertyChanged("MessageVisibility");
            }
        }

        public Visibility HmiTabVisibility
        {
            get { return _hmiTabVisibility; }
            private set
            {
                _hmiTabVisibility = value;
                RaisePropertyChanged("HmiTabVisibility");
            }
        }

        public string EnteredCode
        {
            get { return _enteredCode; }
            set
            {
                _enteredCode = value;
                _config.ec = value;
                ValidateCode();
                RaisePropertyChanged("EnteredCode");
            }
        }

        public string Message
        {
            get { return _message; }
            private set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }

        #endregion
    }
}
