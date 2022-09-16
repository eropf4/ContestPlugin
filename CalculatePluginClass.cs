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
            var LevelCollector = new FilteredElementCollector(doc);

            ICollection<Element> allWalls = WallCollector.OfClass(typeof(Wall)).WhereElementIsNotElementType().ToElements();
            var allLevelsId = LevelCollector.OfClass(typeof(Level)).ToElementIds();
            var firstLevelId = allLevelsId.First();

            CreateWalls(doc, firstLevelId);
            CalculateSquarePoints(new XYZ(0, 0, 0), new XYZ(3, 3, 2), new XYZ(0, 0, 2), new XYZ(3, 3, 3));

            return Result.Succeeded;

            //    var firstWall = allWalls.First();
            //    var secondWall = allWalls.Last();

            //    var newTransaction = new Transaction(doc);

            //    newTransaction.Start("Join");

            //    JoinGeometryUtils.JoinGeometry(doc, firstWall, secondWall);
            //    newTransaction.Commit();

            //    var wallGeometry = firstWall.get_Geometry(new Options());
            //    return Result.Succeeded;
            //}

            //private XYZ[] GetWallPoints(BoundingBoxXYZ boundingBoxXYZ)
            //{
            //    var maxPoint = boundingBoxXYZ.Max;
            //    var minPoint = boundingBoxXYZ.Min;

            //    return new XYZ[8] {maxPoint,
            //        minPoint,
            //        new XYZ(maxPoint.X, minPoint.Y, maxPoint.Z),
            //        new XYZ(maxPoint.X, maxPoint.Y, minPoint.Z),
            //        new XYZ(maxPoint.X, minPoint.Y, minPoint.Z),
            //        new XYZ(minPoint.X, minPoint.Y, maxPoint.Z),
            //        new XYZ(minPoint.X, maxPoint.Y, maxPoint.Z),
            //        new XYZ(maxPoint.X, maxPoint.Y, minPoint.Z)};
            //}
            //private bool TwoWallsIsBeside (Wall firstWall, Wall secondWall)
            //{
            //    if (firstWall.Orientation == SecondWall.Orientation)
            //    {
            //        var firstWallLocation = (LocationCurve)firstWall.Location;
            //        var secondWallLocation = (LocationCurve)secondWall.Location;
            //        if (firstWallLocation.Curve.GetEndPoint(1) )
            //    }
            //    return false;
        }

        public static void CreateWalls (Document doc, ElementId LevelId)
        {
            //var lineOfFirstWall = (Curve)Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 10, 10));
            //var lineOfSecondWall = (Curve)Line.CreateBound(new XYZ(10, 5, 0), new XYZ(20, 5, 0));

            //var newTransaction = new Transaction(doc);
            //newTransaction.Start("CreateWalls");
            //Wall.Create(doc, lineOfFirstWall, LevelId, true);
            //Wall.Create(doc, lineOfSecondWall, LevelId, true);
            //newTransaction.Commit();

            XYZ start = new XYZ(0, 0, 0);
            XYZ end = new XYZ(10, 10, 0);
            Line geomLine = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 0, 0));

            var newTransaction = new Transaction(doc);
            newTransaction.Start("CreateWalls");
            Wall.Create(doc, geomLine, LevelId, true);
            newTransaction.Commit();
        }

        public static IEnumerable<XYZ> CalculateSquarePoints (XYZ minWall1, XYZ maxWall1, XYZ minWall2, XYZ maxWall2)
        {
            var allPoints = new List<XYZ> { minWall1, maxWall1, minWall2, maxWall2 };

            var listX = allPoints.Select(x => x.X).OrderBy(x => x).ToList();
            var listY = allPoints.Select(x => x.Y).OrderBy(x => x).ToList();
            var listZ = allPoints.Select(x => x.Z).OrderBy(x => x).ToList();

            listX.Remove(listX.First());
            listX.Remove(listX.Last());

            listY.Remove(listY.First());
            listY.Remove(listY.Last());

            listZ.Remove(listZ.First());
            listZ.Remove(listZ.Last());

            var result = new List<XYZ>();

            foreach (var x in listX)
                foreach (var y in listY)
                    foreach (var z in listZ)
                        result.Add(new XYZ(x, y, z));

            var distinctResult = result.Distinct(new XyzComparer()).ToList();
            return distinctResult;
        }
    }

    public class XyzComparer : IEqualityComparer<XYZ>
    {
        public bool Equals(XYZ x1, XYZ x2)
        {
            return x1.X == x2.X && x1.Y == x2.Y && x1.Z == x2.Z;
        }

        public int GetHashCode(XYZ obj)
        {
            return (int)(100 * obj.X + 10 * obj.Y + obj.Z);
        }
    }
}
