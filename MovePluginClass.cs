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

    public class MovePluginClass : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            var WallCollector = new FilteredElementCollector(doc);
            var FloorCollector = new FilteredElementCollector(doc);

            ICollection<Element> allWalls = WallCollector.OfClass(typeof(Wall)).WhereElementIsNotElementType().ToElements();
            ICollection<Element> allFloor = FloorCollector.OfClass(typeof(Floor)).WhereElementIsNotElementType().ToElements();

            ICollection<Element> allElements = allWalls.Union(allFloor).ToList();
            ICollection<ElementId> allElementsId = allElements.Select(x => x.Id).ToList();


                var floor = (Floor)allFloor.First();
                var floorBoundingBox = floor.get_BoundingBox(doc.ActiveView);
                var floorBoundingBoxXYZ = (floorBoundingBox.Min + floorBoundingBox.Max) / 2;


            using (var newTransaction = new Transaction(doc))
            {
                newTransaction.Start("MoveElement");
                ElementTransformUtils.MoveElements(doc, allElementsId, new XYZ(-floorBoundingBoxXYZ.X, -floorBoundingBoxXYZ.Y, -floorBoundingBoxXYZ.Z));
                newTransaction.Commit();
            }


            return Result.Succeeded;
        }
    }
}
