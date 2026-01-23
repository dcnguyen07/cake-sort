using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CakeShowComponent : MonoBehaviour
{
    [SerializeField] Camera cakeCamera;
    [SerializeField] List<MeshFilter> cakeSlideMeshs;
    [SerializeField] float normalCamZoom;
    [SerializeField] float unlockCamZoom;
    [SerializeField] Transform cakePlate;
    [SerializeField] Transform cakeShowPos;
    [SerializeField] Transform cakeStartPos;

    private void Start()
    {
        ShowNormalCake();
    }

    public void ShowNormalCake()
    {
        cakeCamera.orthographicSize = normalCamZoom;
    }

    public void ShowNewUnlockCake()
    {
        Mesh cakeSlideMesh = GameManager.Instance.cakeManager.GetNewUnlockedCakePieceMesh();
        cakeCamera.orthographicSize = unlockCamZoom;
        cakePlate.DOScale(1, 0.35f).From(0).SetEase(Ease.OutBack);
        cakePlate.DOMove(cakeShowPos.position, 0.35f).From(cakeStartPos.position).SetEase(Ease.OutBack);

        for (int i = 0; i < cakeSlideMeshs.Count; i++)
        {
            cakeSlideMeshs[i].mesh = cakeSlideMesh;
            cakeSlideMeshs[i].GetComponent<MeshRenderer>().material = GameManager.Instance.cakeManager.GetNewUnlockedCakePieceMaterial();
            cakeSlideMeshs[i].transform.DOScale(3.5f, 0.25f).SetDelay((i)*0.1f + 0.2f).From(0).SetEase(Ease.OutBack);
        }
    }
}
