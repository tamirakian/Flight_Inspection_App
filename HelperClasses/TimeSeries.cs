using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Globalization;

namespace Flight_Inspection_App.HelperClasses
{
    // representing each one of the features
    public class featureID
    {
        List<float> values;
        string featureName;
        public List<float> Values
        {
            get { return values; }
            set { }
        }
        public string FeatureName
        {
            get { return featureName; }
            set { }
        }
    };


    class TimeSeries
    {
        private string CSVFile;
        private SortedDictionary<int, featureID> featuresMap;

        public TimeSeries(string CSVFile)
        {
            this.CSVFile = CSVFile;
        }

        // intializing the map keys and values from the csv file
        public void initFeaturesMap(string fileName)
        {
            string featuresLine;
            StreamReader reader = new StreamReader(fileName);
            // getting the first line from the file - the features names line
            featuresLine = reader.ReadLine();
            // creating from each feature a key in the map by its position indes in the line
            loadFeaturesFromFile(featuresLine);
            // iterating over the time steps lines in the file
            string valuesLine;
            while ((valuesLine = reader.ReadLine()) != null)
            {
                // assigning the time step values to their feature in map
                addValues(valuesLine);
            }
            reader.Close();
        }

        // parsring a string into substrings and returning a vector of all of the substrings
        public List<string> stringParser(string str)
        {
            List<string> list = new List<string>();
            string[] words = str.Split(',');

            foreach (var word in words)
            {
                list.Add(word);
            }
            return list;
        }

        // getting the first line of the features names and creating for each of them a new key in the map by their order from right
        // to left.
        public void loadFeaturesFromFile(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            XmlElement root = doc.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("Output");
            int i = 0;
            foreach (XmlNode node in nodes)
            {
                var childNodes = node.SelectNodes("Chunk");
                if (childNodes != null)
                {
                    foreach (XmlNode childNode in childNodes)
                    {
                        featureID id = new featureID();
                        if(childNode.InnerText == "name")
                        {
                            id.FeatureName = childNode.InnerText;
                            featuresMap[i++] = id;
                        }
                    }
                }
            }
        }

        public void addValues(string timeStepValues)
        {
            List<string> valList = stringParser(timeStepValues);
            for (int i = 0; i < valList.Count(); i++)
            {
                //converting the string to float
                float floatValue = float.Parse(valList[i], CultureInfo.InvariantCulture.NumberFormat);
                //adding the value i to the feature in index i in the map
                featuresMap[i].Values.Add(floatValue);
            }
        }

        // getting the features names vector
        public List<string> getFeaturesNames()
        {
            List<string> featuresNames = new List<string>();
            for (int i = 0; i<featuresMap.Count(); i++)
            {
                featuresNames.Add(featuresMap[i].FeatureName);
            }
            return featuresNames;
        }

        // getting a specific value of a specific feature 
        public float getFeatureVal(int featureIndex, int valueIndex)
        {
            return featuresMap[featureIndex].Values[valueIndex];
        }

        // getting the specific time step values
        public List<float> getTimeStemp(int stepIndex)
        {
            List<float> list = new List<float>();
            for (int i = 0; i<featuresMap.Count(); i++)
            {
                list.Add(featuresMap[i].Values[stepIndex]);
            }
            return list;
        }

        // getting the size of the features
        public int getNumOfFeatures()
        {
            return featuresMap.Count();
        }

        //getting the featureID in index i from the map
        public featureID getFeatureID(int featureIndex)
        {
            return featuresMap[featureIndex];
        }

        // getting the feature indes in the map by its name
        public int getFeatureIndex(string featureName)
        {
            for (int i = 0; i<featuresMap.Count(); i++)
            {
                if (featuresMap[i].FeatureName == featureName)
                {
                    return i;
                }
            }
            return 0;
        }

        // get the size of the time steps
        public int getNumOfTimesteps()
        {
            return featuresMap[0].Values.Count();
        }
    }
}
