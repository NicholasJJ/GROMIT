using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinerRock : CombinerObject
{
    // Start is called before the first frame update
    void Start()
    {
        CombinerObjectData data = new CombinerObjectData
        {
            description = "Rock",
            display = "1F5FF",
            code = "public override void Action()\n" +
                      "{\n" +
                      "    GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);\n" +
                      "    rock.transform.localScale = 3f * Vector3.one;\n" +
                      "    rock.GetComponent<Renderer>().material.color = Color.gray;\n" +
                      "    rock.transform.position = transform.position + transform.forward*2 + transform.up*4;\n" +
                      "    rock.AddComponent<DamageOnCollision>().damage = 30;\n" +
                      "    rock.AddComponent<Rigidbody>().AddForce(transform.up * -10 + transform.forward*100);\n" +
                      "    rock.AddComponent<DestroyAfterTime>().lifetime = 2;\n" +
                      "}\n"
    };
        Setup(data);
        CombinerManager.Instance.AddExample(data);
    }


    public override void Action()
    {
        GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rock.transform.localScale = 3f * Vector3.one;
        rock.GetComponent<Renderer>().material.color = Color.gray;
        rock.transform.position = transform.position + transform.forward*2 + transform.up*4;
        rock.AddComponent<DamageOnCollision>().damage = 30;
        rock.AddComponent<Rigidbody>().AddForce(transform.up * -10 + transform.forward*100);
        rock.AddComponent<DestroyAfterTime>().lifetime = 2;
    }
}
