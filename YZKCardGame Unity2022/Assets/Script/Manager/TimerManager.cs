//Timer使用方法:
//通过CreateTimer()或者CreatePhysicsTimers()创建计时器
//然后直接.SetXxxxx().SetXxxxx().SetAction();即可设置计时器(类似于DoTween的链式编程)
//示例:
//TimerManager.CreateTimer().SetPlayerName("示例").SetTime(1).SetAction(Fun).SubscribeDeleteDelegate(Fun2);
//即可创建一个 名为"示例"，1秒钟运行一次，每次运行Fun()函数 的计时器,计时器删除时调用Fun2.
//如果不设置时间time或者time<0，那么会每帧运行一次
//如果不设置运行次数count或者count<0，那么就会永远运行
//删除时直接调用Timer.Delete()即可
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class TimerManager : ManagerBase<TimerManager>
{
	//帧计时器
	public static List<Timer> timers = new List<Timer>();
	public static Timer CreateTimer() {
		Timer timer = ManagerObj.AddComponent<Timer>();
		timer.timerType = TimerType.normal;
		timer.SubscribeDeleteDelegate(RemoveTimerFromList);

		timers.Add(timer);
		return timer;
	}
	private static void RemoveTimerFromList(Timer timer) {
		timers.Remove(timer);
		if (timer != null)
			Destroy(timer);
	}

	//物理帧计时器
	public static List<Timer> physicsTimers = new List<Timer>();
	public static Timer CreatePhysicsTimers() {
		Timer timer = ManagerObj.AddComponent<Timer>();

		timer.timerType = TimerType.physicsTimer;
		timer.SubscribeDeleteDelegate(RemovePhysicsTimerFromList);

		physicsTimers.Add(timer);
		return timer;
	}
	private static void RemovePhysicsTimerFromList(Timer timer) {
		physicsTimers.Remove(timer);
		if (timer != null)
			Destroy(timer);
	}

	private void Update()
	{
		float time = Time.deltaTime;
		for (int i = timers.Count - 1; i >= 0; i--)
		{
			if (timers[i] != null)
				timers[i].TimerTick(time);
		}
	}
	private void FixedUpdate()
	{
		float time = Time.fixedDeltaTime;
		for (int i = physicsTimers.Count - 1; i >= 0; i--)
		{
			if (physicsTimers[i] != null)
				physicsTimers[i].TimerTick(time);
		}
	}
}
