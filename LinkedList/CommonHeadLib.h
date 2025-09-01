#pragma once


#define true 1
#define false 0
#define bool _Bool

#define Status int

#define ERROR 1
#define SUCCESS 0

typedef struct ElementType {
	int data;
}ElemType;

typedef struct LNode {
	ElemType data;
	struct LNode* next;
}LNode, * LinkList;
