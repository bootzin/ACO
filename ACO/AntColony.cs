using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ACO
{
    public sealed class AntColony
    {
        private readonly Graph Graph;
        private readonly Parameters Parameters;

        public Stopwatch Stopwatch { get; }
        public List<double> Results { get; }
        public Ant GlobalBestAnt { get; private set; }

        public AntColony(Graph g, Parameters parameters)
        {
            Graph = g;
            Graph.MinPheromone = parameters.T0;
            Parameters = parameters;
            Stopwatch = new Stopwatch();
            Results = new List<double>();
        }

        public void Run()
        {
            Stopwatch.Restart();
            Results.Clear();
            GlobalBestAnt = null;
            Graph.SetPheromone(Graph.MinPheromone);
            for (int it = 0; it < Parameters.Iterations; it++)
            {
                List<Ant> ants = CreateAnts();
                GlobalBestAnt ??= ants[0];

                Ant bestAnt = BuildSolutions(ants);

                if (bestAnt.Distance > GlobalBestAnt.Distance)
                {
                    GlobalBestAnt = bestAnt;
                    Console.WriteLine("Best ant updated with distance " + bestAnt.Distance + " on iteration " + it);
                }
                Results.Add(bestAnt.Distance);
            }
            Stopwatch.Stop();
        }

        private Ant BuildSolutions(List<Ant> ants)
        {
            for (int i = 0; i < Graph.Edges.Select(e => e.StartId).Distinct().Count(); i++)
            {
                foreach (Ant ant in ants)
                {
                    Edge edge = ant.Move();
                    if (edge != null)
                        LocalUpdate(edge);
                }
            }

            GlobalUpdate();

            return ants.OrderByDescending(x => x.Distance).FirstOrDefault();
        }

        private void GlobalUpdate()
        {
            double deltaR = 1 / GlobalBestAnt.Distance;
            foreach (Edge edge in GlobalBestAnt.Path)
            {
                double evaporate = (1 - Parameters.GlobalEvaporationRate);
                Graph.Evaporate(edge, evaporate);

                double deposit = Parameters.GlobalEvaporationRate * deltaR;
                Graph.Deposit(edge, deposit);
            }
        }

        private void LocalUpdate(Edge edge)
        {
            double evap = 1 - Parameters.LocalEvaporationRate;
            double deposit = Parameters.LocalEvaporationRate * Parameters.T0;

            Graph.Evaporate(edge, evap);
            Graph.Deposit(edge, deposit);
        }

        private List<Ant> CreateAnts()
        {
            List<Ant> ants = new List<Ant>();
            for (int i = 0; i < Parameters.AntCount; i++)
            {
                int rand = Utils.Random.Next(1, Graph.Edges.Select(e => e.StartId).Distinct().Count() + 1);
                Ant ant = new Ant(Graph, Parameters.Beta, Parameters.Q0, rand);
                ants.Add(ant);
            }
            return ants;
        }
    }
}