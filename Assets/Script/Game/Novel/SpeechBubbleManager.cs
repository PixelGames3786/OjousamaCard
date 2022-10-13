using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SpeechBubbleManager : MonoBehaviour
{
	[SerializeField] GameObject dialogue = null;
	[SerializeField] GameObject choices = null;
	[SerializeField] Button readMore = null;
	[SerializeField] TextMeshProUGUI massage = null;
	[SerializeField] Text yBranchMassage = null;
	[SerializeField] Text nBranchMassage = null;
	[SerializeField] OjousamaNovel TextDatas = null;

	int CurrentSentenceID = 0;
	private void OnEnable()
	{
		//�ŏ���1�s�ڂ�\�����܂��B
		Sentence result = SerchSentence(CurrentSentenceID);
		ShowingMassage(result);
	}

	private void Update()
	{
		//Debug.Log(CurrentSentenceID);
	}

	//���͂̑�����\�����܂��B
	public void ReadmoreMessage()
	{
		//���͔ԍ����ЂƂ������ɐi�߂܂��B
		CurrentSentenceID++;

		//���͔ԍ������Ƃɕ����������܂��B
		Sentence result = SerchSentence(CurrentSentenceID);

		//�V�[���`�F���W
		SceneChange(result);

		//���b�Z�[�W������ȏ�Ȃ��ꍇ�̓_�C�A���OUI���A�N�e�B�u�ɂ���B
		EndOfTalk(result);
		//����ꂽ���b�Z�[�W��\�����܂��B
		ShowingMassage(result);
		ShowingMassageIsBranch(result);
		Connect(result);
	}

	//���b�Z�[�W��\�������邾���ł��B
	void ShowingMassage(Sentence sentence)
	{
		massage.text = sentence.message;
	}

	Sentence SerchSentence(int Id)
	{
		//�V�i���I�̃��b�Z�[�W�z��̒����當�͂�ID�Ō������Ď擾���܂��B
		Sentence result = TextDatas.sentences.First(
			(Sentence line) => { return line.id == Id; }
		);
		return result;
	}

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

		Sentence result = SerchSentence(CurrentSentenceID);
		Skip(result.choseYes);
	}

	//�ے��I�������Ƃ��A�{�^���R���|�[�l���g��Event����Ăяo����܂��B
	public void ChooseNo()
	{
		//�I���������������Ƃ�Readmore�{�^����L���ɂ��܂��B
		SwichReadmoreInteractable();
		//�I�������\���ɂ��܂��B
		SwichCoicesActivate();

		Sentence result = SerchSentence(CurrentSentenceID);
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
		Sentence result = SerchSentence(id);
		CurrentSentenceID = result.id;
		ShowingMassage(result);
	}

	//�Ō�̕��͂ɂȂ�����UI���\���ɂ��܂��B
	//��b���I������̂ɂ�������炸�AUI���\������Ă�����o�O�ł��B
	//�������\������K�v������܂��B
	void EndOfTalk(Sentence sentence)
	{
		if (sentence.endOfTalk)
		{
			//���̃[���͌��ݓǂ�ł��镶�͂����Z�b�g���邽�߂̐��l�ł��B
			CurrentSentenceID = 0;
			SwichDialougeActivate();
		}
	}



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
}
