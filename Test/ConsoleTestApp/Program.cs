﻿namespace ConsoleTestApp
{
    using System;
    using System.Reflection;
    using Features.A;
    using Features.Errors;
    using Miruken.Castle;
    using Miruken.Context;
    using Miruken.Mvc;
    using Miruken.Mvc.Castle;
    using Miruken.Mvc.Console;
    using static Miruken.Protocol;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var windsorHandler = new WindsorHandler(container =>
            {
                container.Install(
                    Miruken.Castle.Features.FromAssembly(
                        Assembly.GetExecutingAssembly()),
                    new MvcInstaller(),
                    new ConfigurationFactoryInstaller(),
                    new ResolvingInstaller());
            });

            Console.Title = "Console Test App";
            Console.CursorVisible = false;
            var appContext = new Context();
            appContext
                .AddHandlers(windsorHandler, new NavigateHandler(Window.Region))
                .AddHandler<IError>();

            P<INavigate>(appContext).Next<AController>(x =>
            {
                x.ShowAView();
                return true;
            });

            while (!Window.Quit)
            {
            }
        }
    }
}
