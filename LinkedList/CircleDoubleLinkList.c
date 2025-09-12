#include "CircleDoubleLinkList.h"

/// <summary>
/// 初始化循环双链表
/// </summary>
/// <param name="L"></param>
void InitCircleDoubleLinkList(DLinkList* L) {
	*L = (DLinkList)malloc(sizeof(DLNode));
	if (*L == NULL)
	{
		printf("内存分配失败\n");
		return;
	}
	(*L)->prev = *L;
	(*L)->next = *L;
	(*L)->length = 0;
	printf("初始化成功\n");
}

void PrintCircleDoubleLinkList(DLinkList L) {
	if (L == NULL)
	{
		printf("循环双链表未初始化，请检查\n");
		return;
	}
	DLinkList p = L;
	if (p->next == L)
	{
		printf("循环双链表为空，请检查\n");
		return;
	}
	while (p->next != L)
	{
		printf("%d\t", p->next->data);
		p = p->next;
	}
	printf("\n");
	printf("链表长度是：%d\n", L->length);
}


void PrevPrintCircleDoubleLinkList(DLinkList L) {
	if (L == NULL)
	{
		printf("循环双链表未初始化，请检查\n");
		return;
	}
	DLinkList p = L;
	while (p->prev != L)
	{
		printf("%d\t", p->prev->data);
		p = p->prev;
	}
	printf("\n");
}
/// <summary>
/// 头插法
/// </summary>
/// <param name="L"></param>
/// <param name="data"></param>
/// <returns></returns>
Status InsertCircleDoubleLinkList(DLinkList L, int data) {
	if (L == NULL)
	{
		printf("循环双链表未初始化，请检查\n");
		return ERROR;
	}
	// 创建移动指针
	DLinkList current = L;

	DLNode* newNode = (DLinkList)malloc(sizeof(DLNode));
	if (newNode == NULL)
	{
		printf("内存分配失败，请检查\n");
		return ERROR;
	}

	newNode->next = NULL;
	newNode->prev = NULL;
	newNode->data = data;

	// 头插法：在头节点后插入
	newNode->next = current->next;
	if (current->next != L) {
		current->next->prev = newNode;
	}
	newNode->prev = current;
	current->prev = newNode;
	current->next = newNode;
	// 更新链表长度
	L->length++;

	return SUCCESS;
}


/// <summary>
/// 尾插法
/// </summary>
/// <param name="L"></param>
/// <param name="data"></param>
/// <returns></returns>
Status TailInsertCircleDoubleLinkList(DLinkList L, int data) {
	if (L == NULL)
	{
		printf("循环双链表未初始化，请检查\n");
		return ERROR;
	}

	// 创建新节点
	DLNode* newNode = (DLinkList)malloc(sizeof(DLNode));
	if (newNode == NULL)
	{
		printf("内存分配失败，请检查\n");
		return ERROR;
	}

	newNode->next = NULL;
	newNode->prev = NULL;
	newNode->data = data;

	// current 作为"移动指针"来遍历链表
	DLinkList current = L;
	while (current->next != L) {
		//只是让current指向下一个节点,这个就是移动指针的关键步骤
		current = current->next;
	}
	// 在尾部插入新节点
	current->next = newNode;
	newNode->prev = current;
	newNode->next = L;
	L->prev = newNode;

	// 更新链表长度（注意：长度存储在头节点中）
	L->length++;

	return SUCCESS;
}

Status DeleteCircleDoubleLinkListByElement(DLinkList L, int element) {
	if (L == NULL)
	{
		printf("循环双链表未初始化，请检查\n");
		return ERROR;
	}
	if (L->next == NULL)
	{
		printf("循环双链表为空，请检查\n");
		return ERROR;
	}
	int index = 0;
	bool flag = 0;
	DLinkList current = L;
	while (current->next != L) {
		if (current->next->data == element) {
			flag = true;
			// 假设要删除的节点是 tempNode
			DLNode* tempNode = current->next;

			// 1. 让前一个节点指向后一个节点
			current->next = tempNode->next;

			// 2. 如果后一个节点存在，让它的prev指向前一个节点
			if (tempNode->next != NULL) {
				tempNode->next->prev = current;
			}

			// 3. 释放内存
			free(tempNode);

			// 4. 更新长度
			L->length--;
			break;
		}
		current = current->next;
		index++;
	}
	if (flag) {

		return SUCCESS;
	}
	else {
		printf("未找到对应的元素，请重试\n");
		return ERROR;
	}
}

/// <summary>
/// 根据元素找下标
/// </summary>
/// <returns></returns>
int GetCDLinkListIndexByElement(DLinkList L, int element) {
	if (L == NULL)
	{
		printf("循环双链表未初始化，请检查\n");
		return ERROR;
	}
	if (L->next == NULL)
	{
		printf("循环双链表为空，请检查\n");
		return ERROR;
	}
	int index = 1;
	bool flag = 0;
	DLinkList current = L;
	while (current->next != L) {
		if (current->next->data == element) {
			flag = true;
			index++;
			break;
		}
		index++;
		current = current->next;
	}
	if (flag) {
		return index;
	}
	else {
		printf("未找到对应的元素，请重试\n");
		return OVERFLOW;
	}
}

