namespace StreamDSL.Old
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface ISource<out T>
    {
        void Run(Action<T> action);
    }

    public interface ITransform<in TI, out TO>
    {
        ISource<TO> Apply(ISource<TI> source);
    }

    public interface ISink<T>
    {
    }

    public class TransformedSource<TI, TO> : ISource<TO>
    {
        public TransformedSource(ISource<TI> source, ITransform<TI, TO> transform)
        {
            this.Source = source;
            this.Transform = transform;
        }

        public ISource<TI> Source { get; }

        public ITransform<TI, TO> Transform { get; }

        public void Run(Action<TO> action)
        {
            this.Transform.Apply(this.Source).Run(action);
        }
    }

    public class Outlet<T> : ISource<T>
    {
        public Outlet(ISource<T> source, ISink<T> sink)
        {
            this.Source = source;
            this.Sink = sink;
        }

        public ISource<T> Source { get; }

        public ISink<T> Sink { get; }

        public void Run(Action<T> action)
        {
            var actionSink = this.Sink as ActionSink<T>;
            if (actionSink != null)
                this.Source.Run(actionSink.Action);
        }
    }

    public class MapTransform<TI, TO> : ITransform<TI, TO>
    {
        public MapTransform(Func<TI, TO> map)
        {
            this.Map = map;
        }

        public Func<TI, TO> Map { get; }

        public ISource<TO> Apply(ISource<TI> source)
        {
            return new Imp(source, this.Map);
        }

        private class Imp : ISource<TO>
        {
            private readonly ISource<TI> source;
            private readonly Func<TI, TO> map;

            public Imp(ISource<TI> source, Func<TI, TO> map)
            {
                this.source = source;
                this.map = map;
            }

            public void Run(Action<TO> action)
            {
                this.source.Run(i => action(this.map(i)));
            }
        }
    }

    public interface IBoundedSource<T> : ISource<T>, IEnumerable<T>
    {
    }

    public class ListSource<T> : IBoundedSource<T>
    {
        public ListSource(IReadOnlyList<T> list)
        {
            this.List = list;
        }

        public IReadOnlyList<T> List { get; }

        public IEnumerator<T> GetEnumerator()
        {
            return this.List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public void Run(Action<T> action)
        {
            foreach (var item in List)
            {
                action(item);
            }
        }
    }

    public class ActionSink<T> : ISink<T>
    {
        public ActionSink(Action<T> action)
        {
            this.Action = action;
        }

        public Action<T> Action { get; }
    }

    public interface IBuilder
    {
        ISource<TO> Map<TI, TO>(ISource<TI> source, Func<TI, TO> map);
        ISource<T> To<T>(ISource<T> source, ISink<T> sink);
    }

    public class Workflow<T, TState>
    {
        public Workflow(ISource<T> source, TState state)
        {
            this.Source = source;
            this.State = state;
        }

        public ISource<T> Source { get; }

        public TState State { get; set; }
    }

    public class Runner : IBuilder
    {
        public ISource<TO> Map<TI, TO>(ISource<TI> source, Func<TI, TO> map)
        {
            return new TransformedSource<TI, TO>(source, new MapTransform<TI, TO>(map));
        }

        public ISource<T> To<T>(ISource<T> source, ISink<T> sink)
        {
            return new Outlet<T>(source, sink);
        }

        public void Run<T>(ISource<T> source)
        {
            source.Run(i => { });
        }
    }

    public class Visualizer : IBuilder
    {
        public ISource<TO> Map<TI, TO>(ISource<TI> source, Func<TI, TO> map)
        {
            return new TransformedSource<TI, TO>(source, new MapTransform<TI, TO>(map));
        }

        public ISource<T> To<T>(ISource<T> source, ISink<T> sink)
        {
            return new Outlet<T>(source, sink);
        }

        public void Run<T>(ISource<T> source)
        {
            source.Run(i => { });
        }
    }

    public class MyPipeline
    {
        public void DoWork()
        {
            IBuilder build = new Runner();

            var source = new ListSource<string>(new[] { "Hello", "World!" });
            var sink = new ActionSink<int>(Console.WriteLine);

            var countSource = build.Map(source, s => s.Length);
            var squareSource = build.Map(countSource, i => i * i);
            var pipeline = build.To(squareSource, sink);

            new Runner().Run(pipeline);
        }
    }
}
