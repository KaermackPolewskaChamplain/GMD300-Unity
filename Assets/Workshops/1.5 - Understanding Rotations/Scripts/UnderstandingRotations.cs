using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnderstandingRotations : MonoBehaviour
{
    public Transform RotationCube;
    private Vector3 eulerRotation = Vector3.zero;
    
    public TMP_Text CurrentQuaternionText;
    public TMP_Text CurrentQuaternionEulerText;

    public TMP_Text XValueText;
    public TMP_Text YValueText;
    public TMP_Text ZValueText;

    public Slider XSlider;
    public Slider YSlider;
    public Slider ZSlider;

    void Update()
    {
        //Add the UI slider values to the cube rotation, multiplied by deltaTime to scale properly for the current frame
        eulerRotation = new Vector3(XSlider.value, YSlider.value, ZSlider.value) * Time.deltaTime;

        //Add the euler angle to the cube rotation
        //Multiplying a quaternion with another basically adds them up together.
        RotationCube.rotation *= Quaternion.Euler(eulerRotation);

        //Update the slider UI text
        XValueText.text = XSlider.value.ToString("F1");
        YValueText.text = YSlider.value.ToString("F1");
        ZValueText.text = ZSlider.value.ToString("F1");

        //Show the current quaternion rotation and current euler rotation in the HUD
        Quaternion currentQuaternionRotation = RotationCube.rotation;
        Vector3 currentEulerRotation = currentQuaternionRotation.eulerAngles;
        CurrentQuaternionText.text = "X="+currentQuaternionRotation.x.ToString("F1")+" Y="+currentQuaternionRotation.y.ToString("F1") + " Z="+currentQuaternionRotation.z.ToString("F1") + " W="+currentQuaternionRotation.w.ToString("F1");
        CurrentQuaternionEulerText.text = "X=" + currentEulerRotation.x.ToString("F1") + " Y=" + currentEulerRotation.y.ToString("F1") + " Z=" + currentEulerRotation.z.ToString("F1");

        //Debug lines to showcase the object axis
        Debug.DrawLine(transform.position, transform.right, Color.red);
        Debug.DrawLine(transform.position, transform.up, Color.green);
        Debug.DrawLine(transform.position, transform.forward, Color.blue);
    }
}
