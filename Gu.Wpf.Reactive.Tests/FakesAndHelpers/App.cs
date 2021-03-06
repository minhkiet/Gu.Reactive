namespace Gu.Wpf.Reactive.Tests.FakesAndHelpers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    using Gu.Reactive.Internals;
    using NUnit.Framework;

    public class App : Application
    {
        public static void Start()
        {
            if (Current == null)
            {
                Task.Run(
                        () =>
                        {
                            var app = new App { ShutdownMode = ShutdownMode.OnExplicitShutdown };
                            app.Run().IgnoreReturnValue();
                        })
                    .IgnoreReturnValue();
                Assert.AreEqual(true, SpinWait.SpinUntil(() => Current != null, TimeSpan.FromMilliseconds(100)));
                Assert.NotNull(Current, nameof(Current));
            }
        }
    }
}
