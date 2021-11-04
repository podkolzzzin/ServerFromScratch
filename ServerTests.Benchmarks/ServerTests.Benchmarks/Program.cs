using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ServerTests.Benchmarks
{
    internal class InstanceCreator
    {
        private readonly Dictionary<Type, Func<object>> _activators = new Dictionary<Type, Func<object>>();

        public object CreateInstanceSlow(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public object CreateInstanceFast(Type type)
        {
            if (!_activators.TryGetValue(type, out var val))
                _activators.Add(type, val = CreateActivator(type));
            return val();
        }

        private Func<object> CreateActivator(Type type)
        {
            return (Func<object>)Expression.Lambda(typeof(Func<object>), Expression.New(type))
                .Compile();
        }
    }

    internal class TaskResultExtractor
    {
        private readonly Dictionary<Type, Func<Task, object>> _extractors = new Dictionary<Type, Func<Task, object>>();

        public object ExtractValueSlow(Task task)
        {
            var taskType = task.GetType();
            if (!taskType.IsGenericType)
                return null;
            return taskType.GetProperty("Result").GetValue(task);
        }

        public object ExtractValueFast(Task task)
        {
            var taskType = task.GetType();
            if (!taskType.IsGenericType)
                return null;

            if (!_extractors.TryGetValue(taskType, out var val))
                _extractors.Add(taskType, val = CreateExtractor(taskType));
            return val(task);
        }

        private Func<Task, object> CreateExtractor(Type taskType)
        {
            var param = Expression.Parameter(typeof(Task));
            return (Func<Task, object>)Expression.Lambda(typeof(Func<Task, object>),
                Expression.Convert(Expression.Property(Expression.Convert(param, taskType), "Result"), typeof(object)),
                param).Compile();
        }

        private object GetResult<T>(Task task) => (object)((Task<T>)task).Result;
    }

    class Test { }

    public class CreateBenchmark
    {
        private readonly InstanceCreator builder = new InstanceCreator();
        private readonly Type type = typeof(Test);

        public CreateBenchmark()
        {

        }

        [Benchmark(Baseline = true)]
        public object Slow() => builder.CreateInstanceSlow(type);

        [Benchmark]
        public object Fast() => builder.CreateInstanceFast(type);
    }

    public class GetResultBenchmark
    {
        private readonly TaskResultExtractor _getter = new TaskResultExtractor();
        private readonly Task<int> _task = Task.FromResult(10);

        public GetResultBenchmark()
        {

        }

        [Benchmark(Baseline = true)]
        public object Slow() => _getter.ExtractValueSlow(_task);

        [Benchmark]
        public object Fast() => _getter.ExtractValueFast(_task);
    }

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<CreateBenchmark>();
            //var inst = new TaskResultExtractor();
            //var t = inst.ExtractValueFast(Task.FromResult(10));
            //BenchmarkRunner.Run<GetResultBenchmark>();
        }
    }
}
