using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{
	public static class DiamondSquareGen
	{

		public static int height = 257;
		public static int width =  height;
		public static int[,] heightmap = new int[height, width];
		public static bool[,] heightmoved = new bool[height, width];
		public static Random ra = new Random();

		public static int[,] Start()
		{
			heightmap = new int[height, width];
			heightmoved = new bool[height, width];
			int mijl = height/2;
			heightmap[0, 0] = ra.Next(1, 45);
			heightmap[0, width-1] = ra.Next(1, 45);
			heightmap[height-1, 0] = ra.Next(1, 45);
			heightmap[height-1, width - 1] = ra.Next(1, 45);
			heightmoved[0, 0] = true;
			heightmoved[0, width - 1] = true;
			heightmoved[height - 1, 0] = true;
			heightmoved[height - 1, width - 1] = true;
			double sq = 1.4;
			while (mijl >= 1)
			{
				DiamondSquare(mijl, (int)sq);
				mijl /= 2;
				sq *= 1.6;
			}
			for (int i = 0; i < 5 ; i++)
			{
				Smoothing();
			}
			Normalizing();

			return heightmap;
		}

		private static void Normalizing()
		{
			int min=1000;
			foreach (int i in heightmap)
			{
				if (i < min) min = i;
			}
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					heightmap[i, j] -= min;
				}
			}
		}
		private static void Smoothing()
		{
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					int aux = 0;
					int sum = 0;
					for (int k = -3; k <= 3; k++)
					{
						for (int l = -3; l <= 3; l++)
						{
							if (k + i >= 0 && k + i < height && j + l >= 0 && j + l < width)
							{
								sum += heightmap[i + k, j + l];
								aux++;
							}
						}
					}
					heightmap[i, j] = sum / aux;
				}
			}
		}
		

		private static void DiamondSquare(int mijl, int sq)
		{

			for (int i = mijl; i <= height - mijl; i += mijl)
			{
				for (int j = mijl; j <= width - mijl; j += mijl)
				{
					if (heightmoved[i, j] == false)
					{
						DiamondStep(i, j, mijl, sq);
						SquareStep(i, j, mijl, sq);
					}
				}
			}
		}

		private static void DiamondStep(int i, int j, int mijl, double sq)
		{
			if (!heightmoved[i, j]) heightmap[i, j] = (heightmap[i - mijl, j - mijl] + heightmap[i + mijl, j + mijl] + heightmap[i + mijl, j - mijl] + heightmap[i - mijl, j + mijl]) / 4 + ra.Next(1, (int)(1 + 45 / sq));
			heightmoved[i, j] = true;
		}
		private static void SquareStep(int i, int j, int mijl, double sq)
		{
			int imin = i - mijl;
			int imax = i + mijl;
			int jmin = j - mijl;
			int jmax = j + mijl;
			if (!heightmoved[imin, j])
				heightmap[imin, j] = ((imin - mijl < 0) ? ((heightmap[imin, jmax] + heightmap[imin, jmin] + heightmap[imin + mijl, j]) / 3) : ((heightmap[imin, jmax] + heightmap[imin, jmin] + heightmap[imin + mijl, j] + heightmap[imin - mijl, j]) / 4)) + ra.Next(1, (int)(1 + 45 / sq));
			if (!heightmoved[imax, j])
				heightmap[imax, j] = ((imax + mijl >= height) ? ((heightmap[imax, j + mijl] + heightmap[imax, j - mijl] + heightmap[imax - mijl, j]) / 3) : ((heightmap[imax, j + mijl] + heightmap[imax, j - mijl] + heightmap[imax + mijl, j] + heightmap[imax - mijl, j]) / 4 + 1)) + ra.Next(1, (int)(1 + 45 / sq));
			if (!heightmoved[i, jmin])
				heightmap[i, jmin] = ((jmin - mijl < 0) ? ((heightmap[imin, jmin] + heightmap[imax, jmin] + heightmap[i, jmin + mijl]) / 3) : ((heightmap[imin, jmin] + heightmap[imax, jmin] + heightmap[i, jmin + mijl] + heightmap[i, jmin - mijl]) / 4 + 1)) + ra.Next(1, (int)(1 + 45 / sq));
			if (!heightmoved[i, jmax])
				heightmap[i, jmax] = ((jmax + mijl >= width) ? ((heightmap[imin, jmax] + heightmap[imax, jmax] + heightmap[i, jmax - mijl]) / 3) : ((heightmap[imin, jmax] + heightmap[imax, jmax] + heightmap[i, jmax + mijl] + heightmap[i, jmax - mijl]) / 4 + 1)) + ra.Next(1, (int)(1 + 45 / sq));
			heightmoved[imin, j] = true;
			heightmoved[imax, j] = true;
			heightmoved[i, jmin] = true;
			heightmoved[i, jmax] = true;

		}

	}
}
