using UnityEngine;

public class Log
{
	public static string perfix = "" ;

	private static int perfixLength = 0;
	//public static void IncreasePerfixLength(string title = null) {
	public static void IncreasePerfixLength() {
		perfixLength++;
		perfix = new string('\t', perfixLength);
		//		if (title != null) {
		//			Debug.Log($"{perfix}¡ª¡ª¡ª¡ª        {title}        ¡ª¡ª¡ª¡ª");
		//		}
	}
	public static void ReducePerfixLength(){
		if(perfixLength <= 0){
			Debug.LogError("LogPrefix.ReducePerfixLength: perfixLength <= 0");
			return;
		}
		Debug.Log($"{perfix}½áÊø¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷¡÷");
		perfixLength--;
		perfix = new string('\t', perfixLength);
	}
}
