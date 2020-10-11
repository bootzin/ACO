using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ACO
{
    class Program
    {
        static void Main(string[] args)
        {
            Graph g = LoadGraphFromFile(args[0]);
            g.Edges.ForEach(e => g.GetEdge(e.StartId, e.EndId));
            int vertices = g.Edges.Select(e => e.StartId).Distinct().Count();
            Parameters parameters = new Parameters()
            {
                T0 = 1d / vertices,
                AntCount = vertices,
                Iterations = 1500,
                Beta = 4,
                GlobalEvaporationRate = 0.1,
                LocalEvaporationRate = 0.01,
                Q0 = 0.7,
            };

            AntColony colony = new AntColony(g, parameters);

            List<Ant> bestsies = new List<Ant>();
            for (int i = 0; i < 30; i++)
            {
                colony.Run();
                bestsies.Add(colony.GlobalBestAnt);
                LogRun(colony.GlobalBestAnt, colony.Stopwatch.Elapsed.TotalSeconds, i);
            }

            bestsies = bestsies.OrderBy(a => a.Distance).ToList();
            var avg = bestsies.Average(a => a.Distance);
            var std = bestsies.StdDev();
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Best ant distance after 30 runs: " + bestsies[^1].Distance);
            Console.WriteLine("Average ant distance: " + Math.Round(avg, 3));
            Console.WriteLine("StdDev: " + Math.Round(std, 3));
            Console.WriteLine("Confidence Interval (95%): [" + Math.Round(avg - (1.96 * std), 2) + "," + Math.Round(avg + (1.96 * std), 2) + "]");
            Console.WriteLine("Confidence Interval (99%): [" + Math.Round(avg - (2.576 * std), 2) + "," + Math.Round(avg + (2.576 * std), 2) + "]");
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

        private static Graph LoadGraphFromFile(string file)
        {
            using StreamReader sr = new StreamReader(file);
            Graph g = new Graph();
            while (!sr.EndOfStream)
            {
                string[] edgeStr = sr.ReadLine().Split('\t', StringSplitOptions.RemoveEmptyEntries);
                Edge e = new Edge(int.Parse(edgeStr[0]), int.Parse(edgeStr[1]), int.Parse(edgeStr[2]));
                g.AddEdge(e);
            }
            return g;
        }
    }
}
