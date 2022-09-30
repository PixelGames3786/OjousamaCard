using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public enum SceneType
    {
        Novel,
        Adventure,
    }

    public SceneType Scene;

    //�m�x���V�[���ŃZ�[�u�����ꍇ�̍s��
    public int CurrentNovelId;

    //�����ݒ�
    [SerializeField]
    public List<int> HavingCards=new List<int> { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 }, 
                     MyDecks=new List<int>() { 3,3,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2};
}
