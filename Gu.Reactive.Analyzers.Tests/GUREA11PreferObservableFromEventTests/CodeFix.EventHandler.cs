namespace Gu.Reactive.Analyzers.Tests.GUREA11PreferObservableFromEventTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public partial class CodeFix
    {
        public class EventHandler
        {
            private const string FooCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public event EventHandler SomeEvent;
    }
}";

            [Test]
            public void WhenNotUsingSenderNorArgLambda()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            ↓foo.SomeEvent += (sender, i) => Console.WriteLine(string.Empty);
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Linq;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            Observable.FromEvent<EventHandler, EventArgs>(
                h => (_, e) => h(e),
                h => foo.SomeEvent += h,
                h => foo.SomeEvent -= h)
                      .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
                AnalyzerAssert.CodeFix(Analyzer, Fix, new[] { FooCode, testCode }, fixedCode);
            }

            [Test]
            public void WhenUsingArgLambda()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Linq;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            ↓foo.SomeEvent += (sender, e) => Console.WriteLine(e);
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Linq;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            Observable.FromEvent<EventHandler, EventArgs>(
                h => (_, e) => h(e),
                h => foo.SomeEvent += h,
                h => foo.SomeEvent -= h)
                      .Subscribe(e => Console.WriteLine(e));
        }
    }
}";
                AnalyzerAssert.CodeFix(Analyzer, Fix, new[] { FooCode, testCode }, fixedCode);
            }

            [Test]
            public void WhenNotUsingSenderNorArgMethodGroup()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            ↓foo.SomeEvent += this.OnSomeEvent;
        }

        private void OnSomeEvent(object sender, EventArgs e)
        {
            Console.WriteLine(string.Empty);
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Linq;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            Observable.FromEvent<EventHandler, EventArgs>(
                h => (_, e) => h(e),
                h => foo.SomeEvent += h,
                h => foo.SomeEvent -= h)
                      .Subscribe(_ => OnSomeEvent());
        }

        private void OnSomeEvent()
        {
            Console.WriteLine(string.Empty);
        }
    }
}";
                AnalyzerAssert.CodeFix(Analyzer, Fix, new[] { FooCode, testCode }, fixedCode);
            }

            [Test]
            public void WhenUsingArgMethodGroup()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            ↓foo.SomeEvent += this.OnSomeEvent;
        }

        private void OnSomeEvent(object sender, EventArgs e)
        {
            Console.WriteLine(e);
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Linq;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            Observable.FromEvent<EventHandler, EventArgs>(
                h => (_, e) => h(e),
                h => foo.SomeEvent += h,
                h => foo.SomeEvent -= h)
                      .Subscribe(OnSomeEvent);
        }

        private void OnSomeEvent(EventArgs e)
        {
            Console.WriteLine(e);
        }
    }
}";
                AnalyzerAssert.CodeFix(Analyzer, Fix, new[] { FooCode, testCode }, fixedCode);
            }
        }
    }
}
