///////////////////////////////////////////////////////////////////////////
///File name: StatusIcon.cs
///Date Created: 14/12/2020
///Created by: JG
///Brief: Class for control of status icons
///Last Edited by: JG
///Last Edited on: 14/12/2020
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusIcon : MonoBehaviour
{

    private RawImage m_riAlertIcon;
    private RawImage m_riEngagedIcon;
    private RawImage m_riStunnedIcon;
    private Slider m_sSeeingIcon;

    [SerializeField]
    private float m_fAlphaStep;

    [SerializeField]
    private float m_fSolidTime;

    // Start is called before the first frame update
    public void Start()
    {

        try
        {

            m_riAlertIcon = transform.GetChild(0).GetComponent<RawImage>();
            m_riEngagedIcon = transform.GetChild(1).GetComponent<RawImage>();
            m_riStunnedIcon = transform.GetChild(2).GetComponent<RawImage>();
            m_sSeeingIcon = transform.GetChild(3).GetComponent<Slider>();

            m_riAlertIcon.enabled = false;
            m_riEngagedIcon.enabled = false;
            m_riStunnedIcon.enabled = false;
            m_sSeeingIcon.gameObject.SetActive(false);

        }
        catch (System.Exception)
        {

            throw;
        }

    }

    public void StartEngage()
    {
        m_riEngagedIcon.enabled = true;
    }

    public void StopEngage()
    {
        m_riEngagedIcon.enabled = false;
    }

    public void StartFade()
    {

        m_riAlertIcon.enabled = true;
        StartCoroutine(Fading());
    }

    public void StopFade()
    {

        m_riAlertIcon.enabled = false;
        StopCoroutine(Fading());
    }

    public void StartColourChange(float a_fTimeForChange)
    {
        m_riStunnedIcon.enabled = true;

        m_riStunnedIcon.canvasRenderer.SetColor(Color.blue);

        StartCoroutine(ColourChange(a_fTimeForChange));

    }

    public void EndColourChange()
    {

        m_riStunnedIcon.canvasRenderer.SetColor(Color.blue);

        m_riStunnedIcon.enabled = false;
    }

    public void SliderActiveState(bool a_bool)
    {
        m_sSeeingIcon.gameObject.SetActive(a_bool);

    }

    public void SetSliderValue(float a_fValueChange)
    {
        m_sSeeingIcon.value = a_fValueChange;
    }
    public float GetSliderValue()
    {
        return m_sSeeingIcon.value;
    }
    public void SetMaxSliderValue(float a_fValueChange)
    {
        m_sSeeingIcon.maxValue = a_fValueChange;
    }

    public IEnumerator Fading()
    {

        yield return new WaitForSeconds(m_fSolidTime);

        do
        {
            if (m_riAlertIcon.canvasRenderer.GetColor().a == 1)
            {
                m_riAlertIcon.CrossFadeAlpha(0, m_fAlphaStep, false);
            }

            if (m_riAlertIcon.canvasRenderer.GetColor().a == 0)
            {
                m_riAlertIcon.CrossFadeAlpha(1, m_fAlphaStep, false);
            }


            yield return new WaitForFixedUpdate();

        } while (true);

        yield return null;
    }


    public IEnumerator ColourChange(float a_fTimeToChange)
    {

        m_riStunnedIcon.CrossFadeColor(Color.red, a_fTimeToChange, false, false);

        yield return new WaitForSeconds(a_fTimeToChange);

        EndColourChange();

        yield return null;

    }

}
