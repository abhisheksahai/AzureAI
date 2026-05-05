namespace Tutort.DSA
{
	public class Assignment
	{
		public int MinimumThreeNumbers(int x, int y, int z)
		{
			int result = z;
			if (x < y && x < z)
			{
				result = x;
			}
			else if (y < x && y < z)
			{
				result = y;
			}
			return result;
		}

		public bool CoupleIsEligibleForMarriage(int girlAge, int boyAge)
		{
			return (girlAge >= 18 && boyAge >= 21) ? true : false;
		}
	}
}