namespace TargetModule.Destination
{
	public interface IDestinationPoint
	{
		public ref TargetPointData GetPreviousTargetData();
		public ref TargetPointData GetCurrentTargetData();

		/// <summary>
		/// ����� ��� ���������� TargetPointData ������� �����
		/// </summary>
		public void NextData();

		/// <summary>
		/// ����� ������� �����
		/// </summary>
		public void NextPoint();

		public void SetPoint(int point);

		public void ResetPoint();
	}
}