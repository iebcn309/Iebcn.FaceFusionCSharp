
using System.Diagnostics;

namespace Iebcn.FaceFusion;
public class ProcessManager
{
    public static Stopwatch Stopwatch = new Stopwatch();
    // 定义处理状态枚举
    public enum ProcessState
    {
        Pending,
        Checking,
        Processing,
        Stopping
    }

    // 使用字典来存储处理状态
    private static Dictionary<string, ProcessState> processStates = new Dictionary<string, ProcessState>
{
    { "pending", ProcessState.Pending },
    { "checking", ProcessState.Checking },
    { "processing", ProcessState.Processing },
    { "stopping", ProcessState.Stopping }
};

    // 获取当前处理状态
    public static ProcessState GetProcessState()
    {
        return processStates.Values.FirstOrDefault();
    }

    // 设置处理状态
    public static void SetProcessState(ProcessState processState)
    {
        processStates["processState"] = processState;
    }

    // 检查当前是否处于检查状态
    public static bool IsChecking()
    {
        return GetProcessState() == ProcessState.Checking;
    }

    // 检查当前是否处于处理状态
    public static bool IsProcessing()
    {
        return GetProcessState() == ProcessState.Processing;
    }

    // 检查当前是否处于停止状态
    public static bool IsStopping()
    {
        return GetProcessState() == ProcessState.Stopping;
    }

    // 检查当前是否处于待处理状态
    public static bool IsPending()
    {
        return GetProcessState() == ProcessState.Pending;
    }

    // 将状态设置为检查状态
    public static void Check()
    {
        SetProcessState(ProcessState.Checking);
    }

    // 将状态设置为处理状态
    public static void Start()
    {
        if (Stopwatch.ElapsedMilliseconds > 0)
        {
            Stopwatch.Reset();
        }
        Stopwatch.Start();
        SetProcessState(ProcessState.Processing);
    }

    // 将状态设置为停止状态
    public static void Stop()
    {
        SetProcessState(ProcessState.Stopping);
    }

    // 将状态设置回待处理状态
    public static void End()
    {
        Stopwatch.Stop();
        SetProcessState(ProcessState.Pending);
    }

    // 管理队列有效载荷
    public static IEnumerable<QueuePayload> Manage(List<QueuePayload> queuePayloads)
    {
        // 仅在处理状态下产生队列有效载荷
        if (IsProcessing())
        {
            foreach (var payload in queuePayloads)
            {
                yield return payload;
            }
        }
    }
}

// 定义队列有效载荷类
public class QueuePayload
{
    // 根据需要实现QueuePayload类
    public string FramePath { get; internal set; }
}

// 定义处理状态类
public class ProcessState
{
    // 这里不需要实现，仅作为枚举使用
}