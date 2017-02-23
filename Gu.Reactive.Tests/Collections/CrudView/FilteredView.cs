namespace Gu.Reactive.Tests.Collections.CrudView
{
    using System;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class FilteredView : CrudViewTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
#pragma warning disable GU0036 // Don't dispose injected.
            this.View?.Dispose();
#pragma warning restore GU0036 // Don't dispose injected.
            this.View = this.Ints.AsFilteredView(x => true, TimeSpan.FromMilliseconds(10), this.Scheduler);
        }
    }
}