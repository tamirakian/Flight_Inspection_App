using System;
using System.Collections.Generic;
using System.Text;

namespace Flight_Inspection_App.HelperClasses
{
    public class correlatedFeatures
    {
        string feature1, feature2;  // names of the correlated features
        float corrlation;
        Line lin_reg;
        float threshold;
        float cx, cy;

        public correlatedFeatures(string f1, string f2, float cor, Line reg, float threshold)
        {
            this.feature1 = f1;
            this.feature2 = f2;
            this.corrlation = cor;
            this.lin_reg = reg;
            this.threshold = threshold;
        }

        public string Feature1
        {
            get
            {
                return feature1;
            }
            set
            {
                feature1 = value;
            }
        }

        public string Feature2
        {
            get
            {
                return feature2;
            }
            set
            {
                feature2 = value;
            }
        }

        public float CX
        {
            get
            {
                return cx;
            }
            set
            {
                cx = value;
            }
        }

        public float CY
        {
            get
            {
                return cy;
            }
            set
            {
                cy = value;
            }
        }

        public Line LineReg
        {
            get
            {
                return lin_reg;
            }
            set
            {
                lin_reg = value;
            }
        }

        public float Threshhold
        {
            get
            {
                return threshold;
            }
            set
            {
                threshold = value;
            }
        }

        public float Correlation
        {
            get
            {
                return corrlation;
            }
            set
            {
                corrlation = value;
            }
        }
    }
}
