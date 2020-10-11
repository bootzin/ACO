using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACO
{
    public class Ant
    {
        private Graph Graph;
        private int Beta;
        private double Q0;
        private int StartNodeId;

        public List<int> VisitedNodes { get; }
        public List<int> UnvisitedNodes { get; }
        public List<Edge> Path { get; }
        public double Distance { get; private set; }

        public Ant(Graph graph, int beta, double q0, int rand)
        {
            Graph = graph;
            Beta = beta;
            Q0 = q0;
            StartNodeId = rand;
            Distance = 0d;

            VisitedNodes = new List<int> { rand };
            UnvisitedNodes = new List<int>();
            UnvisitedNodes.AddRange(Graph.Edges.Select(e => e.StartId).Distinct().Where(v => v != rand).ToList());
            Path = new List<Edge>();
        }

        public int CurrentNode => VisitedNodes[^1];
        public bool CanMove { get; set; } = true;

        public Edge Move()
        {
            Edge edge = null;
            if (CanMove)
            {
                int start = CurrentNode;
                int end;
                if (UnvisitedNodes.Count == 0)
                {
                    CanMove = false;
                    return null;
                }
                else
                {
                    end = ChooseNode();
                    if (end == -1)
                    {
                        CanMove = false;
                        return null;
                    }
                    VisitedNodes.Add(end);
                    UnvisitedNodes.Remove(end);
                }

                edge = Graph.GetEdge(start, end);
                Path.Add(edge);
                Distance += edge.Length;
            }
            return edge;
        }

        private int ChooseNode()
        {
            List<Edge> edgesWithWeight = new List<Edge>();
            Edge bestEdge = new Edge();

            foreach (int node in UnvisitedNodes)
            {
                Edge edge = Graph.GetEdge(CurrentNode, node);
                if (edge == null)
                    continue;
                edge.Weight = Eval(edge);

                if (edge.Weight > bestEdge.Weight)
                {
                    bestEdge = edge;
                }

                edgesWithWeight.Add(edge);
            }

            if (edgesWithWeight.Count == 0)
            {
                return -1;
            }

            if (Utils.Random.NextDouble() > Q0)
                return Exploit(bestEdge);
            return Explore(edgesWithWeight);
        }

        private int Explore(List<Edge> edgesWithWeight)
        {
            double total = edgesWithWeight.Sum(e => e.Weight);
            List<Edge> edgeProbabilities = edgesWithWeight;
            edgeProbabilities.ForEach(e =>
            {
                e.Weight /= total;
            });
            double sum = 0;
            foreach (var e in edgeProbabilities)
            {
                sum += e.Weight;
                e.Weight = sum;
            }
            return edgeProbabilities.First(e => e.Weight >= Utils.Random.NextDouble()).EndId;
        }

        private int Exploit(Edge bestEdge)
        {
            return bestEdge.EndId;
        }

        private double Eval(Edge edge)
        {
            int eta = edge.Length;
            return edge.Pheromone * Utils.Pow(eta, Beta);
        }
    }
}
