using System;
using System.Collections.Generic;

namespace JuanMartin.Kernel
{
    public class ValueHolder : ICloneable
    {
        private string _name;
        private Value _value;
        private List<ValueHolder> _annotations;

        public ValueHolder()
        {
            _name = string.Empty;
            _value = new Value();
            _annotations = new List<ValueHolder>();
        }

        public ValueHolder(string Name) : this()
        {
            _name = Name;
        }

        public ValueHolder(string Name, object Value)
            : this(Name)
        {
            _value = new Value(Value);
        }

        public ValueHolder(string Name, Value Value)
            : this(Name)
        {
            _value = Value;
        }

        public ValueHolder(ValueHolder Other)
        {
            this.Name = Other.Name;
            this.ValueContainer = (Value)Other.ValueContainer.Clone();
            _annotations = new List<ValueHolder>(Other.Annotations);
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public object Value
        {
            get { return _value.Result; }
            set { _value.Result = value; }
        }

        public Value ValueContainer
        {
            get { return _value; }
            set { _value = value; }
        }

        public List<ValueHolder> Annotations
        {
            get { return _annotations; }
        }

        public bool HasAnnotations()
        {
            return (_annotations.Count > 0);
        }

        public void AddAnnotations(List<ValueHolder> Annotations)
        {
            foreach (ValueHolder annotation in Annotations)
                AddAnnotation(annotation);
        }

        public string AddAnnotation(ValueHolder Annotation)
        {
            _annotations.Add(Annotation);

            return Annotation.Name;
        }

        public string AddAnnotation(string Name, object Value)
        {
            ValueHolder valueHolder = new ValueHolder(Name, Value);

            return AddAnnotation(valueHolder);
        }

        public ValueHolder GetAnnotation(string Name)
        {
            foreach (ValueHolder annotation in _annotations)
            {
                if (annotation.Name == Name)
                    return annotation;
            }

            return null;
        }

        public ValueHolder GetAnnotationByValue(object Value)
        {
            foreach (ValueHolder annotation in _annotations)
            {
                if (annotation.Value.Equals(Value))
                    return annotation;
            }

            return null;
        }

        public void UpdateAnnotation(string Name, object Value)
        {
            ValueHolder current = this.GetAnnotation(Name);

            if (current != null)
            {
                current.Value = Value;
            }
        }

        public void RemoveAnnotation(string Name)
        {
            ValueHolder current = this.GetAnnotation(Name);

            if (current != null)
            {
                _annotations.Remove(current);
            }
        }

        public override string ToString()
        {
            return $"{Name}:{Value}";
        }
        #region Clonable interface methods
        public object Clone()
        {
            return new ValueHolder(this);
        }
        #endregion
    }
}
