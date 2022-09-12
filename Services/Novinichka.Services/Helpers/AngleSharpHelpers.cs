using System;
using System.Linq;

using AngleSharp.Dom;

namespace Novinichka.Services.Helpers
{
    public static class AngleSharpHelpers
    {
        public static void RemoveChildNodes(this INode element, INode elementToRemove)
        {
            if (elementToRemove == null)
            {
                return;
            }

            try
            {
                element.RemoveChild(elementToRemove);
            }
            catch
            {
                // ignored
            }

            foreach (var node in element.ChildNodes)
            {
                node.RemoveChildNodes(elementToRemove);
            }
        }

        public static void RemoveElement(this IElement element, string elementToRemove)
            => element?.QuerySelector(elementToRemove)?.Remove();

        public static void RemoveGivenTag(this IElement element, string tag)
        {
            foreach (var e in element?.QuerySelectorAll(tag))
            {
                e.Remove();
            }
        }

        public static void RemoveComments(this IElement element)
            => element.Descendents<IComment>()
                .Where(n => n.NodeType == NodeType.Comment)
                .ToList()
                .ForEach(n => n.Remove());
    }
}
