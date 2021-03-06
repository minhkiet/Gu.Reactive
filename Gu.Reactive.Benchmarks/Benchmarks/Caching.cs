namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using BenchmarkDotNet.Attributes;
    using Gu.Reactive.Internals;

    public static class Caching
    {
        private static readonly IReadOnlyList<string> Strings = Enumerable.Range(0, 1000).Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
        private static readonly Expression<Func<Fake, int>> SingleItemPath = x => x.Value;
        private static readonly Expression<Func<Fake, int>> TwoItemPath = x => x.Next.Value;
        private static readonly ConcurrentBag<IdentitySet<string>> Bag = new ConcurrentBag<IdentitySet<string>>(new[] { new IdentitySet<string>(), });
        private static readonly PropertyInfo Property = typeof(Fake).GetProperty(nameof(Fake.Next), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        [Benchmark(Baseline = true)]
        public static int StringGetHashCode()
        {
            return "x => x.Value".GetHashCode();
        }

        [Benchmark]
        public static object NewSetPoolIdentitySet()
        {
            return new IdentitySet<string>();
        }

        [Benchmark]
        public static object NewSetPoolIdentitySetUnionWithStrings()
        {
            var set = new IdentitySet<string>();
            set.UnionWith(Strings);
            set.Clear();
            return set;
        }

        [Benchmark]
        public static object SetPoolBorrowReturn()
        {
            var set = SetPool.Borrow<string>();
            SetPool.Return(set);
            return set;
        }

        [Benchmark]
        public static object TryTakeAdd()
        {
            if (Bag.TryTake(out IdentitySet<string> set))
            {
                Bag.Add(set);
            }

            return set;
        }

        [Benchmark]
        public static object NewSetPoolBorrowReturnUnionWithStrings()
        {
            var set = SetPool.Borrow<string>();
            set.UnionWith(Strings);
            SetPool.Return(set);
            return set;
        }

        [Benchmark]
        public static Expression<Func<Fake, int>> OneLevelExpression()
        {
            return x => x.Value;
        }

        [Benchmark]
        public static Expression<Func<Fake, int>> TwoLevelExpression()
        {
            return x => x.Next.Value;
        }

        [Benchmark]
        public static int PropertyPathComparerGetHashCodeSingleItemPath()
        {
            return ((IEqualityComparer<LambdaExpression>)PropertyPathComparer.Default).GetHashCode(SingleItemPath);
        }

        [Benchmark]
        public static int PropertyPathComparerGetHashCodeTwoItemPath()
        {
            return ((IEqualityComparer<LambdaExpression>)PropertyPathComparer.Default).GetHashCode(TwoItemPath);
        }

        [Benchmark]
        public static object NotifyingPathGetOrCreateSingleItemPath()
        {
            return NotifyingPath.GetOrCreate(SingleItemPath);
        }

        [Benchmark]
        public static object NotifyingPathGetOrCreateTwoItemPath()
        {
            return NotifyingPath.GetOrCreate(TwoItemPath);
        }

        [Benchmark]
        public static object GetterGetOrCreateProperty()
        {
            return Getter.GetOrCreate(Property);
        }
    }
}
