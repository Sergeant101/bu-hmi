using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Monokot.Hmi.Core.Utils;

namespace NGO.Elements.Special.Pdf
{
    public class PdfHelper : INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler PropertyChanged;

        private string _path;
        private readonly ObservableCollection<PdfHelper> _files = new ObservableCollection<PdfHelper>();
        private readonly WPFPdfViewer.PdfViewer _pdfViewer;


        public bool isSelected
        {
            get { return _isSelected; }
            set
            {
                if (value.Equals(_isSelected)) return;
                _isSelected = value;
                if (isFile) _pdfViewer.LoadFile(path);
                onPropertyChanged("isSelected");
            }
        }

        public PdfHelper(WPFPdfViewer.PdfViewer p)
        {
            _pdfViewer = p;
        }

        public Uri img
        {
            get
            {
                return isFile ? new Uri(@"pack://application:,,,/NGO.Elements;component/Content/Icon/pdf-icon.png") : new Uri(@"pack://application:,,,/NGO.Elements;component/Content/Icon/folder-icon.png");
            }
        }

        public string name
        {
            get
            {
                return isFile ? Path.GetFileName(path) : new DirectoryInfo(path).Name;
            }
        }

        public string path
        {
            get { return _path; }
            set
            {
                if (value == _path) return;
                _path = value;
                onPropertyChanged("path");
            }
        }

        public ObservableCollection<PdfHelper> files
        {
            get
            {
                try
                {
                    var result = new ObservableCollection<PdfHelper>();
                    var dirInf = new DirectoryInfo(path);

                    if (!dirInf.Exists) return result;
                    var dir = Directory.GetDirectories(path);
                    foreach (var d in dir)
                        result.Add(new PdfHelper(_pdfViewer) { path = d });
                    var pdfs = Directory.GetFiles(path, "*.pdf");
                    foreach (var f in pdfs)
                        result.Add(new PdfHelper(_pdfViewer) { path = f, isFile = true });


                    return result;
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message, "Error!", MessageBoxButton.OK);
                }
                return _files;
            }
        }

        private bool _isFile;
        private bool _isSelected;

        public bool isFile
        {
            get { return _isFile; }
            set
            {
                if (value.Equals(_isFile)) return;
                _isFile = value;
                onPropertyChanged("isFile");
            }
        }

        protected virtual void onPropertyChanged(string property_name)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(property_name));
        }
    }
}
