
/***
 *  告警显示   
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BestHTTP;
using LitJson;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


#region 创建的类
 
public class GJ
{
    public string endTime;
    public int[] levels;
    public string startTime;
    public int[] status;
    public int[] types;


    public GJ()
    {

    }

    public GJ(string EndTime, int[] Levels, string StartTime, int[] Status, int[] Types)
    {
        endTime = EndTime;
        levels = Levels;
        startTime = StartTime;
        status = Status;
        types = Types;
    }
}
#endregion

public class Gaojing : MonoBehaviour
{

    public Toggle _GToggle;//告警按钮 
    private string EndTime;
    public Transform ParTransform; 
    public GameObject Parent;
    private static List<GameObject> Obj_list=new List<GameObject>();
  //  public GameObject Model;

    public Button zhengchang;
    public Button manyou;
    public Button zhibo;
    public Image cedianliebiao;
    
    private static int Sum=0;//总条数
    private static int OFFSET = 0;

    public Button zengjia;

     


    private void Awake()
    { 
        _GToggle.onValueChanged.AddListener(OpenG);

         zengjia.onClick.AddListener(() => { Zengjia(); });



        StartCoroutine(DeleteCread());//清楚列表内按钮

        StartCoroutine(CreatGJAction());//创建按钮
    }

    #region Toggle控制 
   
    private void OpenG(bool isok)
    {
        if (isok)
        {
            Parent.SetActive(true);
            zhengchang.enabled = false;
            manyou.enabled = false;
            zhibo.enabled = false;
            cedianliebiao.raycastTarget = true; 
            CreatGJObj(EndTime, new int[] { 1, 2, 3 }, "20180101102400520", new[] { 0, 1 }, new[] { 1, 2 }, OFFSET, 50);//0到50  
        }
        else
        {  
            Parent.SetActive(false); 
            zhengchang.enabled = true;
            manyou.enabled = true;
            zhibo.enabled = true;
            cedianliebiao.raycastTarget = false; 
        }
    }
    #endregion

    #region 增加个数

    public void Zengjia()
    { 
        
        OFFSET += 50;

        CreatGJObj(EndTime, new int[] { 1, 2, 3 }, "20180101102400520", new[] { 0, 1 }, new[] { 1, 2 }, OFFSET + 50, 50);//0到50 

        if (OFFSET%2!=0)
        {
            CreatGJObj(EndTime, new int[] { 1, 2, 3 }, "20180101102400520", new[] { 0, 1 }, new[] { 1, 2 }, OFFSET, Sum - OFFSET);//0到50
        } 
        Debug.Log(OFFSET + "输出个数");
        Debug.Log(Sum - OFFSET + "剩余个数");
        
    }

        #endregion


    #region 请求告警数据个数 

    IEnumerator CreatGJAction()
    {
        yield return new WaitForSeconds(0.1f);
        EndTime = string.Format("{0}{1}{2}{3}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, "102400520"); 

        GJAction(EndTime, new int[] { 1, 2, 3 }, "20180101102400520", new[] { 0, 1 }, new[] { 1, 2 });
    } 
    public void GJAction(string EndT,int[] Les,string StartT,int[] Sts,int[] Tys)
    {
        string url="49.4.95.9:8080/essential/count/criteria";
        HTTPRequest hTTPRequest = new HTTPRequest(new System.Uri("http://" + url), HTTPMethods.Post, FinishedGet);
        hTTPRequest.SetHeader("Content-type", "application/json; charset=utf-8");
        hTTPRequest.RawData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(new GJ(EndT,Les,StartT,Sts,Tys)));  
        hTTPRequest.Send();
    }

    private void FinishedGet(HTTPRequest arg1, HTTPResponse arg2)
    {  
        Sum=Int32.Parse(arg2.DataAsText);

     //  Debug.Log("总条数:" + Sum);  
        // CreatGJObj(EndTime, new int[] { 1, 2, 3 }, "20180101102400520", new[] { 0, 1 }, new[] { 1, 2 },sum); 
    }
    #endregion

    #region 创建克隆体
    public void CreatGJObj(string EndT, int[] Les, string StartT, int[] Sts, int[] Tys,int offset,int Sum)
    {
        string url = "49.4.95.9:8080/essential/criteria?offset="+ offset + "&length=" + Sum; 
        HTTPRequest hTTPRequest = new HTTPRequest(new System.Uri("http://" + url), HTTPMethods.Post, CreatGet);
        hTTPRequest.SetHeader("Content-type", "application/json; charset=utf-8");
        hTTPRequest.RawData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(new GJ(EndT, Les, StartT, Sts, Tys)));
        hTTPRequest.Send();
    } 

    private void CreatGet(HTTPRequest arg1, HTTPResponse arg2)
    {   

        JsonData jsonData = JsonMapper.ToObject(arg2.DataAsText); 


        for (int i = 0; i < jsonData.Count; i++)
        {
            var ObjClone = SmartPool.Spawn("GJ");  

            ObjClone.transform.SetParent(ParTransform,false); 

            ObjClone.transform.localScale=Vector3.one; 

            string Time = jsonData[i]["time"].ToString();
            ObjClone.GetComponent<GaojingRecard>().time.text = string.Format("{0}-{1}-{2}", Time.Substring(0, 4),Time.Substring(4, 2), Time.Substring(6, 2));
            ObjClone.GetComponent<GaojingRecard>().shebei.text = jsonData[i]["target"]["name"].ToString();
            ObjClone.GetComponent<GaojingRecard>().xinxi.text = jsonData[i]["message"].ToString();

            Obj_list.Add(ObjClone);

            for (int j = 0; j < jsonData[j].Count; j++)
            {
                ObjClone.GetComponent<GaojingRecard>().jibie.text = jsonData[3]["threshold"]["name"].ToString();
                ObjClone.GetComponent<GaojingRecard>().fanwei.text = string.Format("[{0},{1}]MPa", jsonData[3]["threshold"]["lower"], jsonData[3]["threshold"]["upper"]);
            } 
        } 
    }
    #endregion

    #region 删除克隆体

    IEnumerator DeleteCread()
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < Obj_list.Count; i++)
        {
            SmartPool.DespawnAllItems("GJ");
        }
        Obj_list.Clear();
    }

    #endregion

    #region 检测滑动
    //private int index = 0;
    //// 滑动加载  
    //public void Check()
    //{
    //private int index = 0;
    //// 滑动加载  
    //scrollView.setOnTouchListener(new OnTouchListener()
    //{

      
    //    public boolean onTouch(View v, MotionEvent event) {
            
    //        switch (event.getAction()) {  
    //            case MotionEvent.ACTION_DOWN :  
  
    //            break;  
    //            case MotionEvent.ACTION_MOVE :  
    //            index++;  
    //            break;  
    //            default :  
    //            break;  
    //        }  
    //        if (event.getAction() == MotionEvent.ACTION_UP &&  index > 0) {
    //            index = 0;
    //            View view = ((ScrollView)v).getChildAt(0);
    //            if (view.getMeasuredHeight() <= v.getScrollY() + v.getHeight())
    //            {
    //                //加载数据代码  
    //            }
    //        }  
    //        return false;
    //    }  
    //});  
    //}

    

    #endregion
}
