
/***
 * 
  *   解析字典
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicManger : MonoBehaviour 
{
    private  Dictionary<string,string> Dic=new Dictionary<string, string>();
	
	void Start ()
	{
		Dic.Add("1","lhl");
		Dic.Add("2","hhh");
		Dic.Add("3","aaa");
		Dic.Add("4","bbb");
		Dic.Add("5","ccc");
	    Dic.Add("6", "ddd");

    }

    void Awake()
    {
        StartCoroutine(Load());
    } 
    IEnumerator Load()
    {
        yield return new WaitForSeconds(0.1f); 
       // Debug.Log(LoadDic(Dic, "2"));
    }


    public string LoadDic(Dictionary<string,string> dic,string Key)
    {
        string Values = "";
        if (dic!=null)
        { 
            Dic.TryGetValue(Key, out Values); 
        } 
        return Values;
    }
}
