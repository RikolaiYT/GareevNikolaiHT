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

        private int _threadsCompleted = 0; // для отслеживания завершения всех потоков

        // -------- Форматирование BigInteger как 1.23×10^N --------
        public static class BigIntFormatter
        {
            public static string FormatBigIntegerAsScientific(BigInteger value, int significantDigits = 3)
            {
                if (value == 0) return "0";

                string s = BigInteger.Abs(value).ToString();
                int exponent = s.Length - 1;
                string firstDigits = s.Substring(0, Math.Min(significantDigits, s.Length));
                string formatted = firstDigits.Length > 1
                    ? $"{firstDigits[0]}.{firstDigits.Substring(1)}"
                    : firstDigits;
                string result = $"{formatted} × 10^{exponent}";
                if (value.Sign < 0) result = "−" + result;
                return result;
            }
        }

        // ---------- Многопоточные методы ----------
        private void Factorial()
        {
            BigInteger result = BigInteger.One;
            for (int i = 1; i <= varFact1; i++)
            {
                result *= i;
                if (i % 1000 == 0) Thread.Yield();

                lock (_lock) varTotalCalculations++;
            }

            string formatted = BigIntFormatter.FormatBigIntegerAsScientific(result);
            FactorialComplete?.Invoke(formatted, varTotalCalculations);

            ThreadCompleted();
        }

        private void FactorialMinusOne()
        {
            BigInteger result = BigInteger.One;
            for (int i = 1; i <= varFact2 - 1; i++)
            {
                result *= i;
                if (i % 1000 == 0) Thread.Yield();

                lock (_lock) varTotalCalculations++;
            }

            string formatted = BigIntFormatter.FormatBigIntegerAsScientific(result);
            FactorialMinusOneComplete?.Invoke(formatted, varTotalCalculations);

            ThreadCompleted();
        }

        private void AddTwo()
        {
            int result = varAddTwo + 2;
            lock (_lock) varTotalCalculations++;

            AddTwoComplete?.Invoke(result, varTotalCalculations);

            ThreadCompleted();
        }

        private void RunALoop()
        {
            for (int x = 1; x <= varLoopValue; x++)
            {
                for (int y = 1; y <= 500; y++)
                {
                    lock (_lock) varTotalCalculations++;
                }
            }

            LoopComplete?.Invoke(varTotalCalculations, varLoopValue);

            ThreadCompleted();
        }




        // ---------- Метод отслеживания завершения потоков ----------
        public delegate void AllThreadsCompletedHandler(long elapsedMilliseconds);
        public event AllThreadsCompletedHandler AllThreadsCompleted;

        // ...

        private void ThreadCompleted()
        {
            lock (_lock)
            {
                _threadsCompleted++;
                if (_threadsCompleted == 4)
                {
                    _stopwatch.Stop();
                    LastOperationMilliseconds = _stopwatch.ElapsedMilliseconds;

                    AllThreadsCompleted?.Invoke(LastOperationMilliseconds);
                }
            }
        }


        // ---------- Один метод, запускающий все 4 потока ----------
        public void RunAllThreads()
        {
            varTotalCalculations = 0;
            _threadsCompleted = 0;

            _stopwatch.Reset();
            _stopwatch.Start(); // запускаем общий таймер для всех потоков

            Thread t1 = new Thread(Factorial);
            Thread t2 = new Thread(FactorialMinusOne);
            Thread t3 = new Thread(AddTwo);
            Thread t4 = new Thread(RunALoop);

            t1.IsBackground = true;
            t2.IsBackground = true;
            t3.IsBackground = true;
            t4.IsBackground = true;

            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();
        }
    }
}
