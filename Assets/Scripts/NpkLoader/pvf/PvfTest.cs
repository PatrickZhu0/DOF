using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using pvfLoaderXinyu;
using UnityEngine;

public class PvfTest : MonoBehaviour
{
    public string filePath;
    public string pvfFilename;
    // Start is called before the first frame update
    void Start()
    {
        //pvfFilename = "C:\\Users\\unknown\\Desktop\\Documents\\DNFTools\\Script.pvf";
        //var pvf = new Pvf(pvfFilename);//初始化pvf文件，进行读取操作

        //string fileContent = pvf.getPvfFileByPath("character/swordman/swordman.chr", Encoding.UTF8); 

        ////string stayContent = pvf.unpackAniFileByPath("character/fighter/animation/attack2.ani");

        ////pvf.dispose();//不用了就释放掉
        //Debug.LogError(fileContent);

        //BinaryAniCompiler.LoadAni("");

        filePath = Application.dataPath + "/../PvfRoot/character/swordman/swordman.chr";
        BinaryAniCompiler.LoadChr(filePath);

        //filePath = Application.dataPath + "/../PvfRoot/stringtable.bin";
        //BinaryAniCompiler.loadStringTableBin(filePath);

    }

    // Update is called once per frame
    void Update()
    {

    }

}
