#pragma once
#include "LinkList.h"


/// <summary>
/// 初始化循环链表
/// </summary>
/// <param name="L"></param>
void InitCircleLinkList(LinkList* L);

/// <summary>
///   要打印的链表
/// </summary>
/// <param name="L"></param>
void PrintCircleLinkList(LinkList L);



/// <summary>
/// 循环链表删除，删除第一个值为data的节点
/// </summary>
/// <param name="L"></param>
/// <param name="data"></param>
/// <returns></returns>
Status CircleLinkListDelete(LinkList L, int data);

/// <summary>
/// 判断循环链表是否为空
/// </summary>
/// <param name="L"></param>
/// <returns></returns>
Status IsCircleLinkListEmpty(LinkList L);

/// <summary>
/// 获取循环链表的长度
/// </summary>
/// <param name="L"></param>
/// <returns></returns>
int GetCircleLinkListLength(LinkList L);

/// <summary>
/// 根据下标获取对应的元素，位置从0开始
/// </summary>
/// <param name="L"></param>
/// <param name="index"></param>
/// <returns></returns>
Status GetCircleLinkListElementByIndex(LinkList L, int index);

/// <summary>
/// 根据元素值，获取对应的下标，位置从0开始
/// </summary>
/// <param name="L"></param>
/// <param name="element"></param>
/// <returns></returns>
Status GetCircleLinkListLocationByElement(LinkList L, int element);

/// <summary>
/// 更新循环链表中第一个值为data的节点为newData
/// </summary>
/// <param name="L"></param>
/// <param name="data"></param>
/// <param name="newData"></param>
/// <returns></returns>
Status CircleLinkListUpdate(LinkList L, int data, int newData);

/// <summary>
/// 销毁循环链表
/// </summary>
/// <param name="L"></param>
/// <returns></returns>
Status DestroyCircleLinkList(LinkList L);

/// <summary>
/// 清空循环链表
/// </summary>
/// <param name="L"></param>
/// <returns></returns>
Status ClearCircleLinkList(LinkList L);

/// <summary>
/// 查找循环链表中是否存在值为data的节点
/// </summary>
/// <param name="L"></param>
/// <param name="data"></param>
/// <returns></returns>
Status CircleLinkListLocate(LinkList L, int data);

/// <summary>
/// 尾插入
/// </summary>
/// <param name="L"></param>
/// <param name="data"></param>
/// <returns></returns>
Status CircleLinkListTailInsert(LinkList L, int data);

/// <summary>
/// 头插法
/// </summary>
/// <param name="L"></param>
/// <param name="data"></param>
/// <returns></returns>
Status CircleLinkListHeadInsert(LinkList L, int data);

/// <summary>
/// 根据下标删除节点，位置从0开始
/// </summary>
/// <param name="L"></param>
/// <param name="index"></param>
/// <returns></returns>
Status CircleLinkListDeleteByIndex(LinkList L, int index);

/// <summary>
/// 根据节点指针删除节点
/// </summary>
/// <param name="L"></param>
/// <param name="node"></param>
/// <returns></returns>
Status CircleLinkListDeleteByNode(LinkList L, LNode* node);
