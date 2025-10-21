using GareevNikolaiHT;
using System;
using System.Windows;
using System.Windows.Threading;

namespace MultithreadCalculator
{
    public partial class MainWindow : Window
    {
        private Calculator Calculator1;

        public MainWindow()
        {
            InitializeComponent();

            Calculator1 = new Calculator();
            Calculator1.FactorialComplete += Calculator1_FactorialComplete;
            Calculator1.FactorialMinusOneComplete += Calculator1_FactorialMinusOneComplete;
            Calculator1.AddTwoComplete += Calculator1_AddTwoComplete;
            Calculator1.LoopComplete += Calculator1_LoopComplete;
            Calculator1.AllThreadsCompleted += Calculator1_AllThreadsCompleted; // новое событие

            lblTotalCalculations.Content = "Всего вычислений — 0";
            lblLastElapsed.Content = "—";
        }

        private void btnRunAll_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtValue.Text, out int v))
            {
                MessageBox.Show("Введите целое число");
                return;
            }

            Calculator1.varFact1 = v;
            Calculator1.varFact2 = v;
            Calculator1.varAddTwo = v;
            Calculator1.varLoopValue = v;

            btnRunAll.IsEnabled = false; // блокируем кнопку до завершения всех потоков

            Calculator1.RunAllThreads();
        }

        private void Calculator1_FactorialComplete(string Value, double Calculations)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblFactorial1.Content = Value;
                lblTotalCalculations.Content = $"Всего вычислений — {Calculations}";
            }));
        }

        private void Calculator1_FactorialMinusOneComplete(string Value, double Calculations)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblFactorial2.Content = Value;
                lblTotalCalculations.Content = $"Всего вычислений — {Calculations}";
            }));
        }

        private void Calculator1_AddTwoComplete(int Value, double Calculations)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblAddTwo.Content = Value.ToString();
                lblTotalCalculations.Content = $"Всего вычислений — {Calculations}";
            }));
        }

        private void Calculator1_LoopComplete(double Calculations, int Count)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblRunLoops.Content = Count.ToString();
                lblTotalCalculations.Content = $"Всего вычислений — {Calculations}";
            }));
        }

        private void Calculator1_AllThreadsCompleted(long elapsedMilliseconds)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblLastElapsed.Content = $"{elapsedMilliseconds} ms"; // показываем суммарное время
                btnRunAll.IsEnabled = true; // разблокируем кнопку после завершения всех потоков
            }));
        }
    }
}
