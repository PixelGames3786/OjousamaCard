using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Sentence   //�Z���e���X�Ɠǂ݂܂��B���͂Ƃ����Ӗ��ł��B�V�i���I�͕��͂ō\������Ă���̂ŁA���̖��O�ɂ��܂����B
{
    public int id;              //�������邽�߂̈�ӂȐ��l�ł��B
    public string message;      //���b�Z�[�W�̖{���ł��B
    public bool branch;         //�I���������邩�𔻒肷�邽�߂Ɏg���܂��B
    public string yesMessage;   //�m��I�ȑI�����ł��B
    public string noMessage;    //�ے�I�ȑI�����ł��B
    public int choseYes;        //�m��I�ȑI������I�񂾂Ƃ��A�ǂ̕��͂ɕ��򂷂邩��ID�ł��B
    public int choseNo;         //�ے�I�ȑI������I�񂾂Ƃ��A�ǂ̕��͂ɕ��򂷂邩��ID�ł��B
    public bool doConnect;      //���򂵂���Ɏ�������Ƃ��ȂǂɎg���܂��B
    public int skipId;          //��������Ƃ��A�ǂ̕��͂���ǂނ̂���ID�ł��B
    public bool endOfTalk;      //�Θb���I������Ō�̕��͂Ȃ̂����肷��̂Ɏg���܂��B
    public string sceneChange;  //�V�[�����ڍs����Ƃ��Ɉڍs����V�[���̖��O�����܂��B
    public string nextBattle;   //�o�g���V�[���Ɉڍs����Ƃ��ɍs���o�g����ݒ肵�܂��B
    public string Talker;
    public string Tatie;
    public string BackGround;
}