/// <summary>
/// 根据下标找元素
/// </summary>
/// <returns></returns>
int GetCDLinkListElementByIndex(DLinkList L, int index) {
	if (L == NULL)
	{
		printf("循环双链表未初始化，请检查\n");
		return ERROR;
	}
	if (L->next == NULL)
	{
		printf("循环双链表为空，请检查\n");
		return ERROR;
	}
	int i = 1;
	bool flag = 0;
	int data = 0;
	DLinkList current = L;
	while (current->next != L) {
		if (i == index) {
			flag = true;
			data = current->next->data;
			index++;
			break;
		}
		current = current->next;
		i++;
	}
	if (flag) {
		return data;
	}
	else {
		printf("未找到对应的元素，请重试\n");
		return OVERFLOW;
	}
}

/// <summary>
/// 根据指定位置插入元素
/// </summary>
/// <returns></returns>
Status InsertCDElementByIndex(DLinkList L, int index, int element) {
	if (L == NULL)
	{
		printf("循环双链表未初始化，请检查\n");
		return ERROR;
	}
	if (L->next == NULL)
	{
		printf("循环双链表为空，请检查\n");
		return ERROR;
	}
	if (index <= 0 || index >= L->length)
	{
		printf("请输入链表长度内的位置\n");
		return OVERFLOW;
	}
	// 初始化新插入节点
	DLNode* newNode = (DLinkList)malloc(sizeof(DLNode));
	if (newNode == NULL)
	{
		printf("新节点初始化失败，请检查\n");
		return OVERFLOW;
	}
	newNode->data = element;
	newNode->next = L;
	L->prev = newNode;

	DLinkList current = L;
	// 这里默认不以下标开始
	int i = 1;
	int flag = false;
	while (current->next != L) {
		if (i == index)
		{
			newNode->next = current->next;
			current->next->prev = newNode;
			newNode->prev = current;
			current->next = newNode;
			L->length++;
			flag = true;
			break;
		}
		i++;
		current = current->next;
	}
	if (flag)
	{
		return SUCCESS;
	}
	else {
		printf("未找到对应的位置\n");
		return ERROR;
	}
}

/// <summary>
/// 根据指定元素插入插入元素
/// </summary>
/// <returns></returns>
Status InsertCDElementByElement(DLinkList L, int elementLocation, int element) {
	if (L == NULL)
	{
		printf("循环双链表未初始化，请检查\n");
		return ERROR;
	}
	if (L->next == NULL)
	{
		printf("循环双链表为空，请检查\n");
		return ERROR;
	}
	// 初始化新插入节点
	DLNode* newNode = (DLinkList)malloc(sizeof(DLNode));
	if (newNode == L)
	{
		printf("新节点初始化失败，请检查\n");
		return OVERFLOW;
	}
	newNode->next = NULL;
	newNode->prev = NULL;
	newNode->data = element;

	DLinkList current = L;
	// 这里默认不以下标开始
	int i = 1;
	int flag = false;
	while (current->next) {
		if (current->next->data == elementLocation)
		{
			newNode->next = current->next;
			current->next->prev = newNode;
			newNode->prev = current;
			current->next = newNode;
			L->length++;
			flag = true;
			break;
		}
		i++;
		current = current->next;
	}
	if (flag)
	{
		return SUCCESS;
	}
	else {
		printf("未找到对应的位置\n");
		return ERROR;
	}
}

int GetCircleDoubleLinkListLength(DLinkList L) {
	if (L == NULL)
	{
		printf("循环双链表未初始化，请检查\n");
		return ERROR;
	}
	if (L->next == NULL)
	{
		printf("循环双链表为空，请检查\n");
		return ERROR;
	}
	return L->length;
}


/// <summary>
/// 在指定元素后面插入新元素
/// </summary>
/// <returns></returns>
Status InsertCDElementBehindByElement(DLinkList L, int elementLocation, int element) {
	if (L == NULL)
	{
		printf("循环双链表未初始化，请检查\n");
		return ERROR;
	}
	if (L->next == NULL)  // 头节点后无节点，链表为空
	{
		printf("循环双链表为空，无法找到指定元素\n");
		return ERROR;
	}

	// 初始化新插入节点
	DLNode* newNode = (DLNode*)malloc(sizeof(DLNode));
	if (newNode == NULL)
	{
		printf("新节点初始化失败，请检查\n");
		return OVERFLOW;
	}
	newNode->data = element;
	newNode->next = NULL;
	newNode->prev = NULL;

	DLNode* current = L;  // 从哨兵节点开始遍历
	int flag = false;

	while (current->next != NULL) {  // 遍历寻找目标元素
		DLNode* targetNode = current->next;  // 目标元素节点

		if (targetNode->data == elementLocation) {
			// 1. 新节点的后继指向目标节点的后继
			newNode->next = targetNode->next;
			// 2. 若目标节点不是尾节点，需更新其后继节点的前驱
			if (targetNode->next != NULL) {
				targetNode->next->prev = newNode;
			}
			// 3. 新节点的前驱指向目标节点
			newNode->prev = targetNode;
			// 4. 目标节点的后继指向新节点
			targetNode->next = newNode;

			L->length++;  // 链表长度+1
			flag = true;
			break;
		}
		current = current->next;
	}

	if (flag) {
		return SUCCESS;
	}
	else {
		printf("未找到元素 %d，插入失败\n", elementLocation);
		free(newNode);  // 未插入成功，释放新节点内存
		return ERROR;
	}
}