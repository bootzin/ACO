using System;

namespace ACO
{
    public class Parameters
    {
        public int Beta { get; set; }
        public double GlobalEvaporationRate { get; set; }
        public double LocalEvaporationRate { get; set; }
        public double Q0 { get; set; }
        public double T0 { get; set; }
        public int AntCount { get; set; }
        public int Iterations { get; set; }

        public Parameters(int Beta, double GlobalEvaporationRate, double LocalEvaporationRate, double T0, double Q0, int AntCount, int Iterations)
        {
            this.Beta = Beta;
            this.GlobalEvaporationRate = GlobalEvaporationRate;
            this.LocalEvaporationRate = LocalEvaporationRate;
            this.T0 = T0;
            this.Q0 = Q0;
            this.AntCount = AntCount;
            this.Iterations = Iterations;
        }

        public Parameters()
        {
            Beta = 2;
            GlobalEvaporationRate = 0.1;
            LocalEvaporationRate = 0.01;
            Q0 = 0.7;
            AntCount = 20;
            Iterations = 10000;
            T0 = 0.01;
        }

        public void Show()
        {
            Console.WriteLine("Beta: " + Beta);
            Console.WriteLine("Global Evaporation Rate: " + GlobalEvaporationRate);
            Console.WriteLine("Local Evaporation Rate: " + LocalEvaporationRate);
            Console.WriteLine("Q0: " + Q0);
            Console.WriteLine("AntCount: " + AntCount);
            Console.WriteLine("Iterations: " + Iterations);
            Console.WriteLine("T0: " + T0);
            Console.WriteLine();
        }
    }
}
