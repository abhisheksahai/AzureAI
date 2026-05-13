using Tutort.DSA.Utils;

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

		public double PrintTaxAmount(double amount)
		{
			return (amount >= 500000) ? amount * .01 : amount * 0.05;
		}

		public bool IsLeapYear(int year)
		{
			return (year % 4 == 0) && (year % 100 != 0 || year % 400 == 0);
		}

		public NumberType GetNumberType(int number)
		{
			return (number % 2) == 0 ? NumberType.Even : NumberType.Odd;
		}

		public void PrintTriangle(int size)
		{
			for (int i = 1; i <= size; i++)
			{
				for (int j = 1; j <= i; j++)
				{
					Console.Write("*");
				}
				Console.WriteLine("");
			}
		}

		public int PrintLargestOf3Numbers_1(int a, int b, int c)
		{
			return Math.Max(Math.Max(a, b), c);
		}

		public int PrintLargestOf3Numbers_2(int a, int b, int c)
		{
			int max = int.MinValue;
			if (a > max)
			{
				max = a;
			}
			if (b > max)
			{
				max = b;
			}
			if (c > max)
			{
				max = c;
			}
			return max;
		}

		public int PrintLargestOf3Numbers_3(int a, int b, int c)
		{
			return (a > b && a > c) ? a : (b > a && b > c) ? b : c;
		}

		public void PrintNumberSeries(int size)
		{
			int num = 1;
			for (int i = 1; i <= size; i++)
			{
				for (int j = 1; j <= i; j++)
				{
					Console.Write($"{num} ");
					num++;
				}
				Console.WriteLine("");
			}
		}


		public int[] MergeTwoSortedArrays_1(int[] a, int[] b)
		{
			int arr1Len = a.Length;
			int arr2Len = b.Length;
			int totalLen = arr1Len + arr2Len;
			int[] c = new int[totalLen];
			for (int i = 0; i < arr1Len; i++)
			{
				c[i] = a[i];
			}
			for (int j = arr1Len; j < totalLen; j++)
			{
				c[j] = b[j - arr1Len];
			}
			Array.Sort(c);
			return c;
		}

		public int[] MergeTwoSortedArrays_2(int[] a, int[] b)
		{
			int l1 = a.Length;
			int l2 = b.Length;
			int[] c = new int[l1 + l2];
			int i = 0, j = 0, k = 0;
			while (i < l1 && j < l2)
			{
				if (a[i] < b[j])
				{
					c[k++] = a[i++];
				}
				else
				{
					c[k++] = b[j++];
				}
			}
			while (i < l1)
			{
				c[k++] = a[i++];
			}
			while (j < l2)
			{
				c[k++] = b[j++];
			}
			return c;
		}
	}
}