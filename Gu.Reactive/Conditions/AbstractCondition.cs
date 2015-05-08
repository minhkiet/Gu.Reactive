﻿namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// This is a baseclass when you want to have a nonstatic Criteria method
    /// </summary>
    public abstract class AbstractCondition : ICondition
    {
        private readonly Condition _condition;

        protected AbstractCondition(IObservable<object> observable)
        {
            _condition = new Condition(observable, Criteria);
            _condition.ObservePropertyChanged()
                      .Subscribe(x => OnPropertyChanged(x.EventArgs));
            Name = _condition.Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool? IsSatisfied
        {
            get { return _condition.IsSatisfied; }
        }

        public string Name { get; protected set; }

        public IEnumerable<ICondition> Prerequisites
        {
            get { return _condition.Prerequisites; }
        }

        public IEnumerable<ConditionHistoryPoint> History
        {
            get { return _condition.History; }
        }

        public ICondition Negate()
        {
            return _condition.Negate();
        }

        public void Dispose()
        {
            _condition.Dispose();
        }

        protected abstract bool? Criteria();

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}