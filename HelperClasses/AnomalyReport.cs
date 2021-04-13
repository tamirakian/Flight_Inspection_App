using System;
using System.Collections.Generic;
using System.Text;

namespace Flight_Inspection_App.HelperClasses
{
	public class AnomalyReport

	{
		string description;
		long timeStep;

		public AnomalyReport(string description, long timeStep)
		{
			this.description = description;
			this.timeStep = timeStep;
		}
	};
}
