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

        public featureID()
        {
            values = new List<float>();
            featureName = "";
        }

        public List<float> Values
        {
            get { return values; }
            set
            {
                values = value;
            }
        }

        public string FeatureName
        {
            get { return featureName; }
            set
            {
                featureName = value;
            }
        }
    };


    public class TimeSeries
    {
        private SortedDictionary<int, featureID> featuresMap;
        public SortedDictionary<int, featureID> FeaturesMap
        {
            get { return featuresMap; }
            set { featuresMap = value; }
        }

        public TimeSeries()
        {
            featuresMap = new SortedDictionary<int, featureID>();
        }

        // intializing the map keys and values from the csv file
        public void initFeaturesMap(string fileName, string settingsName)
        {
            StreamReader reader = new StreamReader(fileName);
            // creating from each feature a key in the map by its position index in the line
            loadFeaturesFromFile(settingsName);
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

        // getting the first line of the features names and creating for each of them a new key in the map by their order from right to left.
        public void loadFeaturesFromFile(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            XmlNode root = doc.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("descendant::name");
            int i = 0;
            foreach (XmlNode node in nodes)
            {
                if (i >= Constants.NUM_OF_NODES)
                {
                    break;
                }
                featureID id = new featureID();
                id.FeatureName = node.InnerText;
                featuresMap.Add(i++, id);
            }
        }

        public void addValues(string timeStepValues)
        {
            List<string> valList = stringParser(timeStepValues);
            for (int i = 0; i < valList.Count(); i++)
            {
                //converting the string to float
                float floatValue = (float)Convert.ToDouble(valList[i]);
                //adding the value i to the feature in index i in the map
                featuresMap.ElementAt(i).Value.Values.Add(floatValue);
            }
        }

        // getting the features names vector
        public List<string> getFeaturesNames()
        {
            List<string> featuresNames = new List<string>();
            for (int i = 0; i < featuresMap.Count(); i++)
            {
                featuresNames.Add(featuresMap[i].FeatureName);
            }
            return featuresNames;
        }

        // getting a specific feature's value from index
        public float getFeatureVal(int featureIndex, int valueIndex)
        {
            return featuresMap[featureIndex].Values[valueIndex];
        }

        // getting a specific feature's value from name
        public float getFeatureVal(string featureName, int valueIndex)
        {
            foreach (var feature in featuresMap)
            {
                if (feature.Value.FeatureName == featureName)
                {
                    return getFeatureVal(feature.Key, valueIndex);
                }
            }
            return 0;
        }

        // getting the specific time stemp values
        public List<float> getTimeStemp(int stepIndex)
        {
            List<float> list = new List<float>();
            for (int i = 0; i < featuresMap.Count(); i++)
            {
                list.Add(featuresMap[i].Values[stepIndex]);
            }
            return list;
        }

        public string GetTimestepStr(int index)
        {
            string str = "";
            str += featuresMap[0].Values[index].ToString();
            for (int i = 1; i < featuresMap.Count(); i++)
            {
                str += ",";
                str += featuresMap[i].Values[index].ToString();
            }
            str += "\r\n";
            return str;
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
            for (int i = 0; i < featuresMap.Count(); i++)
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

        public List<float> getAllFeatureValues (string name)
        {
            return featuresMap[getFeatureIndex(name)].Values;
        }
    }
}
