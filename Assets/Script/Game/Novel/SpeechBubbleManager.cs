using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SpeechBubbleManager : MonoBehaviour
{
	[SerializeField] RectTransform LeftSpeechBubble, RightSpeechBubble;
	[SerializeField] TextMeshProUGUI LeftMess, RightMess;
	[SerializeField] ScenarioBattle TextDatas = null;

	private RectTransform TargetBubble=null;
	private TextMeshProUGUI Target=null;
	private string TargetText,AddText;

	private int CurrentSentenceID = 0, NowCharaNum=0,NowLineNum=1;

	public float NeedAddTime;
	private float AddTime;

	private bool Showing, CanNext;

	public void MessageStart(int id)
	{
		CurrentSentenceID = id;
		BattleSentence result = SearchSentence(CurrentSentenceID);

		ShowingMassage(result);
	}

	void Update()
	{
        if (Showing)
        {
			AddTime += Time.deltaTime;

            if (AddTime>=NeedAddTime)
            {
				AddTime = 0;

				AddText += TargetText[NowCharaNum];

				Target.text = AddText;

				NowCharaNum++;

                if (NowCharaNum>NowLineNum*12)
                {
					NowLineNum++;

					TargetBubble.sizeDelta = new Vector2(500,50+(NowLineNum*50));
                }

				//�����S���\�����I������Ȃ�
                if (NowCharaNum>=TargetText.Length)
                {
					AddText = "";

					NowCharaNum = 0;
					NowLineNum = 1;

					Showing = false;
					CanNext = true;
                }
            }
        }
	}


	//���͂̑�����\�����܂��B
	public void ReadmoreMessage()
	{
		if (!CanNext) return;

		//���͔ԍ����ЂƂ������ɐi�߂܂��B
		CurrentSentenceID++;

		//���͔ԍ������Ƃɕ����������܂��B
		BattleSentence result = SearchSentence(CurrentSentenceID);

		//�Ȃ�炩�̓���ȃC�x���g���Ȃ����m�F
		IventCheck(result);

		//����ꂽ���b�Z�[�W��\�����܂��B
		ShowingMassage(result);
	}


	//���b�Z�[�W��\�������邾���ł��B
	void ShowingMassage(BattleSentence sentence)
	{
		LeftSpeechBubble.gameObject.SetActive(false);
		RightSpeechBubble.gameObject.SetActive(false);
		LeftMess.gameObject.SetActive(false);
		RightMess.gameObject.SetActive(false);

		switch (sentence.speakerSide)
		{
			case "Left":
				{
					TargetBubble = LeftSpeechBubble;
					TargetText = sentence.message;
					Target = LeftMess;
				}
				break;

			case "Right":
				{
					TargetBubble = RightSpeechBubble;
					TargetText = sentence.message;
					Target = RightMess;
				}
				break;
		}

		Target.text = "";

		TargetBubble.gameObject.SetActive(true);
		Target.gameObject.SetActive(true);

		Showing = true;
		CanNext = false;
	}

	BattleSentence SearchSentence(int Id)
	{
		//�V�i���I�̃��b�Z�[�W�z��̒����當�͂�ID�Ō������Ď擾���܂��B
		BattleSentence result = TextDatas.Sentences.First(
			(BattleSentence line) => { return line.id == Id; }
		);
		return result;
	}

	/*

	//�I�����̃��b�Z�[�W��\�������邾���ł��B
	//�I������UI�̓{�^���ƃe�L�X�g�ō\������Ă��܂��B
	void ShowingMassageIsBranch(Sentence sentence)
	{
		if (sentence.branch)
		{
			SwichReadmoreInteractable();
			SwichCoicesActivate();
			yBranchMassage.text = sentence.yesMessage;
			nBranchMassage.text = sentence.noMessage;
		}
	}

	//�m���I�������Ƃ��A�{�^���R���|�[�l���g��Event����Ăяo����܂��B
	public void ChooseYes()
	{
		//�I���������������Ƃ�Readmore�{�^����L���ɂ��܂��B
		SwichReadmoreInteractable();
		//�I�������\���ɂ��܂��B
		SwichCoicesActivate();

		Sentence result = SearchSentence(CurrentSentenceID);
		Skip(result.choseYes);
	}

	//�ے��I�������Ƃ��A�{�^���R���|�[�l���g��Event����Ăяo����܂��B
	public void ChooseNo()
	{
		//�I���������������Ƃ�Readmore�{�^����L���ɂ��܂��B
		SwichReadmoreInteractable();
		//�I�������\���ɂ��܂��B
		SwichCoicesActivate();

		Sentence result = SearchSentence(CurrentSentenceID);
		Skip(result.choseNo);
	}

	//���͂�ǂݔ�΂��܂��B
	//�I���̌��ʁA�V�i���I�����򂵂��̂���������Ƃ��ȂǂɎg�p���܂��B
	void Connect(Sentence sentence)
	{
		if (sentence.doConnect)
		{
			Skip(sentence.skipId);
		}
	}

	//�ʂ̕��͂ɓǂݔ�΂��܂��B
	void Skip(int id)
	{
		Sentence result = SearchSentence(id);
		CurrentSentenceID = result.id;
		ShowingMassage(result);
	}

	*/

	//�Ō�̕��͂ɂȂ�����UI���\���ɂ��܂��B
	//��b���I������̂ɂ�������炸�AUI���\������Ă�����o�O�ł��B
	//�������\������K�v������܂��B
	void IventCheck(BattleSentence sentence)
	{
		if (sentence.ivent=="BattleStart")
		{
			//���̃[���͌��ݓǂ�ł��镶�͂����Z�b�g���邽�߂̐��l�ł��B
			CurrentSentenceID = 0;
			Close();

			BattleManager.BM.BattleStart();
		}
	}

	void Close()
    {
		LeftSpeechBubble.gameObject.SetActive(false);
		RightSpeechBubble.gameObject.SetActive(false);
		LeftMess.gameObject.SetActive(false);
		RightMess.gameObject.SetActive(false);

		gameObject.SetActive(false);
	}

	/*
	//Readmore�{�^���������̉s��؂�ւ��܂��B
	//�I����������Ƃ��A�I�����ȊO�̃{�^���������Ȃ����邽�߂Ɏg�p���܂��B
	void SwichReadmoreInteractable()
	{
		if (readMore.interactable)
		{
			readMore.interactable = false;
		}
		else
		{
			readMore.interactable = true;
		}
	}

	//�A�N�e�B�u��Ԃ�؂�ւ��܂��B
	void SwichActiveState(GameObject target)
	{
		target.SetActive(!target.activeSelf);

	}

	//�I�����̃A�N�e�B�u��Ԃ�؂�ւ��܂��B
	void SwichCoicesActivate()
	{
		if (choices.activeSelf)
		{
			choices.SetActive(false);
		}
		else
		{
			choices.SetActive(true);
		}
	}

	//�ΘbUI�̃A�N�e�B�u��Ԃ�؂�ւ��܂��B
	void SwichDialougeActivate()
	{
		if (dialogue.activeSelf)
		{
			dialogue.SetActive(false);
		}
		else
		{
			dialogue.SetActive(true);
		}
	}


	//�V�[���`�F���W���܂��B
	void SceneChange(Sentence sentence)
	{
		if (sentence.sceneChange != "")
		{
			if (sentence.nextBattle != "")
			{
				SaveLoadManager.instance.NextBattle = sentence.nextBattle;
			}

			SceneManager.LoadScene(sentence.sceneChange);
		}
	}
	*/

}
