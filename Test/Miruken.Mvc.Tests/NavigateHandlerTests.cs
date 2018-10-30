﻿namespace Miruken.Mvc.Tests
{
    using System;
    using Animation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Callback;
    using Callback.Policy;
    using Context;
    using Mvc.Options;

    [TestClass]
    public class NavigateHandlerTests
    {
        private Context _rootContext;
        private Navigator _navigate;

        static NavigateHandlerTests()
        {
            HandlerDescriptor.GetDescriptor<HelloController>();
            HandlerDescriptor.GetDescriptor<GoodbyeController>();
        }

        public class HelloController : Controller
        {
            [Provides, Contextual]
            public HelloController()
            {           
            }

            public RegionOptions SayHello(string name)
            {
                Console.WriteLine($"Hello {name}");
                var navigation = IO.Resolve<Navigation>();
                Assert.IsNotNull(navigation);
                Assert.AreSame(this, navigation.Controller);
                Push<GoodbyeController>().SayGoodbye(name);
                var options = new RegionOptions();
                return IO.Handle(options, true) ? options : null;
            }

            public void Partial()
            {
                var navigation = IO.Resolve<Navigation>();
                Assert.IsNotNull(navigation);
                Assert.AreSame(this, navigation.Controller);
                Assert.AreEqual(NavigationStyle.Partial, navigation.Style);
                Assert.AreSame(navigation, Context.Resolve<Navigation>());
            }

            public void Render()
            {
                Assert.IsNotNull(Show<TestView>());
                var navigation = IO.Resolve<Navigation>();
                Assert.IsNotNull(navigation);
                Assert.AreSame(this, navigation.Controller);
                Assert.AreSame(navigation, Context.Resolve<Navigation>());
            }
        }

        public class GoodbyeController : Controller
        {
            [Provides, Contextual]
            public GoodbyeController()
            {             
            }

            public string SayGoodbye(string name)
            {
                EndContext();
                return $"Goodbye {name}";
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _rootContext = new Context();
            _navigate    = new Navigator(new TestViewRegion());
            _rootContext.AddHandlers(new StaticHandler(), _navigate);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _rootContext.End();
        }

        [TestMethod]
        public void Should_Navigate_Next()
        {
            var controller = _rootContext.Next<HelloController>();
            controller.SayHello("Brenda");
            Assert.AreEqual(_rootContext, controller.Context.Parent);
        }

        [TestMethod]
        public void Should_Navigate_Push()
        {
            var controller = _rootContext.Push<HelloController>();
            controller.SayHello("Craig");
            Assert.AreEqual(_rootContext, controller.Context.Parent);
        }

        [TestMethod]
        public void Should_Navigate_Partial()
        {
            var controller = _rootContext.Partial<HelloController>();
            controller.Partial();
            Assert.AreEqual(_rootContext, controller.Context.Parent);
        }

        [TestMethod, 
         ExpectedException(typeof(InvalidOperationException))]
        public void Should_Reject_Initial_Property_Navigation()
        {
            var controller = _rootContext.Next<HelloController>();
            Assert.AreEqual(_rootContext, controller.Context);
        }

        [TestMethod]
        public void Should_Propagate_Next_Options()
        {
            var controller = 
                _rootContext.Push(Origin.MiddleLeft)
                .Next<HelloController>();
            var options = controller.SayHello("Kaitlyn");
            Assert.IsNotNull(options);
            var translation = options.Animation as Translate;
            Assert.IsNotNull(translation);
            Assert.AreEqual(Mode.InOut, translation.Mode);
            Assert.AreEqual(translation.Start, Origin.MiddleLeft);
        }

        [TestMethod]
        public void Should_Propagate_Push_Options()
        {
            var controller =
                _rootContext.SlideIn(Origin.MiddleRight)
                .Push<HelloController>();
            var options = controller.SayHello("Lauren");
            Assert.IsNotNull(options);
            var translation = options.Animation as Translate;
            Assert.IsNotNull(translation);
            Assert.AreEqual(Mode.In, translation.Mode);
            Assert.AreEqual(translation.Start, Origin.MiddleRight);
        }

        [TestMethod]
        public void Should_Render_A_View()
        {
            _rootContext.Next<HelloController>().Render();
        }

        [TestMethod, 
         ExpectedException(typeof(NotSupportedException))]
        public void Should_Fail_If_Context_Missing()
        {
            _navigate.Next<HelloController>().SayHello("Patches");
        }
    }
}
