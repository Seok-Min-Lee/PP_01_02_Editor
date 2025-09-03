using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterButton : MonoBehaviour
{
    [SerializeField] private Image cover;

    private void Awake()
    {
        cover.gameObject.SetActive(false);
    }
    public void Highlight(bool value)
    {
        cover.gameObject.SetActive(value);
    }
}
