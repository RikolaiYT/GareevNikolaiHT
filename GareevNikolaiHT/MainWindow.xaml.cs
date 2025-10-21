using GareevNikolaiHT;
using System;
using System.Windows;
using System.Windows.Threading;

namespace MultithreadCalculator
{
    public partial class MainWindow : Window
    {
        // Делегаты для BeginInvoke/Dispatcher marshaling
        public delegate void FHandler(double Value, double Calculations);
        public delegate void A2Handler(int Value, double Calculations);
        public delegate void LDHandler(double Calculations, int Count);

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

            // При желании можно инициализировать текстовые поля
            lblTotalCalculations.Content = "Всего вычислений — 0";
            lblLastElapsed.Content = "—";
        }

        private void btnFactorial1_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtValue.Text, out int v)) { MessageBox.Show("Введите целое число"); return; }
            Calculator1.varFact1 = v;
            btnFactorial1.IsEnabled = false;
            // старт потока 1
            Calculator1.ChooseTask(1);
        }

        private void btnFactorial2_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtValue.Text, out int v)) { MessageBox.Show("Введите целое число"); return; }
            Calculator1.varFact2 = v;
            btnFactorial2.IsEnabled = false;
            Calculator1.ChooseTask(2);
        }

        private void btnAddTwo_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtValue.Text, out int v)) { MessageBox.Show("Введите целое число"); return; }
            Calculator1.varAddTwo = v;
            btnAddTwo.IsEnabled = false;
            Calculator1.ChooseTask(3);
        }

        private void btnRunLoops_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtValue.Text, out int v) || v < 0) { MessageBox.Show("Введите неотрицательное целое число"); return; }
            Calculator1.varLoopValue = v;
            btnRunLoops.IsEnabled = false;
            lblRunLoops.Content = "Looping";
            Calculator1.ChooseTask(4);
        }

        // Обработчики событий компонента — они вызываются из НЕ-UI потока,
        // поэтому поместим обновление UI через Dispatcher.BeginInvoke (WPF эквивалент BeginInvoke in WinForms).
        private void Calculator1_FactorialComplete(string Value, double Calculations)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblFactorial1.Content = Value;
                btnFactorial1.IsEnabled = true;
                lblTotalCalculations.Content = $"Всего вычислений — {Calculations}";
                lblLastElapsed.Content = $"{Calculator1.LastOperationMilliseconds} ms";
            }));
        }

        private void Calculator1_FactorialMinusOneComplete(string Value, double Calculations)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblFactorial2.Content = Value;
                btnFactorial2.IsEnabled = true;
                lblTotalCalculations.Content = $"Всего вычислений — {Calculations}";
                lblLastElapsed.Content = $"{Calculator1.LastOperationMilliseconds} ms";
            }));
        }


        private void Calculator1_AddTwoComplete(int Value, double Calculations)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblAddTwo.Content = Value.ToString();
                btnAddTwo.IsEnabled = true;
                lblTotalCalculations.Content = $"Всего вычислений — {Calculations}";
                lblLastElapsed.Content = $"{Calculator1.LastOperationMilliseconds} ms";
            }), DispatcherPriority.Normal);
        }

        private void Calculator1_LoopComplete(double Calculations, int Count)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                btnRunLoops.IsEnabled = true;
                lblRunLoops.Content = Count.ToString();
                lblTotalCalculations.Content = $"Всего вычислений — {Calculations}";
                lblLastElapsed.Content = $"{Calculator1.LastOperationMilliseconds} ms";
            }), DispatcherPriority.Normal);
        }
    }
}
