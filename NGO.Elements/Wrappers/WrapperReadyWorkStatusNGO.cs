using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SCADAExtension8.Core.Dictionaries;
using SCADAExtension8.Core.SCADAE8;
using SCADAExtension8.Core.Wrappers;
using SCADAExtension8.Design.Editors.ExtPropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NGO.Elements.Wrappers
{
    public static class STANDART
    {
        public static ObservableCollection<ReadyWorkStatusItem> _STATUS_CODES = new ObservableCollection<ReadyWorkStatusItem>()
        {
            new ReadyWorkStatusItem() {code = 2, text = "СТОП"},
            new ReadyWorkStatusItem() {code = 3, text = "СТОП"},
            new ReadyWorkStatusItem() {code = 64, text = "СТОП"},
            new ReadyWorkStatusItem() {code = 2, text = "СТОП"},
            new ReadyWorkStatusItem() {code = 202, text = "СТОП"},
            new ReadyWorkStatusItem() {code = 203, text = "СТОП"},
            new ReadyWorkStatusItem() {code = 264, text = "СТОП"},
            new ReadyWorkStatusItem() {code = 402, text = "СТОП"},
            new ReadyWorkStatusItem() {code = 802, text = "СТОП"},
            new ReadyWorkStatusItem() {code = 2602, text = "СТОП"},

            new ReadyWorkStatusItem() {code = 1, text = "ГОТОВ"},
            new ReadyWorkStatusItem() {code = 201, text = "ГОТОВ"},
            new ReadyWorkStatusItem() {code = 401, text = "ГОТОВ"},
            new ReadyWorkStatusItem() {code = 801, text = "ГОТОВ"},
            new ReadyWorkStatusItem() {code = 2601, text = "ГОТОВ"},

            new ReadyWorkStatusItem() {code = 1101, text = "РАБОТА"},
            new ReadyWorkStatusItem() {code = 1102, text = "РАБОТА"},
            new ReadyWorkStatusItem() {code = 1103, text = "РАБОТА"},
            new ReadyWorkStatusItem() {code = 1104, text = "РАБОТА"},
            new ReadyWorkStatusItem() {code = 1105, text = "РАБОТА"},
            new ReadyWorkStatusItem() {code = 1107, text = "РАБОТА"},
            new ReadyWorkStatusItem() {code = 1108, text = "РАБОТА"},




        };
    }

    public class WrapperReadyWorkStatusNGO : TagWrapper, TagWrapper.IPostInitialize
    {
        private ObservableCollection<ReadyWorkStatusItem> _statusCodes = STANDART._STATUS_CODES;
        private string _wrapperId = "pump1_cer";
        private string _emergencyWraperId = "dr_drive_emergency";
        private string _text = "";
        private string _driveName = "Привод Х";
        private BitWrapper _emertag;

        public ObservableCollection<ReadyWorkStatusItem> statusCodes
        {
            get { return _statusCodes; }
            set
            {
                _statusCodes = value;
                onPropertyChanged(new PropertyChangedEventArgs("statusCodes"));
            }
        }

        public string wrapperId
        {
            get { return _wrapperId; }
            set
            {
                _wrapperId = value;
                onPropertyChanged(new PropertyChangedEventArgs("wrapperId"));
            }
        }

        public string text
        {
            get { return _text; }
            set
            {
                _text = value;
                onPropertyChanged(new PropertyChangedEventArgs("text"));

            }
        }

        public string driveName
        {
            get { return _driveName; }
            set
            {
                _driveName = value;
                onPropertyChanged(new PropertyChangedEventArgs("driveName"));

            }
        }

        public string emergencyWraperId
        {
            get { return _emergencyWraperId; }
            set { _emergencyWraperId = value; }
        }


        public void initialize(Project scadaeprj)
        {
            if (((WrappersModule)scadaeprj.modules[WrappersModule.MODULE_ID]).objects.ContainsKey(wrapperId))
            {
                var tag = ((WrappersModule)scadaeprj.modules[WrappersModule.MODULE_ID]).objects[wrapperId];
                tag.PropertyChanged += tag_PropertyChanged;
            }


            if (((WrappersModule) scadaeprj.modules[WrappersModule.MODULE_ID]).objects.ContainsKey(emergencyWraperId))
            {
                _emertag =
                    ((WrappersModule) scadaeprj.modules[WrappersModule.MODULE_ID]).objects[emergencyWraperId] as
                        BitWrapper;
                if (_emertag != null)
                    _emertag.PropertyChanged += emertag_PropertyChanged;
            }
        }

        void emertag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var t = sender as BitWrapper;
            if (t == null) return;
            if (t.value)
                text = driveName + " " + "АВАРИЯ";
        }

        void tag_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var t = sender as NumericalWrapper;
            if (t == null) return;

            if (e.PropertyName != "value") return;
            var code = (int)t.value;
            foreach (var item in statusCodes.Where(item => item.code == code))
            {
                text = driveName + " " + item.text;
                return;
            }

            if (!_emertag.value)
                text = driveName + " НЕ ГОТОВ";
        }

        public void exit(Project project)
        {
            if (((WrappersModule)project.modules[WrappersModule.MODULE_ID]).objects.ContainsKey(wrapperId))
            {
                var tag = ((WrappersModule)project.modules[WrappersModule.MODULE_ID]).objects[wrapperId];
                tag.PropertyChanged -= tag_PropertyChanged;
            }

            if (((WrappersModule)project.modules[WrappersModule.MODULE_ID]).objects.ContainsKey(emergencyWraperId))
            {
                var emertag = ((WrappersModule)project.modules[WrappersModule.MODULE_ID]).objects[emergencyWraperId];
                emertag.PropertyChanged -= emertag_PropertyChanged;
            }
        }
    }

    public class ReadyWorkStatusItem
    {
        public int code { get; set; }
        public string text { get; set; }
    }
}
