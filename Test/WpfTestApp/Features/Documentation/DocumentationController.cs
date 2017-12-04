﻿namespace WpfTestApp.Features.Documentation
{
    using System.Threading.Tasks;
    using HelloWorld;
    using Miruken.Mvc;
    using Miruken.Mvc.Animation;

    public class DocumentationController: Controller
    {
        public void Index()
        {
            Show<Documentation>();
        }

        public void Delay(int delay)
        {
            Task.Delay(delay)
                .ContinueWith(t => Show<Documentation>());
        }

        public void Done()
        {
            EndContext();
            /*
            Next<HelloWorldController>(IO
                .Zoom()
                .Fade()
                .Push(Position.BottomCenter))
                .Greet();
                */
        }
    }
}
