using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class InputManager : MonoBehaviour
{

    [SerializeField]
    private InputField nameInput;

    private void Awake()
    {
        nameInput.onEndEdit.AddListener(delegate { ChangeName(nameInput.text); });

    }


    // Start is called before the first frame update
    public void ChangeName(string newName)
    {
        print(newName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
