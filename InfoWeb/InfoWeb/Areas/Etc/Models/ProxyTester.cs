using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace InfoWeb.Areas.Etc.Models
{
    /// <summary>
    /// Test proxy servers based on criteria.
    /// </summary>
    public class ProxyTester
    {
        public async Task<IEnumerable<TestResult<T>>> TestAllAsync<T>(IEnumerable<ProxyEndpoint> proxyEndpoints, Func<ProxyEndpoint, T> filterFunc)
        {
            List<Task<TestResult<T>>> tasks = new List<Task<TestResult<T>>>();
            foreach (ProxyEndpoint proxyServer in proxyEndpoints)
            {
                Task<TestResult<T>> task = new Task<TestResult<T>>(() => {
                    DateTime startTime = DateTime.Now;
                    T result = filterFunc(proxyServer);
                    TimeSpan duration = DateTime.Now - startTime;
                    return new TestResult<T>(proxyServer, duration, result);
                });
                task.Start();
                tasks.Add(task);
            }
            return await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public Task<TestResult<bool>> PickFastestAsync(IEnumerable<ProxyEndpoint> proxyEndpoints, Func<ProxyEndpoint, bool> filterFunc, CancellationToken cancellationToken)
        {
            List<Task<TestResult<bool>>> tasks = new List<Task<TestResult<bool>>>();
            
            TaskCompletionSource<TestResult<bool>> taskCompletionSource = new TaskCompletionSource<TestResult<bool>>();
            // Cancel the TCS when cancellation is requested
            cancellationToken.Register(() => taskCompletionSource.TrySetCanceled());

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            foreach (ProxyEndpoint proxyServer in proxyEndpoints)
            {
                Task<TestResult<bool>> task = new Task<TestResult<bool>>(() => {
                    DateTime startTime = DateTime.Now;
                    bool result = filterFunc(proxyServer);
                    TimeSpan duration = DateTime.Now - startTime;
                    TestResult<bool> testResult = new TestResult<bool>(proxyServer, duration, result);
                    if (result)
                    {
                        taskCompletionSource.TrySetResult(testResult);
                        if (!cancellationTokenSource.IsCancellationRequested)
                        {
                            cancellationTokenSource.Cancel();
                        }
                    }
                    return testResult;
                }, cancellationTokenSource.Token);
                task.Start();
                tasks.Add(task);
            }
            Task.Run(async () =>
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }, cancellationToken);

            return taskCompletionSource.Task;
        }
    }

    public enum ProxyType { Unspecified, Transparent, Anonymous, Elite }
    public class ProxyEndpoint
    {
        public string Hostname { get; private set; }
        public int Port { get; private set; }
        public ProxyType ProxyType { get; private set; }
        public ProxyEndpoint(string hostname, int port, ProxyType proxyType = ProxyType.Unspecified)
        {
            Hostname = hostname;
            Port = port;
            ProxyType = proxyType;
        }
    }

    public class TestResult<T>
    {
        public ProxyEndpoint ProxyEndpoint { get; private set; }
        public TimeSpan Duration { get; private set; }
        public T Result { get; private set; }
        public TestResult(ProxyEndpoint proxyEndpoint, TimeSpan duration, T result)
        {
            ProxyEndpoint = proxyEndpoint;
            Duration = duration;
            Result = result;
        }
    }

}