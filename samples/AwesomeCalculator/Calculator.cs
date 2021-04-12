using System;
using System.Collections.Generic;
using System.Linq;

namespace AwesomeCalculator
{
    public class Calculator
    {
        readonly Stack<Func<double, double>> _commandHistory = new();

        void Apply(Func<double, double> command) => _commandHistory.Push(command);

        public void Reset() => _commandHistory.Clear();
        public void Undo() => _commandHistory.Pop();
        public void Add(double number) => Apply(state => state + number);
        public void Subtract(double number) => Apply(state => state - number);
        public void Multiply(double number) => Apply(state => state * number);
        public void Divide(double number) => Apply(state => number == 0 ? double.NaN : state / number);

        public bool HasError => double.IsNaN(State);
        public double State => _commandHistory.Aggregate(0d, (state, command) => command(state));
    }
}
