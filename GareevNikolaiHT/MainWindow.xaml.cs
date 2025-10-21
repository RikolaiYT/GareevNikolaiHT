using GareevNikolaiHT;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MultithreadCalculator
{
    public partial class MainWindow : Window
    {
        private Calculator Calculator1;

        public MainWindow()
        {
            InitializeComponent();

            // Создаем экземпляр калькулятора и подписываемся на события
            Calculator1 = new Calculator();
            Calculator1.FactorialComplete += Calculator1_FactorialComplete;
            Calculator1.FactorialMinusOneComplete += Calculator1_FactorialMinusOneComplete;
            Calculator1.AddTwoComplete += Calculator1_AddTwoComplete;
            Calculator1.LoopComplete += Calculator1_LoopComplete;
  

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

   

            btnRunAll.IsEnabled = false;

            Calculator1.RunAllThreads(); // запускаем все 4 потока
        }


        private void Calculator1_FactorialComplete(string Value, double Calculations)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblFactorial1.Content = Value;
                lblTotalCalculations.Content = $"Всего вычислений — {Calculations}";
                lblLastElapsed.Content = $"{Calculator1.LastOperationMilliseconds} ms";
            }));
        }

        private void Calculator1_FactorialMinusOneComplete(string Value, double Calculations)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblFactorial2.Content = Value;
                lblTotalCalculations.Content = $"Всего вычислений — {Calculations}";
                lblLastElapsed.Content = $"{Calculator1.LastOperationMilliseconds} ms";
            }));
        }

        private void Calculator1_AddTwoComplete(int Value, double Calculations)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblAddTwo.Content = Value.ToString();
                lblTotalCalculations.Content = $"Всего вычислений — {Calculations}";
                lblLastElapsed.Content = $"{Calculator1.LastOperationMilliseconds} ms";
            }), DispatcherPriority.Normal);
        }

        private void Calculator1_LoopComplete(double Calculations, int Count)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblRunLoops.Content = Count.ToString();
                lblTotalCalculations.Content = $"Всего вычислений — {Calculations}";
                lblLastElapsed.Content = $"{Calculator1.LastOperationMilliseconds} ms";

                // Все потоки завершены — можно разблокировать кнопку
                btnRunAll.IsEnabled = true;
            }), DispatcherPriority.Normal);
        }
    }
}
