namespace ACO
{
    public class Edge
    {
        public int StartId { get; set; }
        public int EndId { get; set; }
        public int Length { get; set; }
        public double Pheromone { get; set; }
        public double Weight { get; set; }

        public Edge() { }

        public Edge(int startId, int endId, int length)
        {
            StartId = startId;
            EndId = endId;
            Length = length;
        }
    }
}
