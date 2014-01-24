namespace MDM.Client.PerformanceTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class TestResults
    {
        private readonly Stopwatch stopWatch = new Stopwatch();
        private readonly IList<long> timings = new List<long>();
        private readonly int numberOfIterations;

        public TestResults(int numberOfIterations = 1)
        {
            this.numberOfIterations = numberOfIterations;
        }

        public void InvokeAndCollect(Action action)
        {
            for (var i = 0; i < numberOfIterations; i++)
            {
                stopWatch.Start();
                action();
                stopWatch.Stop();
                timings.Add(stopWatch.ElapsedMilliseconds);
                stopWatch.Reset();
            }
        }

        public int NumberOfIterations
        {
            get { return numberOfIterations; }
        }

        public long Min
        {
            get { return timings.Min(); }
        }

        public long Max
        {
            get { return timings.Max(); }
        }

        public long Average
        {
            get { return (long)timings.Average(); }
        }

        public long Sum
        {
            get { return timings.Sum(); }
        }
    }
}

