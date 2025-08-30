#define true 1
#define false 0
#define bool _Bool

#define Status int

#define ERROR 1
#define SUCCESS 0

#include<stdio.h>
#include<stdlib.h>

typedef struct ElementType {
	int data;
}ElemType;

typedef struct LNode {
	ElemType data;
	struct LNode* next;
}LNode, * LinkList;

/// <summary>
/// 初始化链表
/// </summary>
/// <param name="L"></param>
/// <returns></returns>
Status InitLinkList(LinkList* L);

/// <summary>
/// 头插法
/// </summary>
/// <param name="L"></param>
/// <param name="data"></param>
/// <returns></returns>
Status HeadInsert(LinkList L, ElemType data);


/// <summary>
/// 尾插法
/// </summary>
/// <param name="L"></param>
/// <param name="data"></param>
/// <returns></returns>
Status TailInsert(LinkList L, ElemType data);

/// <summary>
/// 是否为空链表
/// </summary>
/// <param name="L"></param>
/// <returns></returns>
int IsEmpty(LinkList L);

/// <summary>
/// 删除元素
/// </summary>
/// <param name="L"></param>
/// <param name="data"></param>
/// <returns></returns>
Status Delete(LinkList L, int data);

/// <summary>
/// 更新链表中指定节点的数据。
/// </summary>
/// <param name="L">要操作的链表。</param>
/// <param name="data">需要查找并更新的原始数据值。</param>
/// <param name="newData">用于替换的新的数据值。</param>
/// <returns>如果更新成功，返回 Status 类型的成功状态；否则返回失败状态。</returns>
Status Update(LinkList L, int data, int newData);


/// <summary>
/// 在链表中查找指定数据元素的位置。
/// </summary>
/// <param name="L">要查找的链表。</param>
/// <param name="data">要查找的数据元素。</param>
/// <returns>如果找到，返回元素的位置；否则返回错误状态。</returns>
Status Locate(LinkList L, int data);

/// <summary>
/// 根据索引获取链表中的元素。
/// </summary>
/// <param name="L">要操作的链表。</param>
/// <param name="index">要获取元素的索引（从0开始）。</param>
/// <returns>操作的状态码，通常用于指示操作是否成功。</returns>
Status GetElementByIndex(LinkList L, int index);


/// <summary>
/// 根据元素值在链表中查找其位置。
/// </summary>
/// <param name="L">要查找的链表。</param>
/// <param name="element">要查找的元素值。</param>
/// <returns>如果找到，返回元素在链表中的位置（通常从1开始）；如果未找到，返回错误状态。</returns>
Status GetLocationByElement(LinkList L, int element);

/// <summary>
/// 打印链表中的所有元素。
/// </summary>
/// <param name="L">要打印的链表。</param>
/// <returns>操作的状态码，表示打印是否成功。</returns>
Status PrintLinkList(LinkList L);

/// <summary>
/// 销毁链表并释放其占用的内存空间。
/// </summary>
/// <param name="L">要销毁的链表。</param>
/// <returns>操作的状态码，指示销毁链表是否成功。</returns>
Status DestroyList(LinkList L);

/// <summary>
/// 获取链表的长度。
/// </summary>
/// <param name="L">要计算长度的链表。</param>
/// <returns>链表中的节点数量。</returns>
int GetLength(LinkList L);
