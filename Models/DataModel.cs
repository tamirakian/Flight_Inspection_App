using Flight_Inspection_App.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Flight_Inspection_App
{
    class DataModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private List<string> featureTypes;
        private int frameNumber;
        private string csvFileName;
        private featureModel featuremodel;


        public DataModel()
        {
            featureTypes = new List<string>();
            //each line in csv file represents frame
            frameNumber = 0;

            // reading the XML File and parsing it to get the feature names.
            XmlReader rdr = XmlReader.Create("playback_small.xml");
            while (rdr.Read())
            {
                if (rdr.LocalName == "chunk")
                {
                    XElement chunk = (XElement)XDocument.ReadFrom(rdr);
                    string child = (chunk.FirstNode.NextNode as XElement).Value;
                    // adding the names to the features list.
                    featureTypes.Add(child);
                }
            }

            featuremodel = new featureModel();
            featuremodel.startCommunicatingWithServer();
        }


        public string pCSV
        {
            get
            { return this.csvFileName; }
            set
            {
                this.csvFileName = value;
                NotifyPropertyChanged("pCSV");
            }
        }

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
