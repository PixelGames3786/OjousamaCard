using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class BattleSentence   //�Z���e���X�Ɠǂ݂܂��B���͂Ƃ����Ӗ��ł��B�V�i���I�͕��͂ō\������Ă���̂ŁA���̖��O�ɂ��܂����B
{
    public int id;              //�������邽�߂̈�ӂȐ��l�ł��B
    public string speaker;
    public string speakerSide;
    public string message;      //���b�Z�[�W�̖{���ł��B
    public string ivent;      //�Θb���I������Ō�̕��͂Ȃ̂����肷��̂Ɏg���܂��B
}