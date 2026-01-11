namespace SerializeInterfaces.Editor.UIElements
{
	using UnityEngine.UIElements;

	internal class Tab : Toggle
	{
		public Tab(string text) : base()
		{
			base.text = text;
			RemoveFromClassList(Toggle.ussClassName);
			AddToClassList(ussClassName);
		}
	}
}