using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject[] weapons;
    public float switchDelay;
    public int index = 0;
    private int maxWeaponHold = 2;
    private bool isSwitching = false;
    GunSystem gunSystem;
    void Start()
    {
        gunSystem = GetComponent<GunSystem>();
        // weapons = GetComponentsInChildren<GameObject>();
        // Debug.Log(weapons.Length);
        InitializeWeapons();
        gunSystem.InitializeWeapons();
    }
    private void InitializeWeapons(){
        for(int i = 0; i < weapons.Length; i++)
            weapons[i].SetActive(false);
        
        weapons[0].SetActive(true);
        // NetworkServer.Spawn(weapons[0]);
    }
    [Command]
    void CmdSwitchWeapon(int index){

        Debug.Log("Tell the server");
        RpcSwitchWeapon(index);
        // StartCoroutine(SwitchAfterDelay(index));
    }
    [ClientRpc]
    void RpcSwitchWeapon(int index){
        Debug.Log("Tell the clients");
        SwitchWeapons(index);
    }
    private void SwitchWeapons(int index){
        Debug.Log ("Equipping weapons for = " + this.name);

        for(int i = 0; i < weapons.Length; i++)
            weapons[i].SetActive(false);

        if(index < 0)
            index = maxWeaponHold - 1;

        
        weapons[index].SetActive(true);
        gunSystem.InitializeWeapons();
        // NetworkServer.Spawn(weapons[index]);
    }
    private IEnumerator SwitchAfterDelay(int index){
        isSwitching = true;

        yield return new WaitForSeconds(switchDelay);
        CmdSwitchWeapon(index);

        if(isLocalPlayer)
            SwitchWeapons(index);

        isSwitching = false;
    }

    void FixedUpdate()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && !isSwitching){
            index++;
            if(index >= maxWeaponHold)
                index = 0;

            StartCoroutine(SwitchAfterDelay(index));
            // isSwitching = true;
            // Invoke("SwitchAfterDelay(index)", switchDelay);
            Debug.Log(index);
        }   
    }
}