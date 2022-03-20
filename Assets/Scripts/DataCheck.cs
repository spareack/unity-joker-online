using System;
using System.Collections.Generic;
using UnityEngine;
public class DataCheck : MonoBehaviour
{
    public DataSave DS = new DataSave();
}
[Serializable]
public class DataSave
{
    public int[] achievementGoal = new int[25]
    {
        100, // 0 --- | �������� 100 ����������� ������
        10, // 1 --- | ����� 10 ��� ������ �������� �� ���� ������� �����
        20, // 2 --- | ����� 20 ��� ������ �������� �� ���� ������� �����
        30, // 3 --- | ����� 30 ��� ������ �������� �� ���� ������� �����
        50, // 4 --- | ����� 50 ��� ������ �������� �� ���� ������� �����
        5, // 5 --- | 5 ������ ������ ������ �������� ������
        3, // 6 --- | 3 ������� ������ ������ �������� 2 �������
        20, // 7 --- | ����� 20 �������� �� ���� ������
        3, // 8 --- | ������ ������, ��������� 3 ���� ������ ������ �����
        4, // 9 --- | ������ ������, ��������� 4 ���� ������ ������ �����
        5, // 10 --- | ������ ������, ��������� 5 ���� ������ ������ �����
        2, // 11 --- | 2 ������ �� ����
        3, // 12 --- | 3 ������ �� ����
        4, // 13 --- | 4 ������ �� ����
        4, // 14 --- | ����� 4 ���� ����� �� ������, ��� ���������� �� ���� �� 4 ���� ���������
        1, // 15 --- | ����� ���� 9 �� 9
        2, // 16 --- | ����� ���� 9 �� 9, ������ �� ��� ������
        3, // 17 --- | ����� ���� 9 �� 9, ������ �� ��� ������
        4, // 18 --- | ����� ���� 9 �� 9, ��������� �� ��� ������
        0, // 19 --- | �������� ������ � ���������� ���������������
        7000, // 20 --- | ����� 7000 ����� �� ������
        0, // 21 --- | �����, ���������� ������ ��� ������ 4 �����, ������ ������ � ��������� ������
        15, // 22 --- ������� 15 ��� � �������� ����� ������
        6, // 23 --- | ������ ������, ������� ��������� �������� 6 "�����" � ����� ������
        1 // 24 --- | �����, ��� ������� ������ �������� � ��������
    };
    public int[] achievementLevel = new int[25];
}
