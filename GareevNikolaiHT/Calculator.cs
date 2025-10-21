using System;
using System.Numerics;

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

        private readonly StopwatchTimer _stopwatch = new StopwatchTimer();

        // -------- Форматирование BigInteger как 1.23×10^N --------
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

        // ---------- ОДНОПОТОЧНЫЕ вычисления ----------
        public void Factorial()
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            BigInteger result = BigInteger.One;
            for (int i = 1; i <= varFact1; i++)
            {
                result *= i;
                varTotalCalculations++;
            }

            _stopwatch.Stop();
            LastOperationMilliseconds = _stopwatch.ElapsedMilliseconds;

            string formatted = BigIntFormatter.FormatBigIntegerAsScientific(result);
            FactorialComplete?.Invoke(formatted, varTotalCalculations);
        }

        public void FactorialMinusOne()
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            BigInteger result = BigInteger.One;
            for (int i = 1; i <= varFact2 - 1; i++)
            {
                result *= i;
                varTotalCalculations++;
            }

            _stopwatch.Stop();
            LastOperationMilliseconds = _stopwatch.ElapsedMilliseconds;

            string formatted = BigIntFormatter.FormatBigIntegerAsScientific(result);
            FactorialMinusOneComplete?.Invoke(formatted, varTotalCalculations);
        }

        public void AddTwo()
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            int result = varAddTwo + 2;
            varTotalCalculations++;

            _stopwatch.Stop();
            LastOperationMilliseconds = _stopwatch.ElapsedMilliseconds;

            AddTwoComplete?.Invoke(result, varTotalCalculations);
        }

        public void RunALoop()
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            for (int x = 1; x <= varLoopValue; x++)
            {
                for (int y = 1; y <= 500; y++)
                {
                    varTotalCalculations++;
                }
            }

            _stopwatch.Stop();
            LastOperationMilliseconds = _stopwatch.ElapsedMilliseconds;

            LoopComplete?.Invoke(varTotalCalculations, varLoopValue);
        }

        // --- Вместо потоков: просто вызывает метод напрямую ---
        public void ChooseThreads(int threadNumber)
        {
            switch (threadNumber)
            {
                case 1:
                    Factorial();
                    break;
                case 2:
                    FactorialMinusOne();
                    break;
                case 3:
                    AddTwo();
                    break;
                case 4:
                    RunALoop();
                    break;
            }
        }
    }
}
