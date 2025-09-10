#include "DoubleLinkList.h"

/// <summary>
/// 初始化双链表
/// </summary>
/// <param name="L"></param>
void InitDoubleLinkList(DLinkList* L) {
	*L = (DLinkList)malloc(sizeof(DLNode));
	if (*L == NULL)
	{
		printf("内存分配失败");
		return;
	}
	(*L)->prev = NULL;
	(*L)->next = NULL;
	(*L)->length = 0;
	printf("初始化成功");
}

void PrintDoubleLinkList(DLinkList L) {
	if (L == NULL)
	{
		printf("双链表未初始化，请检查");
		return;
	}
	DLinkList p = L;
	while (p->next)
	{
		printf("%d\t", p->next->data);
		p = p->next;
	}
	printf("\n");
}

Status InsertDoubleLinkList(DLinkList L, int data) {
	if (L == NULL)
	{
		printf("双链表未初始化，请检查\n");
		return ERROR;
	}
	DLinkList dp = L;

	DLNode* newNode = (DLinkList)malloc(sizeof(DLNode));
	if (newNode == NULL)
	{
		printf("内存分配失败，请检查\n");
		return ERROR;
	}

	newNode->next = NULL;
	newNode->prev = NULL;
	newNode->data = data;

	if (dp->next == NULL)
	{
		newNode->next = dp->next;
		newNode->prev = dp;
		dp->next = newNode;
	}
	else {
		newNode->next = dp->next;
		dp->next->prev = newNode;
		newNode->prev = dp;
		dp->next = newNode;
	}
}