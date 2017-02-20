namespace Gu.Wpf.Reactive.UiTests
{
    using FlaUI.Core;
    using FlaUI.Core.AutomationElements;
    using FlaUI.Core.Input;
    using FlaUI.Core.WindowsAPI;
    using FlaUI.UIA3;

    using NUnit.Framework;

    public abstract class WindowTests
    {
        private Application application;
        private UIA3Automation automation;

        protected Window Window { get; private set; }

        protected abstract string WindowName { get; }

        public void Restart()
        {
            if (this.Window != null)
            {
                Helpers.WaitUntilResponsive(this.Window);
            }

            this.application?.Dispose();
            this.application = Application.AttachOrLaunch(Info.CreateStartInfo(this.WindowName));
            this.automation = new UIA3Automation();
            this.Window = this.application.GetMainWindow(this.automation);
        }

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            this.Restart();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Keyboard.ReleaseScanCode((ushort)ScanCodeShort.CONTROL, false);
            Keyboard.ReleaseScanCode((ushort)ScanCodeShort.SHIFT, false);
            if (this.Window != null)
            {
                Helpers.WaitUntilResponsive(this.Window);
            }

            this.application?.Dispose();
        }
    }
}