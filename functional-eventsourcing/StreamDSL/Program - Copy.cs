namespace StreamDSL
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public interface ISource<out T>
    {
    }

    public interface ISink<T>
    {
    }

    public interface ITransform<in TI, out TO>
    {
        ISource<TO> Apply(ISource<TI> source);
    }

    public interface IRun
    {
        void Run();
    }


    public class Block<TI, TO> : IRun, ISink<TI>, ISource<TO>
    {
        public Block(ISource<TI> source, ITransform<TI, TO> transform, ISink<TO> sink)
        {
            this.Source = source;
            this.Transform = transform;
            this.Sink = sink;
        }

        public ISink<TO> Sink { get; set; }

        public ISource<TI> Source { get; set; }

        public ITransform<TI, TO> Transform { get; }

        public void Run()
        {
            if (this.Sink is IRun)
            {
                (this.Sink as IRun).Run();
                return;
            }

            var list = this.Transform.Apply(this.Source) as IBoundedSource<TO>;
            var action = this.Sink as IActionSink<TO>;
            foreach (var item in list)
                action.Action(item);
        }
    }

    public interface IBoundedSource<out T> : ISource<T>, IEnumerable<T>
    {
    }

    public class ListSource<T> : IBoundedSource<T>
    {
        public ListSource(IReadOnlyList<T> list)
        {
            this.List = list;
        }

        public IReadOnlyList<T> List { get; }
        
        public void Run(Action<T> action)
        {
            foreach (var item in List)
            {
                action(item);
            }
        }

        public IEnumerator<T> GetEnumerator() => this.List.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public interface IActionSink<T> : ISink<T>
    {
        Action<T> Action { get; }
    }

    public class ActionSink<T> : IActionSink<T>
    {
        public ActionSink(Action<T> action)
        {
            this.Action = action;
        }

        public Action<T> Action { get; }
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

        private class Imp : IBoundedSource<TO>
        {
            private readonly ISource<TI> source;
            private readonly Func<TI, TO> map;

            public Imp(ISource<TI> source, Func<TI, TO> map)
            {
                this.source = source;
                this.map = map;
            }

            public IEnumerator<TO> GetEnumerator() => (this.source as IBoundedSource<TI>).Select(this.map).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }
    }

    public interface IFlow<T>
    {
        IFlow<TO> Map<TO>(Func<T, TO> map);

        IRun To(ISink<T> sink);
    }

    public class Flow<T> : IFlow<T>
    {
        public Flow(ISource<T> source, Func<ISink<T>, IRun> factory)
        {
            this.Source = source;
            this.Factory = factory;
        }

        public ISource<T> Source { get; }
        public Func<ISink<T>, IRun> Factory { get; }

        public IFlow<TO> Map<TO>(Func<T, TO> map)
        {
            var transform = new MapTransform<T, TO>(map);
            var source2 = transform.Apply(this.Source);
            return new Flow<TO>(
                source2,
                sink => new Block<T, TO>(this.Source, transform, sink));
        }

        public IRun To(ISink<T> sink)
        {
            return this.Factory(sink);
        }
    }

    public class Printer: IRun
    {
        private readonly IEnumerable<string> log;

        public Printer(IEnumerable<string> log)
        {
            this.log = log;
        }

        public void Run()
        {
            foreach (var item in this.log)
                Console.WriteLine(item);
        }
    }

    public class Visual<T>: IFlow<T>
    {
        private readonly IEnumerable<string> log;

        public Visual(IEnumerable<string> log)
        {
            this.log = log;
        }

        public IFlow<TO> Map<TO>(Func<T, TO> map)
        {
            var s = $"{typeof(T).Name} -> {typeof(TO).Name}";
            return new Visual<TO>(this.log.Concat(new[] { s }));
        }

        public IRun To(ISink<T> sink)
        {
            var s = $"-> {sink.GetType().Name}";
            return new Printer(this.log.Concat(new[] { s }));
        }
    }

    public class Runner
    {
        public IFlow<T> From<T>(ISource<T> source)
        {
            return new Flow<T>(
                source,
                sink => new Block<T, T>(source, new MapTransform<T, T>(i => i), sink));
        }
    }

    public class Visualizer
    {
        public IFlow<T> From<T>(ISource<T> source)
        {
            return new Visual<T>(new [] { $"{source.GetType().Name} ->" });
        }
    }

    public class MyPipeline
    {
        public void DoWork()
        {
            var source = new ListSource<string>(new[] { "Hello", "World!" });
            var sink = new ActionSink<double>(Console.WriteLine);

            var runner = new Visualizer();
            runner.From(source)
                .Map(s => s.Length)
                .Map(i => i * Math.PI)
                .Map(Math.Round)
                .To(sink)
                .Run();

            //var transform = new MapTransform<string, int>(s => s.Length);
            //var transform2 = new MapTransform<int, double>(i => i * Math.PI);
            //var source2 = transform.Apply(source);

            //var innerBlock = new Block<int, double>(source2, transform2, sink);
            //var pipeline = new Block<string, int>(source, transform, innerBlock);
            //pipeline.Run();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            new MyPipeline().DoWork();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
