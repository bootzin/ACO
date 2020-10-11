using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace ACO
{
    public class Graph
    {
        public List<Edge> Edges { get; set; } = new List<Edge>();
        public MemoryCache Cache { get; set; }
        public double MinPheromone { get; set; }

        public Graph()
        {
            Cache = new MemoryCache(new MemoryCacheOptions());
        }

        public void AddEdge(Edge edge)
        {
            Cache.CreateEntry(edge.StartId + "_" + edge.EndId);
            Cache.Set(edge.StartId + "_" + edge.EndId, edge);
            Edges.Add(edge);
        }

        public Edge GetEdge(int startId, int endId)
        {
            Cache.TryGetValue(startId + "_" + endId, out Edge result);
            return result;
        }

        public void SetPheromone(double pheromone)
        {
            Edges.ForEach(e => e.Pheromone = pheromone);
        }

        public void Evaporate(Edge edge, double value)
        {
            edge.Pheromone = Math.Max(MinPheromone, edge.Pheromone * value);
        }

        public void Deposit(Edge edge, double value)
        {
            edge.Pheromone += value;
        }
    }
}
