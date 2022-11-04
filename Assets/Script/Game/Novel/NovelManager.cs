using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class NovelManager : MonoBehaviour
{
    [SerializeField] GameObject dialogue = null;
    [SerializeField] GameObject choices = null;
    [SerializeField] Button readMore = null;
    [SerializeField] TextMeshProUGUI massage = null;
    [SerializeField] Text yBranchMassage = null;
    [SerializeField] Text nBranchMassage = null;
    [SerializeField] OjousamaNovel TextDatas = null;

	public RectTransform LeftTalker, RightTalker;

	public Image Back01, Back02,LeftTatie,RightTatie;

	private AudioSource Audio;

	private List<Sprite> Taties;
	private List<string> TatieNames;

	private string TargetText, AddText;

	private int CurrentSentenceID = -1,NowCharaNum = 0, NowLineNum = 1;

	public float NeedAddTime;
	private float AddTime;

	public bool CanNext=true,Showing,WhichBack=true;



	void Start()
	{
		CurrentSentenceID = SaveLoadManager.instance.NextNovel;

		//�����G�ǂݍ���
		Taties = Resources.LoadAll<Sprite>("Novel/Taties").ToList();
		TatieNames = Taties.Select(Tatie => Tatie.name).ToList();

		Audio = GetComponent<AudioSource>();

		//�ŏ���1�s�ڂ�\�����܂��B
		ReadmoreMessage();
	}

	private void Update()
	{
        if ((Input.GetMouseButtonDown(0)||Input.GetKeyDown(KeyCode.Space))&&CanNext)
        {
			CanNext = false;

			ReadmoreMessage();
        }
		//�ꊇ�\��
		else if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && Showing)
        {
			massage.text = TargetText;

			AddText = "";

			NowCharaNum = 0;

			Showing = false;
			CanNext = true;
        }

		if (Showing)
		{
			AddTime += Time.deltaTime;

			if (AddTime >= NeedAddTime)
			{
				string RichText = "";

				AddTime = 0;

                if (TargetText[NowCharaNum]=='<')
                {
                    while (true)
                    {
						RichText += TargetText[NowCharaNum];

						if (TargetText[NowCharaNum] == '>')
						{
							break;
						}

						NowCharaNum++;
                    }

					AddText += RichText;
                }
                else
                {
					AddText += TargetText[NowCharaNum];
				}

				massage.text = AddText;

				NowCharaNum++;

				//�����S���\�����I������Ȃ�
				if (NowCharaNum >= TargetText.Length)
				{
					AddText = "";

					NowCharaNum = 0;

					Showing = false;
					CanNext = true;
				}
			}
		}
	}

	//���͂̑�����\�����܂��B
	public void ReadmoreMessage()
	{
		//���͔ԍ������Ƃɕ����������܂��B
		Sentence result = SerchSentence(CurrentSentenceID);

		//�V�[���`�F���W
		SceneChange(result);

		//�����̃E�B���h�E���o��
		TalkerWindow(result);

		AboutTatie(result);

		BackGroundChange(result);

		BGM(result);

		SE(result);

		//����ȉ��o���Ȃ����`�F�b�N
		StartCoroutine("CheckSpecial",result);

		//���b�Z�[�W������ȏ�Ȃ��ꍇ�̓_�C�A���OUI���A�N�e�B�u�ɂ���B
		EndOfTalk(result);
        //����ꂽ���b�Z�[�W��\�����܂��B

        if (result.message!=""&&CanNext)
        {
			ShowingMassage(result);
		}

		//���͔ԍ����ЂƂ������ɐi�߂܂��B
		CurrentSentenceID++;
	}

	//���b�Z�[�W��\�������邾���ł��B
	void ShowingMassage(Sentence sentence)
	{
		TargetText = sentence.message;
		massage.text = "";

		Showing = true;
		CanNext = false;

		//massage.text = sentence.message;
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

	public void TalkerWindow(Sentence sentence)
    {
        if (sentence.Talker=="")
        {
			return;
        }

		string[] Splited = sentence.Talker.Split(',');

		switch (Splited[0])
        {
			case "Rise":

                {
					switch (Splited[2])
					{
						case "Left":

							LeftTalker.DOAnchorPosX(-960, 0.3f);

							LeftTalker.GetChild(0).GetComponent<TextMeshProUGUI>().text = Splited[1];

							break;

						case "Right":

							RightTalker.DOAnchorPosX(960, 0.3f);

							RightTalker.GetChild(0).GetComponent<TextMeshProUGUI>().text = Splited[1];

							break;
					}
				}

				break;

			case "Close":

                {
					switch (Splited[1])
					{
						case "Left":

							LeftTalker.DOAnchorPosX(-1460, 0.3f);

							break;

						case "Right":

							RightTalker.DOAnchorPosX(1460, 0.3f);

							break;

						case "Both":

							LeftTalker.DOAnchorPosX(-1460, 0.3f);
							RightTalker.DOAnchorPosX(1460, 0.3f);

							break;
					}
				}

				break;
        }

        
    }

	public void SE(Sentence sentence) 
	{
        if (sentence.SE=="")
        {
			return;
        }

		string[] content = sentence.SE.Split(',');

		switch (content[0])
		{
			case "Play":

				{
					BGMManager.instance.SEPlay(content[1], float.Parse(content[2]));
				}

				break;

			case "PlayLoop":

				BGMManager.instance.SetLoop("SE",true);
				BGMManager.instance.SEPlay(content[1], float.Parse(content[2]));

				break;

			case "PlayStopLoop":

				BGMManager.instance.SetLoop("SE", false);
				BGMManager.instance.SEPlay(content[1], float.Parse(content[2]));

				break;

			case "Stop":

				BGMManager.instance.SEStop(float.Parse(content[1]));

				break;
		}
	}

	public void BGM(Sentence sentence)
    {
        if (sentence.BGM=="")
        {
			return;
        }

		string[] content = sentence.BGM.Split(',');

		switch (content[0])
        {
			case "Play" :

                {
					BGMManager.instance.BGMPlay(content[1],float.Parse(content[2]));
                }

				break;

			case "PlayLoop":

				{
					BGMManager.instance.BGMPlay(content[1], float.Parse(content[2]));
					BGMManager.instance.SetLoop("BGM", true);
				}

				break;

			case "PlayStopLoop":

                {
					BGMManager.instance.BGMPlay(content[1], float.Parse(content[2]));
					BGMManager.instance.SetLoop("BGM", false);
				}

				break;

			case "Stop":

				{
					BGMManager.instance.BGMStop(float.Parse(content[1]));
				}

				break;
        }
    }

	public void AboutTatie(Sentence sentence)
    {
        if (sentence.Tatie=="")
        {
			return;
        }

		string[] content = sentence.Tatie.Split(',');
		Image Target = null;


		if (content[2] == "Left")
		{
			Target = LeftTatie;
		}
		else
		{
			Target = RightTatie;
		}

		//�^�C�v��
		switch (content[0])
        {
			case "FadeIn" :
				int Num = TatieNames.IndexOf(content[1]);

				Target.sprite = Taties[Num];

				Target.DOFade(1f,0.5f);

				break;

			case "FadeOut":

				if (content[2] == "Left")
				{
					Target = LeftTatie;
				}
				else if(content[2]=="Right")
				{
					Target = RightTatie;

				}else if (content[2]=="Both")
                {
					LeftTatie.DOFade(0f,0.5f);
					RightTatie.DOFade(0f,0.5f);

					break;
                }

				Target.DOFade(0f, 0.5f);

				break;

			case "Change":

				Num = TatieNames.IndexOf(content[1]);

				Target.sprite = Taties[Num];

				break;
        }
    }

	public void BackGroundChange(Sentence sentence)
    {
        if (sentence.BackGround=="")
        {
			return;
        }

		string[] content = sentence.BackGround.Split(',');

		float Time = float.Parse(content[1]);

		Sprite ChangeImage = Resources.Load<Sprite>("Novel/BackGrounds/"+content[0]);

		//01���N�����Ă���ꍇ02�ɂ�����
		if (WhichBack)
        {
			Back02.sprite = ChangeImage;

			Back01.DOFade(0f, Time);

			Back02.DOFade(1f,Time);

        }
		//02���N�����Ă���ꍇ01�ɂ�����
        else
        {
			Back01.sprite = ChangeImage;

			Back02.DOFade(0f, Time);

			Back01.DOFade(1f, Time);
		}

		WhichBack = !WhichBack;
    }

	public IEnumerator CheckSpecial(Sentence sentence)
    {


        if (sentence.Special.Contains("SceneFadeIn"))
        {
			CanNext = false;

			string[] content = sentence.Special.Split(',');

			GameObject.Find("Fader").GetComponent<Image>().DOFade(0f, float.Parse(content[1]));

			yield return new WaitForSeconds(float.Parse(content[1]));
        }
		else if (sentence.Special.Contains("SceneFadeOut"))
        {
			CanNext = false;

			string[] content = sentence.Special.Split(',');

			GameObject.Find("Fader").GetComponent<Image>().DOFade(1f, float.Parse(content[1])).OnComplete(()=> { massage.text = ""; });

			yield return new WaitForSeconds(float.Parse(content[1]));
		}
		else if (sentence.Special.Contains("GameEnd"))
        {
			CanNext = false;

			GameObject.Find("Fader").GetComponent<Image>().DOFade(1f, 5f).OnComplete(() => { massage.text = ""; });

			yield break;
		}

		if (sentence.NeedClick)
		{
			CanNext = true;
		}
		else
		{
			ReadmoreMessage();
		}
	}


	//�V�[���`�F���W���܂��B
	void SceneChange(Sentence sentence)
    {
        if (sentence.sceneChange!="")
        {
            if (sentence.nextBattle!="")
            {
				SaveLoadManager.instance.NextBattle = sentence.nextBattle;
            }

			SceneController.instance.StartSceneLoad(sentence.sceneChange);
        }
    }

	public void OnApplicationFocus(bool Focus)
    {

    }
}
