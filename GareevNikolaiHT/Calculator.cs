using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

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

        // --------------------- Форматтер ------------------------
        public static class BigIntFormatter
        {
            public static string FormatBigIntegerAsScientific(BigInteger value, int significantDigits = 3)
            {
                if (value == 0)
                    return "0";

                string s = BigInteger.Abs(value).ToString();
                int exponent = s.Length - 1;
                string firstDigits = s.Substring(0, Math.Min(significantDigits, s.Length));

                string formatted = firstDigits.Length > 1
                    ? $"{firstDigits[0]}.{firstDigits.Substring(1)}"
                    : firstDigits;

                string result = $"{formatted} × 10^{exponent}";
                if (value.Sign < 0)
                    result = "−" + result;

                return result;
            }
        }

        // ------------------- Методы вычислений -------------------
        private void DoFactorial(int n, Action<string, double> callback)
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            BigInteger varResult = BigInteger.One;
            for (int i = 1; i <= n; i++)
            {
                varResult *= i;
                if (i % 1000 == 0)
                    Thread.Yield();

                lock (_lock)
                {
                    varTotalCalculations++;
                }
            }

            string formatted = BigIntFormatter.FormatBigIntegerAsScientific(varResult);
            double total = varTotalCalculations;

            _stopwatch.Stop();
            LastOperationMilliseconds = _stopwatch.ElapsedMilliseconds;

            callback?.Invoke(formatted, total);
        }

        private void DoAddTwo()
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            int result = varAddTwo + 2;
            lock (_lock)
                varTotalCalculations++;

            _stopwatch.Stop();
            LastOperationMilliseconds = _stopwatch.ElapsedMilliseconds;

            AddTwoComplete?.Invoke(result, varTotalCalculations);
        }

        private void DoLoop()
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            for (int x = 1; x <= varLoopValue; x++)
            {
                for (int y = 1; y <= 500; y++)
                {
                    lock (_lock)
                        varTotalCalculations++;
                }
            }

            _stopwatch.Stop();
            LastOperationMilliseconds = _stopwatch.ElapsedMilliseconds;

            LoopComplete?.Invoke(varTotalCalculations, varLoopValue);
        }

        // ------------------- Task-реализация -------------------
        public void ChooseTask(int taskNumber)
        {
            switch (taskNumber)
            {
                case 1:
                    // Оборачиваем событие в лямбду — событие нельзя напрямую передать как Action
                    Task.Run(() => DoFactorial(varFact1, (s, d) => FactorialComplete?.Invoke(s, d)));
                    break;
                case 2:
                    Task.Run(() => DoFactorial(varFact2 - 1, (s, d) => FactorialMinusOneComplete?.Invoke(s, d)));
                    break;
                case 3:
                    Task.Run(() => DoAddTwo());
                    break;
                case 4:
                    Task.Run(() => DoLoop());
                    break;
            }
        }

    }
}
