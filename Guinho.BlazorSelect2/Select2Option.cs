using System;
using System.Collections.Generic;

namespace Guinho.BlazorSelect2
{
    public class Select2Option<T> : IEquatable<T>
    {
        public Select2Option()
        {
        }

        public Select2Option(string label, T value)
        {
            Label = label;
            Value = value;
        }

        public string Label { get; set; }

        public T Value { get; set; }

        public bool Selected { get; private set; } = false;

        public void Select()
        {
            Selected = true;
        }

        public void Unselect()
        {
            Selected = false;
        }

        public bool Equals(T other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other);
        }
    }
}
