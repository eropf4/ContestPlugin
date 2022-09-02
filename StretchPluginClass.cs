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

    public class StretchPluginClass : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var WallCollector = new FilteredElementCollector(doc);
            ICollection<Element> allWalls = WallCollector.OfClass(typeof(Wall)).WhereElementIsNotElementType().ToElements();

            var firstWall = (Wall)allWalls.FirstOrDefault();

            var firstWallLocationCurve = (LocationCurve)firstWall.Location;

            var firstWallCurvePT1 = firstWallLocationCurve.Curve.GetEndPoint(0);
            var firstWallCurvePT2 = firstWallLocationCurve.Curve.GetEndPoint(1);

            var wallVector = new XYZ(firstWallCurvePT2.X - firstWallCurvePT1.X,
                firstWallCurvePT2.Y - firstWallCurvePT1.Y,
                firstWallCurvePT2.Z - firstWallCurvePT1.Z);

            var newXYZs = new XYZ(firstWallCurvePT1.X, firstWallCurvePT1.Y, firstWallCurvePT1.Z);
            var newXYZe = new XYZ(wallVector.X * 2 + firstWallCurvePT1.X,
                wallVector.Y * 2 + firstWallCurvePT1.Y ,
                wallVector.Z * 2 + firstWallCurvePT1.Z);

            Line newLine = Line.CreateBound(newXYZs, newXYZe);

            var newTransaction = new Transaction(doc, "stretch");
            newTransaction.Start("stretch");
            firstWallLocationCurve.Curve = newLine;
            newTransaction.Commit();

            return Result.Succeeded;
        }
    }


}
