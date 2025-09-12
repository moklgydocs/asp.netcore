#pragma once

#include"CommonHeadLib.h"


void InitCircleDoubleLinkList(DLinkList* L);

void PrintCircleDoubleLinkList(DLinkList L);

void PrevPrintCircleDoubleLinkList(DLinkList L);

Status InsertCircleDoubleLinkList(DLinkList L, int data);

Status TailInsertCircleDoubleLinkList(DLinkList L, int data);

Status DeleteCircleDoubleLinkListByElement(DLinkList L, int element);

int GetCDLinkListIndexByElement(DLinkList L, int element);

int GetCDLinkListElementByIndex(DLinkList L, int index);

Status InsertCDElementByIndex(DLinkList L, int index, int element);

Status InsertCDElementByElement(DLinkList L, int elementLocation, int element);

Status InsertCDElementBehindByElement(DLinkList L, int elementLocation, int element);

Status DestroyCircleDoubleLinkListByElement(DLinkList L, int element);