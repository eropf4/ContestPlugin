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

    public class CopyPluginClass : IExternalCommand
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

            
            var firstFloorMaxCoord = allFloor.Last().get_BoundingBox(doc.ActiveView).Max ;
            var firstFloorMinCoord = allFloor.Last().get_BoundingBox(doc.ActiveView).Min;
            var newFloorTranslation = new XYZ(Math.Abs(firstFloorMinCoord.X - firstFloorMaxCoord.X),
                Math.Abs(firstFloorMinCoord.Y - firstFloorMaxCoord.Y),
                Math.Abs(firstFloorMinCoord.Z - firstFloorMaxCoord.Z));

            var newTransaction = new Transaction(doc);

            newTransaction.Start("MoveElement");

            ElementTransformUtils.CopyElements(doc,
                allElementsId,
                new XYZ(newFloorTranslation.X, newFloorTranslation.Y, newFloorTranslation.Z));
            newTransaction.Commit();

            return Result.Succeeded;
        }
    }
}
