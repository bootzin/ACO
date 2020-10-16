using System.Collections.Generic;
using System.Linq;

namespace ACO
{
	public class Ant
    {
        private readonly Graph Graph;
        private readonly int Beta;
        private readonly double Q0;

        public List<int> VisitedNodes { get; }
        public List<int> UnvisitedNodes { get; }
        public List<Edge> Path { get; }
        public double Distance { get; private set; }

        public Ant(Graph graph, int beta, double q0, int rand)
        {
            Graph = graph;
            Beta = beta;
            Q0 = q0;
            Distance = 0d;

            VisitedNodes = new List<int> { rand };
            UnvisitedNodes = new List<int>();
            UnvisitedNodes.AddRange(Graph.Edges.Select(e => e.StartId).Distinct().Where(v => v != rand).ToList());
            Path = new List<Edge>();
        }

        public int CurrentNode => VisitedNodes[^1];
        public bool CanMove { get; set; } = true;

        public Edge Move(int it)
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
                    end = ChooseNode(it);
                    if (end == -1)
                    {
                        CanMove = false;
                        return null;
                    }
                    VisitedNodes.Add(end);
                    UnvisitedNodes.Remove(end);
                }

                edge = Graph.GetEdge(start, end, it);
                Path.Add(edge);
                Distance += edge.Length;
            }
            return edge;
        }

        private int ChooseNode(int it)
        {
            List<Edge> edgesWithWeight = new List<Edge>();
            Edge bestEdge = new Edge();

            foreach (int node in UnvisitedNodes)
            {
                Edge edge = Graph.GetEdge(CurrentNode, node, it);
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

            double rand;
            lock (Utils.Random)
                rand = Utils.Random.NextDouble();
            if (rand > Q0)
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
            double rand;
            lock (Utils.Random)
                rand = Utils.Random.NextDouble();
            return edgeProbabilities.First(e => e.Weight >= rand).EndId;
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
