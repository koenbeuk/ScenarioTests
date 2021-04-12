using System;
using System.Collections.Generic;
using System.Linq;

namespace AwesomeCalculator
{
    public class Calculator
    {
        readonly Stack<Func<decimal, decimal>> _commandHistory = new();

        void Apply(Func<decimal, decimal> command) => _commandHistory.Push(command);

        public void Reset() => _commandHistory.Clear();
        public void Undo() => _commandHistory.Pop();
        public void Add(decimal number) => Apply(state => state + number);
        public void Subtract(decimal number) => Apply(state => state - number);
        public void Multiply(decimal number) => Apply(state => state * number);
        public void Divide(decimal number) => Apply(state => state / number);

        public decimal State => _commandHistory.Aggregate(0m, (state, command) => command(state));
    }
}
