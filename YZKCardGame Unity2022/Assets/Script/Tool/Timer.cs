//Timer使用方法:
//通过CreateTimer()或者CreatePhysicsTimers()创建计时器
//然后直接.SetXxxxx().SetXxxxx().SetAction();即可设置计时器(类似于DoTween的链式编程)
//示例:
//TimerManager.CreateTimer().SetPlayerName("示例").SetTime(1).SetAction(Fun).SubscribeDeleteDelegate(Fun2);
//即可创建一个 名为"示例"，1秒钟运行一次，每次运行Fun()函数 的计时器,计时器删除时调用Fun2.
//如果不设置时间time或者time<0，那么会每帧运行一次
//如果不设置运行次数count或者count<0，那么就会永远运行
//删除时直接调用Timer.Delete()即可
using System;
using UnityEngine;

public enum TimerType
{
	normal,
	physicsTimer
}

public class Timer : MonoBehaviour
{
	public TimerType timerType;

	[SerializeField] string timerName;
	[SerializeField] float settingTime = 0;
	[SerializeField] float runningTime;
	[SerializeField] int settingCount = -1;
	[SerializeField] int runningCount;  //运行次数
	[SerializeField] bool isPause;

	Action timerExpirationCallback;
	Action<Timer> onDelete;

	public Timer SetName(string name) {
		timerName = name;
		return this;
	}
	public Timer SetTime(float time) {
		settingTime = time;
		return this;
	}
	public Timer SetCount(int count) {
		settingCount = count;
		return this;
	}
	public Timer SetPause(bool pause) {
		isPause = pause;
		return this;
	}
	public Timer SetAction(Action callback) {
		timerExpirationCallback = callback;
		return this;
	}
	/// <summary>
	/// 删除时运行动作
	/// </summary>
	public Timer SubscribeDeleteDelegate(Action<Timer> action) {
		onDelete += action;
		return this;
	}

	public void Delete() {
		onDelete?.Invoke(this);
		timerExpirationCallback = null;
		onDelete = null;
	}

	public void TimerTick(float addTime) {
		if (isPause)
			return;

		runningTime += addTime;
		if (runningTime >= settingTime)
		{
			if (settingTime < 0.1)
			{
				runningTime = 0;
			}
			else 
			{
				runningTime -= settingTime;
			}
			timerExpirationCallback?.Invoke();
			runningCount += 1;
			if (runningCount >= settingCount && settingCount >= 0)
				Delete();
		}
	}
}
