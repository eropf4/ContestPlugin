using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace ContestPlugins
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CalculatePluginClass : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var WallCollector = new FilteredElementCollector(doc);

            ICollection<Element> allWalls = WallCollector.OfClass(typeof(Wall)).WhereElementIsNotElementType().ToElements();

            var firstWall = allWalls.First();
            var secondWall = allWalls.Last();

            var newTransaction = new Transaction(doc);

            newTransaction.Start("Join");

            JoinGeometryUtils.JoinGeometry(doc, firstWall, secondWall);
            newTransaction.Commit();

            var wallGeometry = firstWall.get_Geometry(new Options());
            return Result.Succeeded;
        }

        private XYZ[] GetWallPoints(BoundingBoxXYZ boundingBoxXYZ)
        {
            var maxPoint = boundingBoxXYZ.Max;
            var minPoint = boundingBoxXYZ.Min;

            return new XYZ[8] {maxPoint,
                minPoint,
                new XYZ(maxPoint.X, minPoint.Y, maxPoint.Z),
                new XYZ(maxPoint.X, maxPoint.Y, minPoint.Z),
                new XYZ(maxPoint.X, minPoint.Y, minPoint.Z),
                new XYZ(minPoint.X, minPoint.Y, maxPoint.Z),
                new XYZ(minPoint.X, maxPoint.Y, maxPoint.Z),
                new XYZ(maxPoint.X, maxPoint.Y, minPoint.Z)};
        }
        //private bool TwoWallsIsBeside (Wall firstWall, Wall secondWall)
        //{
        //    if (firstWall.Orientation == SecondWall.Orientation)
        //    {
        //        var firstWallLocation = (LocationCurve)firstWall.Location;
        //        var secondWallLocation = (LocationCurve)secondWall.Location;
        //        if (firstWallLocation.Curve.GetEndPoint(1) )
        //    }
        //    return false;
        //}
    }
}
