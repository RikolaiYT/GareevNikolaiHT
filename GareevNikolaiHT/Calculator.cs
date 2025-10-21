using System;
using System.Numerics;
using System.Threading;


namespace MultithreadCalculator
{
    public class Calculator
    {
        public int varAddTwo;
        public int varFact1;
        public int varFact2;
        public int varLoopValue;
        public double varTotalCalculations = 0;

        public Thread FactorialThread;
        public Thread FactorialMinusOneThread;
        public Thread AddTwoThread;
        public Thread LoopThread;

        public delegate void FactorialCompleteHandler(string Result, double TotalCalculations);
        public delegate void AddTwoCompleteHandler(int Result, double TotalCalculations);
        public delegate void LoopCompleteHandler(double TotalCalculations, int Counter);

        public event FactorialCompleteHandler FactorialComplete;
        public event FactorialCompleteHandler FactorialMinusOneComplete;
        public event AddTwoCompleteHandler AddTwoComplete;
        public event LoopCompleteHandler LoopComplete;

        public long LastOperationMilliseconds { get; private set; } = 0;

        private readonly object _lock = new object();
        private readonly StopwatchTimer _stopwatch = new StopwatchTimer();

        public void FactorialMinusOne()
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            BigInteger varResult = BigInteger.One;
            double varTotalAsOfNow = 0;

            for (int varX = 1; varX <= varFact2 - 1; varX++)
            {
                varResult *= varX;
                if (varX % 1000 == 0) // чтобы не блокировать CPU полностью
                {
                    Thread.Yield();
                }

                lock (_lock)
                {
                    varTotalCalculations += 1;
                    varTotalAsOfNow = varTotalCalculations;
                }
            }

            _stopwatch.Stop();
            LastOperationMilliseconds = _stopwatch.ElapsedMilliseconds;

            FactorialMinusOneComplete?.Invoke(varResult.ToString(), varTotalAsOfNow);
        }

        public void Factorial()
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            BigInteger varResult = BigInteger.One;
            double varTotalAsOfNow = 0;

            for (int varX = 1; varX <= varFact1; varX++)
            {
                varResult *= varX;
                if (varX % 1000 == 0)
                {
                    Thread.Yield();
                }

                lock (_lock)
                {
                    varTotalCalculations += 1;
                    varTotalAsOfNow = varTotalCalculations;
                }
            }

            _stopwatch.Stop();
            LastOperationMilliseconds = _stopwatch.ElapsedMilliseconds;

            FactorialComplete?.Invoke(varResult.ToString(), varTotalAsOfNow);
        }

        public void AddTwo()
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            double varTotalAsOfNow = 0;
            int varResult = varAddTwo + 2;
            lock (_lock)
            {
                varTotalCalculations += 1;
                varTotalAsOfNow = varTotalCalculations;
            }

            _stopwatch.Stop();
            LastOperationMilliseconds = _stopwatch.ElapsedMilliseconds;

            AddTwoComplete?.Invoke(varResult, varTotalAsOfNow);
        }

        public void RunALoop()
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            double varTotalAsOfNow = 0;
            for (int varX = 1; varX <= varLoopValue; varX++)
            {
                for (int varY = 1; varY <= 500; varY++)
                {
                    lock (_lock)
                    {
                        varTotalCalculations += 1;
                        varTotalAsOfNow = varTotalCalculations;
                    }
                }
            }

            _stopwatch.Stop();
            LastOperationMilliseconds = _stopwatch.ElapsedMilliseconds;

            LoopComplete?.Invoke(varTotalAsOfNow, varLoopValue);
        }

        public void ChooseThreads(int threadNumber)
        {
            switch (threadNumber)
            {
                case 1:
                    FactorialThread = new Thread(new ThreadStart(this.Factorial));
                    FactorialThread.IsBackground = true;
                    FactorialThread.Start();
                    break;
                case 2:
                    FactorialMinusOneThread = new Thread(new ThreadStart(this.FactorialMinusOne));
                    FactorialMinusOneThread.IsBackground = true;
                    FactorialMinusOneThread.Start();
                    break;
                case 3:
                    AddTwoThread = new Thread(new ThreadStart(this.AddTwo));
                    AddTwoThread.IsBackground = true;
                    AddTwoThread.Start();
                    break;
                case 4:
                    LoopThread = new Thread(new ThreadStart(this.RunALoop));
                    LoopThread.IsBackground = true;
                    LoopThread.Start();
                    break;
            }
        }
    }
}
