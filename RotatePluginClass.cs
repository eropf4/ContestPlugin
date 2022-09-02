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

    public class RotatePluginClass : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var WallCollector = new FilteredElementCollector(doc);
            var FloorCollector = new FilteredElementCollector(doc);

            ICollection<Element> allWalls = WallCollector.OfClass(typeof(Wall)).WhereElementIsNotElementType().ToElements();
            ICollection<Element> allFloor = FloorCollector.OfClass(typeof(Floor)).WhereElementIsNotElementType().ToElements();
            
            ICollection<ElementId> allElementsId = allWalls.Union(allFloor).Select(x => x.Id).ToList();
            

            if (allElementsId.Count > 4)
            {
                var axis = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, 0, 1));
                double angle = 90 * Math.PI / 180;
                var newTransaction = new Transaction(doc);

                newTransaction.Start("RotateElement");

                ElementTransformUtils.RotateElements(doc, allElementsId, axis, angle);
                newTransaction.Commit();
            }

            return Result.Succeeded;
        }
    }
}
