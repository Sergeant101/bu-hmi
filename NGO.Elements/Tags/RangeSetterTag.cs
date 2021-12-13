using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using SCADAExtension8.Core.SCADAE8;
using SCADAExtension8.Core.Utils;
using SCADAExtension8.Core.Wrappers;

namespace NGO.Elements.Tags
{
    public class RangeSetterTag : TagWrapper, TagWrapper.IPostInitialize
    {
        private string _maximumTag;
        private string _minimumTag;
        private string _destinationTag;
        private NumericalWrapper _max, _dst, _min;

        [Category(Helper.SCADAE8_CATEGORY_ATTRIBUTE)]
        public string maximumTag
        {
            get { return _maximumTag; }
            set { _maximumTag = value; onPropertyChanged(new PropertyChangedEventArgs("maximumTag")); }
        }

        [Category(Helper.SCADAE8_CATEGORY_ATTRIBUTE)]
        public string minimumTag
        {
            get { return _minimumTag; }
            set { _minimumTag = value; onPropertyChanged(new PropertyChangedEventArgs("minimumTag")); }
        }

        [Category(Helper.SCADAE8_CATEGORY_ATTRIBUTE)]
        public string destinationTag
        {
            get { return _destinationTag; }
            set { _destinationTag = value; onPropertyChanged(new PropertyChangedEventArgs("destinationTag")); }
        }

        public void initialize(Project scadaeprj)
        {
            var objects = ((WrappersModule) Project.current.modules[WrappersModule.MODULE_ID]).objects.Values;
            _max = objects.OfType<NumericalWrapper>().FirstOrDefault(t => t.name == maximumTag);
            _min = objects.OfType<NumericalWrapper>().FirstOrDefault(t => t.name == minimumTag);
            _dst = objects.OfType<NumericalWrapper>().FirstOrDefault(t => t.name == destinationTag);
            if (_dst == null) return;                
            if (_max != null) _max.PropertyChanged += maxOnPropertyChanged;
            if (_min != null) _min.PropertyChanged += minOnPropertyChanged;
        }

        private void minOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var cast = (NumericalWrapper)sender;
            if (args.PropertyName == "value")
                _dst.absoluteMinimum = cast.value;
        }

        private void maxOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var cast = (NumericalWrapper) sender;
            if (args.PropertyName == "value")
                _dst.absoluteMaximum = cast.value;
        }

        public void exit(Project project)
        {
            if (_dst == null) return;
            if (_max != null) _max.PropertyChanged -= maxOnPropertyChanged;
            if (_min != null) _min.PropertyChanged -= minOnPropertyChanged;
        }
    }
}
