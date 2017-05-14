﻿namespace Gu.Reactive.Analyzers.Tests.GUREA04PreferSlimTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    internal class HappyPath
    {
        private const string FooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Foo : INotifyPropertyChanged
    {
        private int value1;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Value1
        {
            get
            {
                return this.value1;
            }

            set
            {
                if (value == this.value1)
                {
                    return;
                }

                this.value1 = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

        static HappyPath()
        {
            AnalyzerAssert.MetadataReference.AddRange(MetadataReferences.All);
        }

        [Test]
        public void WhenPassingSlimToConditionCtor()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                foo.ObservePropertyChangedSlim(x => x.Value1),
                () => foo.Value1 == 2)
        {
        }
    }
}";
            AnalyzerAssert.NoDiagnostics<GUREA04PreferSlim>(FooCode, testCode);
        }

        [Test]
        public void WhenSubscribingToSlimNotUsingArg()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            foo.ObservePropertyChangedSlim(x => x.Value1)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            AnalyzerAssert.NoDiagnostics<GUREA04PreferSlim>(FooCode, testCode);
        }

        [Test]
        public void WhenSubscribingToSlimUsingArg()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            foo.ObservePropertyChangedSlim(x => x.Value1)
               .Subscribe(x => Console.WriteLine(x.PropertyName));
        }
    }
}";
            AnalyzerAssert.NoDiagnostics<GUREA04PreferSlim>(FooCode, testCode);
        }
    }
}