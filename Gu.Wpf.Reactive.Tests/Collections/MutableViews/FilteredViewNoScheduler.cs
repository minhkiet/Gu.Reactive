#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews
{
    using System;

    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;

    using NUnit.Framework;

    public class FilteredViewNoScheduler : FilterTests
    {
        [SetUp]
        public override void SetUp()
        {
            App.Start();
            this.Scheduler = new TestDispatcherScheduler();
            base.SetUp();
#pragma warning disable GU0036 // Don't dispose injected.
            this.view?.Dispose();
#pragma warning restore GU0036 // Don't dispose injected.
            this.view = this.ints.AsFilteredView(x => true, TimeSpan.Zero);
        }
    }
}