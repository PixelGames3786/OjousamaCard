using System.IO;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

// AssetDatabase���g�p���Ă��邽�߁A�r���h���ɂ͊܂߂Ȃ��悤�ɂ��Ȃ��ƃr���h�G���[���N����
#if UNITY_EDITOR
public class CreateCardsFromCsv : MonoBehaviour
{
    private const string AssetPath = "Assets/Datas/CardData/";

    // MenuItem������t���邱�Ƃ�Editor�̏㕔���j���[��`ScriptableObjects > CreateEnemyParamAsset`���\������܂�
    // ���������`CreateEnemyParamDataAsset()`�����s����܂�
    [MenuItem("ScriptableObjects/CreateCardAsset")]
    static void CreateCardDataAsset()
    {
        TextAsset csvFile= Resources.Load("CSV/CardParameterList") as TextAsset;

        //�ǂݍ���CSV�t�@�C�����i�[
        List<string[]> csvDatas = new List<string[]>();

        //CSV�t�@�C���̍s�����i�[
        int height = 0;

        //�ǂݍ��񂾃e�L�X�g��String�^�ɂ��Ċi�[
        StringReader reader = new StringReader(csvFile.text);
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            // ,�ŋ�؂���CSV�Ɋi�[
            csvDatas.Add(line.Split(','));
            height++; // �s�����Z
        }

        CardParameterList ParaList = ScriptableObject.CreateInstance<CardParameterList>();

        for (int i = 1; i < height; i++)
        {
            CardParameter CardAsset = ScriptableObject.CreateInstance<CardParameter>();

            ParaList.Cards.Add(CardAsset);

            //[i]�͍s���B[0]~[6]�͗�
            //csvDatas��String�^�Ȃ̂ł��̂܂܊i�[�ł���
            CardAsset.CardID = i;
            CardAsset.Name = csvDatas[i][1];

            CardAsset.Description = csvDatas[i][7];

            //���A���e�B�̕ύX�������ɂ����
            CardAsset.Cost = int.Parse(csvDatas[i][2]);

            string[] Moves = csvDatas[i][4].Split('-');

            CardAsset.MoveNum = Moves.Length;

            for (int u=0;u<Moves.Length;u++)
            {
                if (Moves[u]=="")
                {
                    continue;
                }

                //�^�C�v������
                if (Moves[u].Contains("�V�[���h"))
                {
                    CardAsset.Types.Add(CardParameter.CardType.Shield);

                    Moves[u] = Moves[u].Replace("�V�[���h","");
                }
                else if (Moves[u].Contains("�h���["))
                {
                    CardAsset.Types.Add(CardParameter.CardType.Draw);

                    Moves[u] = Moves[u].Replace("�h���[", "");

                }
                else if (Moves[u].Contains("�R�X�g��"))
                {
                    CardAsset.Types.Add(CardParameter.CardType.CostRecover);

                    Moves[u]=Moves[u].Replace("�R�X�g��", "");
                }
                else if (Moves[u].Contains("��"))
                {
                    CardAsset.Types.Add(CardParameter.CardType.Recover);

                    Moves[u] = Moves[u].Replace("��", "");

                }
                else if (Moves[u].Contains("����"))
                {
                    CardAsset.Types.Add(CardParameter.CardType.SelfDamage);

                    Moves[u] = Moves[u].Replace("����", "");

                }
                else
                {
                    CardAsset.Types.Add(CardParameter.CardType.Attack);
                }

                string[] Dameges = Moves[u].Split('~');

                CardAsset.MinPowers.Add(int.Parse(Dameges[0]));
                CardAsset.MaxPowers.Add(int.Parse(Dameges[1])+1);
            }

            CardAsset.ScriptName = csvDatas[i][9];

            //�摜���[�h
            if (AssetDatabase.LoadAssetAtPath("Assets/Arts/CardSprites/" + csvDatas[i][8] + ".png", typeof(Sprite)) != null)
            {
                CardAsset.Icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Arts/CardSprites/" + csvDatas[i][8] + ".png", typeof(Sprite));
            }

            //SE���[�h
            if (AssetDatabase.LoadAssetAtPath("Assets/Arts/Musics/SE/Cards/" + csvDatas[i][8] + ".mp3", typeof(AudioClip)) != null)
            {
                CardAsset.UseSE = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Arts/Musics/SE/Cards/" + csvDatas[i][8] + ".mp3", typeof(AudioClip));
            }

            CardAsset.WaitTime = Mathf.Clamp(float.Parse(csvDatas[i][10]), 1.5f, 999f);

            AssetDatabase.CreateAsset(CardAsset, AssetPath + csvDatas[i][8] + ".asset");

            AssetDatabase.Refresh();

        }

        AssetDatabase.CreateAsset(ParaList,"Assets/Resources/CardScriptList.asset");

        AssetDatabase.Refresh();


        //�Ⴆ��enemyParamAsset.EnemyParamList.Add(hogeParam); �I��

        // �������񂾌�͎��ۂɍ쐬���܂�
        // �����ō�����A�Z�b�g�̒u���ꏊ�ł���p�X�̎w����ł��܂�

        // Asset�쐬��A���f�����邽�߂ɕK�v�ȃ��\�b�h
    }
}
# endif
