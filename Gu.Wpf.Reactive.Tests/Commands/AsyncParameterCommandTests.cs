// ReSharper disable AccessToDisposedClosure
namespace Gu.Wpf.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Threading;
    using System.Threading.Tasks;

    using Gu.Reactive;

    using Moq;

    using NUnit.Framework;

    public class AsyncParameterCommandTests
    {
        [Test]
        public void CanExecuteNoCondition()
        {
            using (var command = new AsyncCommand<int>(x => Task.FromResult(1)))
            {
                Assert.IsTrue(command.CanExecute(0));
                Assert.IsFalse(command.CancelCommand.CanExecute());
                Assert.IsInstanceOf<Condition>(command.Condition);
            }
        }

        [Test]
        public void CanCancel()
        {
            using (var command = new AsyncCommand<int>((i, x) => x.AsObservable().FirstAsync().ToTask()))
            {
                Assert.IsTrue(command.CanExecute(0));
                Assert.IsFalse(command.CancelCommand.CanExecute());
                command.Execute(0);
                Assert.IsTrue(command.CancelCommand.CanExecute());
                command.CancelCommand.Execute();
                Assert.IsFalse(command.CancelCommand.CanExecute());
                Assert.IsInstanceOf<Condition>(command.Condition);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CanExecuteCondition(bool expected)
        {
            using (var condition = Mock.Of<ICondition>(x => x.IsSatisfied == expected))
            {
                using (var command = new AsyncCommand<int>(x => Task.FromResult(1), condition))
                {
                    Assert.AreEqual(2, ((AndCondition)command.Condition).Prerequisites.Count);
                    Assert.AreEqual(expected, command.CanExecute(0));
                }
            }
        }

        [Test]
        public async Task ExecuteNotifiesCanExecuteChanged()
        {
            var count = 0;
            var tcs = new TaskCompletionSource<int>();
            using (var command = new AsyncCommand<int>(x => tcs.Task))
            {
                command.CanExecuteChanged += (_, __) => count++;
                var isExecutingCount = 0;
                using (command.ObservePropertyChangedSlim(nameof(command.IsExecuting), signalInitial: false)
                       .Subscribe(_ => isExecutingCount++))
                {
                    Assert.IsFalse(command.IsExecuting);
                    Assert.IsFalse(command.CancelCommand.CanExecute());
                    Assert.AreEqual(0, isExecutingCount);
                    command.Execute(1);
                    Assert.IsTrue(command.IsExecuting);
                    Assert.AreEqual(1, isExecutingCount);
                    Assert.IsFalse(command.CancelCommand.CanExecute());
                    Assert.AreEqual(1, count);
                    tcs.SetResult(1);
                    await command.Execution.Task.ConfigureAwait(false);
                    Assert.IsFalse(command.IsExecuting);
                    Assert.AreEqual(2, count);
                    Assert.AreEqual(2, isExecutingCount);
                }
            }
        }

        [Test]
        [Explicit("Not sure this is possible")]
        public async Task ExecuteNotifiesTaskStatus()
        {
            // http://stackoverflow.com/questions/34811639/is-there-a-way-to-be-notified-when-task-status-changes-to-running
            var tcs = new TaskCompletionSource<int>();
            using (var command = new AsyncCommand<int>(x => tcs.Task))
            {
                var taskStatuses = new List<TaskStatus>();
                using (command.ObservePropertyChangedWithValue(x => x.Execution.Status)
                       .Subscribe(x => taskStatuses.Add(x.EventArgs.Value)))
                {
                    Assert.IsFalse(command.IsExecuting);
                    Assert.IsFalse(command.CancelCommand.CanExecute());
                    command.Execute(1);
                    Assert.IsTrue(command.IsExecuting);
                    Assert.IsFalse(command.CancelCommand.CanExecute());
                    var expectedStatuses = new List<TaskStatus> { TaskStatus.Created, TaskStatus.WaitingForActivation, TaskStatus.Running, };
                    CollectionAssert.AreEqual(expectedStatuses, taskStatuses);
                    tcs.SetResult(1);
                    await command.Execution.Task.ConfigureAwait(false);
                    Assert.IsFalse(command.IsExecuting);
                    expectedStatuses.Add(TaskStatus.RanToCompletion);
                    CollectionAssert.AreEqual(expectedStatuses, taskStatuses);
                }
            }
        }

        [Test]
        public async Task ExecuteFinished()
        {
            var finished = Task.FromResult(1);
            using (var command = new AsyncCommand<int>(x => finished))
            {
                Assert.IsTrue(command.CanExecute(0));
                command.Execute(0);
                await command.Execution.Task.ConfigureAwait(false);
                Assert.IsTrue(command.CanExecute(0));
                Assert.AreSame(finished, command.Execution.Task);
                Assert.AreSame(finished, command.Execution.Completed);
            }
        }

        [Test]
        public void ExecuteCanceled()
        {
            var tcs = new TaskCompletionSource<int>();
            tcs.SetCanceled();
            using (var command = new AsyncCommand<int>(x => tcs.Task))
            {
                command.Execute(0);
                Assert.AreSame(tcs.Task, command.Execution.Task);
            }
        }

        [Test]
        public void ExecuteThrows()
        {
            var exception = new Exception();
            using (var command = new AsyncCommand<int>(x => Task.Run(() => { throw exception; })))
            {
                command.Execute(0);
                _ = Assert.ThrowsAsync<Exception>(() => command.Execution.Task);

                Assert.AreEqual(exception, command.Execution.InnerException);
                Assert.AreEqual(TaskStatus.Faulted, command.Execution.Status);
                Assert.AreEqual(true, command.CanExecute(0));
            }
        }

        [Test]
        public async Task CannotExecuteWhileRunning()
        {
            using (var resetEvent = new ManualResetEventSlim())
            {
                using (var command = new AsyncCommand<int>(x => Task.Run(() => resetEvent.Wait())))
                {
                    Assert.IsTrue(command.CanExecute(0));
                    command.Execute(0);
                    Assert.IsFalse(command.CanExecute(0));
                    resetEvent.Set();
                    await command.Execution.Task.ConfigureAwait(false);
                    Assert.IsTrue(command.CanExecute(0));
                }
            }
        }

        [Test]
        public void WithOneCondition()
        {
            var mock = new Mock<ICondition>(MockBehavior.Strict);
            mock.SetupGet(x => x.IsSatisfied).Returns(true);
            using (var command = new AsyncCommand<int>(x => Task.Run(() => { }), mock.Object))
            {
                Assert.IsTrue(command.CanExecute(1));

                mock.SetupGet(x => x.IsSatisfied).Returns(false);
                Assert.IsFalse(command.CanExecute(2));
            }

            mock.Verify(x => x.Dispose(), Times.Never);
        }

        [Test]
        public void WithTwoConditions()
        {
            var mock1 = new Mock<ICondition>(MockBehavior.Strict);
            mock1.SetupGet(x => x.IsSatisfied).Returns(true);

            var mock2 = new Mock<ICondition>(MockBehavior.Strict);
            mock2.SetupGet(x => x.IsSatisfied).Returns(true);
            using (var command = new AsyncCommand<int>(x => Task.Run(() => { }), mock1.Object, mock2.Object))
            {
                Assert.IsTrue(command.CanExecute(1));

                mock2.SetupGet(x => x.IsSatisfied).Returns(false);
                Assert.IsFalse(command.CanExecute(2));
            }

            mock1.Verify(x => x.Dispose(), Times.Never);
            mock2.Verify(x => x.Dispose(), Times.Never);
        }
    }
}
