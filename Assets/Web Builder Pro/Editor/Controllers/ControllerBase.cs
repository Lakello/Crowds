using UnityEngine;
using UnityEngine.UIElements;

namespace Anvil.WebBuilderPro
{
    internal abstract class ControllerBase
    {
        protected readonly WebBuilderProModel ProModel;
        protected readonly VisualElement Root;

        internal ControllerBase(WebBuilderProModel proModel, VisualElement root)
        {
            ProModel = proModel;
            Root = root;
        }

        protected void DebugElement(VisualElement element)
        {
            // print the name of every element in the tree
            void PrintElementName(VisualElement element)
            {
                Debug.Log($"Element name: {element.name}");
                foreach (var child in element.Children())
                {
                    PrintElementName(child);
                }
            }

            PrintElementName(element);
        }

        protected bool DoesNameExist(string name)
        {

            bool NameExists(VisualElement element)
            {
                if (element.name == name)
                {
                    return true;
                }

                foreach (var child in element.Children())
                {
                    if (NameExists(child))
                    {
                        return true;
                    }
                }

                return false;
            }

            return NameExists(Root);

        }
    }
}