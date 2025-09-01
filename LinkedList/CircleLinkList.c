#include "CircleLinkList.h"


/// <summary>
/// 不要想太复杂
/// </summary>
/// <param name="L">头指针</param>
void InitCircleLinkList(LinkList* L) {

	// 申请内存，初始化头指针
	*L = (LinkList)malloc(sizeof(LNode));
	if (L == NULL)
	{
		printf("空间不足,初始化失败");
	}
	// 头节点指向自己，形成环 
	(*L)->next = *L;
	printf("链表已初始化");
}

void PrintCircleLinkList(LinkList L) {
	if (L == NULL)
	{
		return;
	}
	int length = 0;
	LinkList p = L;
	// 因为使用了头指针，头节点，所以要从头节点开始
	while (p->next->next != L->next) {
		printf("%d \t", p->next->data.data);
		// 保证能遍历链表的关键，不能改
		p = p->next;
	}
	if (length == 0)
	{
		printf("链表为空");
	}
}

Status CircleLinkListInsert(LinkList L, int data) {
	if (L == NULL)
	{
		printf("请初始化循环链表\n");
		return ERROR;
	}
	LNode* newNode = (LinkList)malloc(sizeof(LNode));
	if (newNode == NULL)
	{
		printf("内存不足，请重试\n");
	}
	newNode->data.data = data;
	newNode->next = L->next->next;
	L->next->next = newNode;
}