﻿using SAM.Geometry.SolarCalculator;
using System.Collections.Generic;

namespace SAM.Analytical.SolarCalculator
{
    public static partial class Convert
    {
        public static SolarModel ToSAM_SolarModel(AnalyticalModel analyticalModel)
        {
            if(analyticalModel == null)
            {
                return null;
            }

            AdjacencyCluster adjacencyCluster = analyticalModel.AdjacencyCluster;
            if (adjacencyCluster == null)
            {
                return null;
            }

            SolarModel result = new SolarModel(analyticalModel.Location);

            List<Panel> panels = adjacencyCluster.GetPanels();
            if(panels != null || panels.Count != 0)
            {
                foreach(Panel panel in panels)
                {
                    List<Space> spaces = adjacencyCluster.GetSpaces(panel);
                    if (spaces != null && spaces.Count >= 2)
                    {
                        continue;
                    }

                    if(!panel.IsExposedToSun())
                    {
                        continue;
                    }

                    SolarFace solarFace = new SolarFace(panel.Guid, panel.Face3D);
                    result.Add(solarFace);
                }

            }

            return result;
        }
    }
}
