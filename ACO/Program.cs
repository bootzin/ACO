using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ACO
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			List<Ant> bestsies = new List<Ant>();
			Parameters parameters = new Parameters();
			if (!int.TryParse(args.ElementAtOrDefault(8), out int numRuns))
			{
				numRuns = 30;
			}
			Parallel.For(0, numRuns, i =>
			{
				Graph g = LoadGraphFromFile(args[0], i);
				int vertices = g.VerticesCount;

				if (args.Length > 1)
				{
					parameters = new Parameters()
					{
						T0 = float.Parse(args[1].Replace(',','.'), NumberStyles.Float, CultureInfo.InvariantCulture),
						AntCount = int.Parse(args[2]),
						Iterations = int.Parse(args[3]),
						Beta = int.Parse(args[4]),
						GlobalEvaporationRate = double.Parse(args[5].Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture),
						LocalEvaporationRate = double.Parse(args[6].Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture),
						Q0 = double.Parse(args[7].Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture),
					};
				}
				else
				{
					parameters = new Parameters()
					{
						T0 = .01,
						AntCount = vertices,
						Iterations = 50,
						Beta = 4,
						GlobalEvaporationRate = 0.1,
						LocalEvaporationRate = 0.01,
						Q0 = 0.9,
					};
				}

				AntColony colony = new AntColony(g, parameters);
				colony.Run(i);
				lock (bestsies)
				{
					bestsies.Add(colony.GlobalBestAnt);
					LogRun(colony.GlobalBestAnt, colony.Stopwatch.Elapsed.TotalSeconds, i);
				}
			});

			bestsies = bestsies.OrderBy(a => a.Distance).ToList();
			var avg = bestsies.Average(a => a.Distance);
			var std = bestsies.StdDev();
			Console.WriteLine("");
			Console.WriteLine("");
			Console.WriteLine("Best ant distance after 30 runs: " + bestsies[^1].Distance);
			Console.WriteLine("Average ant distance: " + Math.Round(avg, 3));
			Console.WriteLine("StdDev: " + Math.Round(std, 3));
			Console.WriteLine("Confidence Interval (95%): [" + Math.Round(avg - (1.96 * std), 2) + ";" + Math.Round(avg + (1.96 * std), 2) + "]");
			Console.WriteLine("Confidence Interval (99%): [" + Math.Round(avg - (2.576 * std), 2) + ";" + Math.Round(avg + (2.576 * std), 2) + "]");
			parameters.Show();
			Console.WriteLine("Best ant path an weights:");
			LogPath(bestsies[^1]);
		}

		private static void LogRun(Ant best, double time, int i)
		{
			Console.WriteLine($"Run {i} finished in {time} seconds!");
			Console.WriteLine("Best Distance: " + best.Distance);
			LogPath(best);
		}

		private static void LogPath(Ant best)
		{
			Console.WriteLine("Ant Path:");
			Console.Write("[");
			best.Path.ForEach(e => Console.Write(e.StartId + ", "));
			Console.Write(best.Path[^1].EndId);
			Console.Write("]");
			Console.WriteLine();
			Console.WriteLine("Path Weights:");
			Console.Write("[");
			Console.Write(best.Path[0].Length);
			for (int i = 1; i < best.Path.Count; i++)
			{
				Console.Write(", " + best.Path[i].Length);
			}
			Console.Write("]");
			Console.WriteLine();
		}

		private static Graph LoadGraphFromFile(string file, int it)
		{
			using StreamReader sr = new StreamReader(file);
			Graph g = new Graph();
			while (!sr.EndOfStream)
			{
				string[] edgeStr = sr.ReadLine().Split('\t', StringSplitOptions.RemoveEmptyEntries);
				Edge e = new Edge(int.Parse(edgeStr[0]), int.Parse(edgeStr[1]), int.Parse(edgeStr[2]));
				g.AddEdge(e, it);
			}
			g.VerticesCount = g.Edges.Select(e => e.StartId).Distinct().Count();
			return g;
		}
	}
}
