using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class OutputArrowHandler : OutputHandler
{
    [SerializeField] GameObject indicator;

    private new void Start() {
        base.Start();
    }
    public override void HandleOutput(Output output)
    {
        var split = Regex.Matches(output.text, @"\{([^{}]+)\}*");
        
        foreach (Match match in split)
        {
          GameObject go = TextTree.Instance.StringToGameobject(match.Value.TrimStart('{').TrimEnd('}'));
          Debug.Log(go.name + " is in the text");
              var description = go.GetComponentInParent<TextDescription>();
              if (description != null) {
                  var centerAndSize = description.GetCenterAndSize();
                  var ind = Instantiate(indicator,centerAndSize.Item1,Quaternion.identity);
                  ind.transform.localScale = centerAndSize.Item2 * 1.5f;
              }
          
        }
    }
}
