using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUI_Interface : MonoBehaviour
{
    public TMP_InputField inp_Resolution;
    public Button btn_Start;

    // Start is called before the first frame update
    void Start()
    {
        OnResolutionChanged();
        RegisterEvents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void RegisterEvents(){
        inp_Resolution.onEndEdit.RemoveAllListeners ();
        inp_Resolution.onEndEdit.AddListener(delegate {OnResolutionChanged(); });
        btn_Start.onClick.RemoveAllListeners();
        btn_Start.onClick.AddListener(delegate {StartGenerateData(); });
    }
    private void OnResolutionChanged(){
        Debug.Log("Resolution changed");
        int _resolution = int.Parse(inp_Resolution.text);
        Debug.Log("Resolution: " + _resolution);

        //Chỉnh Screen Resolution về giá trị vuông với cạnh là  _resolution
        Screen.SetResolution(_resolution, _resolution, false);
    }

    private void StartGenerateData()
    {
        Debug.Log("Start generate data");
    }
}
