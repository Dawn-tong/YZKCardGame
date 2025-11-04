using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Authentication;
using UnityEngine;
using System.Threading.Tasks;

public class TestRelay : MonoBehaviour
{
	async void Start()
	{
		await InitializeRelay();
	}

	private async Task InitializeRelay()
	{
		try
		{
			// 1. 初始化Unity服务
			await UnityServices.InitializeAsync();
			Debug.Log("Unity服务初始化成功！");

			// 2. 匿名登录（不需要用户输入账号密码）
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
			Debug.Log($"匿名登录成功！玩家ID: {AuthenticationService.Instance.PlayerId}");

			// 3. 测试创建Relay分配
			var allocation = await RelayService.Instance.CreateAllocationAsync(4);
			var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
			Debug.Log($"Relay加入码: {joinCode}");

			Debug.Log("Relay测试完全成功！");
		}
		catch (System.Exception e)
		{
			Debug.LogError($"初始化失败: {e.Message}");
		}
	}

	// 在屏幕上显示按钮用于测试
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(10, 10, 300, 300));

		if (GUILayout.Button("创建Relay房间"))
		{
			CreateRelayRoom();
		}

		if (GUILayout.Button("加入Relay房间"))
		{
			// 这里后续可以添加加入功能
			Debug.Log("加入功能待实现");
		}

		GUILayout.EndArea();
	}

	private async void CreateRelayRoom()
	{
		try
		{
			var allocation = await RelayService.Instance.CreateAllocationAsync(4);
			var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
			Debug.Log($"房间创建成功！加入码: {joinCode}");

			// 这里可以显示加入码给朋友
		}
		catch (System.Exception e)
		{
			Debug.LogError($"创建房间失败: {e.Message}");
		}
	}
}